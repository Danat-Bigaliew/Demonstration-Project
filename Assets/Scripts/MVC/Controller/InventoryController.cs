using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Zenject;
using System;
using System.Linq;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private Camera UICamera;

    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private InventoryDatabase inventoryDatabase;
    [SerializeField] private List<InventoryNewItemEntry> defaultInventoryDatabase;

    [SerializeField] private float animationDuration;

    private float defaultInventoryItemPosX;
    private bool isButtonDown = false;

    public Dictionary<int, InventoryItemData> iventoryDatabaseList { get; private set; }

    private GameObject itemPrefab;
    public Transform inventoryContent { get; private set; }
    private RectTransform pressedInventoryItem;
    private GridLayoutGroup gridLayoutGroup;

    public InventoryModel model { get; private set; }
    private InventoryView view;

    [Inject] private DataSaveController dataSaveController;
    [Inject] private InventoryButtonsController inventoryButtonsController;
    private void Construct(DataSaveController DataSaveController, InventoryButtonsController InventoryButtonsController)
    {
        dataSaveController = DataSaveController;
        inventoryButtonsController = InventoryButtonsController;
    }

    public void SetupInventoryController()
    {
        inventoryContent = gameObject.transform;
        iventoryDatabaseList = inventoryDatabase.GetDictionaryItems();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        itemPrefab = Resources.Load<GameObject>("Prefabs/Inventory Item");

        dataSaveController.SearchInventoryDataFile();
        bool fileIsExist = dataSaveController.model.SearchJsonFile();

        model = new InventoryModel(defaultInventoryDatabase, iventoryDatabaseList);
        view = new InventoryView();

        model.OnAmountChanged += SetItemData;

        switch (fileIsExist)
        {
            case false://If JSON File Is empty
                model.OnAmountChanged += AddInventoryItemInInventoryDataDictionary;
                break;
        }

        model.GetItemData(dataSaveController.model.inventoryData);

        switch (fileIsExist)
        {
            case false:
                dataSaveController.model.RecordingAllInventoryData(dataSaveController.model.inventoryData);
                break;
        }
    }

    public void SetItemData(int indexItem, InventoryNewItemEntry newItemData)
    {
        GameObject newItem = Instantiate(itemPrefab, inventoryContent);
        Button newItemButton = newItem.GetComponent<Button>();

        model.AddInventoryButtonsList(indexItem, newItemButton);
        view.SetInventoryItemUI(newItemData, iventoryDatabaseList, newItem, itemPrefab.name);
        CustomButtonsListener(newItem.GetComponent<RectTransform>(), indexItem);
    }

    private void CustomButtonsListener(RectTransform newItem, int indexItem)
    {
        Vector2 pointerOffset = Vector2.zero;
        Vector2 originalPosition = Vector2.zero;

        EventTrigger trigger = newItem.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = newItem.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((data) =>
        {
            RectTransform itemRect = inventoryContent.GetChild(0).GetComponent<RectTransform>();
            PointerEventData ped = (PointerEventData)data;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(newItem, ped.position, UICamera, out pointerOffset);

            isButtonDown = true;
            gridLayoutGroup.enabled = false;
            originalPosition = newItem.anchoredPosition;
            defaultInventoryItemPosX = itemRect.anchoredPosition.x;

            ChangeAlfaUnpressedButtons(newItem.transform, 0.6f);
        });
        trigger.triggers.Add(pointerDown);

        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) =>
        {
            StartCoroutine(GetTargetItem(newItem, indexItem, originalPosition));
            ChangeAlfaUnpressedButtons(newItem.transform, 1f);
        });
        trigger.triggers.Add(pointerUp);

        var itemDrag = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
        itemDrag.callback.AddListener((data) =>
        {
            if (isButtonDown)
                view.NewButtonPosition(newItem, model.NewPressedButtonPosition(data, inventoryContent, UICamera, pointerOffset));
        });
        trigger.triggers.Add(itemDrag);
    }

    private void ChangeAlfaUnpressedButtons(Transform pressedItem, float procentInvisible)
    {
        for (int i = 0; i < inventoryContent.childCount; i++)
        {
            Transform child = inventoryContent.transform.GetChild(i);

            if (child != pressedItem)
            {
                view.ChangeAlphaChannel(child, procentInvisible);
            }
        }
    }

    public void CreateInventoryItem(int newItemIndexPosition,int typeIdCompletedItem, bool isWeaponIntactItem)
    {
        InventoryNewItemEntry newItemData = model.CreateInventoryNewItemEntry(typeIdCompletedItem, isWeaponIntactItem);
        AddInventoryItemInInventoryDataDictionary(newItemIndexPosition, newItemData);
        SetItemData(newItemIndexPosition, newItemData);
    }

    private IEnumerator GetTargetItem(RectTransform draggedItem, int indexDraggedItem, Vector2 defaultDraggedItemPosition)
    {
        int indexDefalutColumn = -1;

        int columnInRow = gridLayoutGroup.constraintCount;
        int draggedItemRow = model.GetPressedItemsRow(draggedItem, inventoryContent, columnInRow);
        int draggedItemColumn = model.GetPressedItemColumn(draggedItem, inventoryContent, columnInRow, draggedItemRow, defaultDraggedItemPosition);

        if (draggedItemColumn != indexDefalutColumn)
        {
            RectTransform targetItem = model.GetItemRect(
            draggedItem, inventoryContent, defaultDraggedItemPosition,
            columnInRow, draggedItemRow, draggedItemColumn);

            int localIndexTargetItem = draggedItem.transform.GetSiblingIndex();
            int localIndexDraggedItem = targetItem.transform.GetSiblingIndex();

            dataSaveController.model.SwapInventoryItem(localIndexTargetItem, localIndexDraggedItem);

            Dictionary<string, object> dataForDOTAnimation = new()
        {
            { "draggedItem", draggedItem },
            { "targetItem", targetItem },
            { "defaultDraggedItemPosition", defaultDraggedItemPosition },
            { "localIndexDraggedItem", localIndexDraggedItem },
            { "localIndexTargetItem", localIndexTargetItem },
            { "animationDuration", animationDuration }
        };

            yield return view.AnimationInventoryItem(dataForDOTAnimation);//Запуск и Ожидание завершения корутины
        }
        else
        {
            draggedItem.anchoredPosition = defaultDraggedItemPosition;
        }

        isButtonDown = false;
        gridLayoutGroup.enabled = true;
    }

    public void AddInventoryItemInInventoryDataDictionary(int indexItem, InventoryNewItemEntry newItemData)
    {
        Dictionary<string, object> tempDictionaryToSaveData = new()
           {
              { "itemTypeID", newItemData.itemID },
              { "Amount", newItemData.amout },
              { "isWeaponIntact", newItemData.isWeaponIntact }
           };

        dataSaveController.model.inventoryData.Add(indexItem, tempDictionaryToSaveData);
    }

    public void UpdateInventoryAmountData(int itemIndex, int newItemAmount)
    {
        Transform item = inventoryContent.GetChild(itemIndex).GetComponent<Transform>();

        view.UpdateInventoryItemAmountUI(item, newItemAmount);
    }
    public void DeleteInventoryItem(int deletedItemIndexPosition)
    {
        Transform deletedItem = inventoryContent.GetChild(deletedItemIndexPosition);

        model.DeleteInventoryButtonList(deletedItemIndexPosition);
        view.DeleteInventoryItem(inventoryContent, deletedItem);
        dataSaveController.RemoveInventoryItem(deletedItemIndexPosition);
        
        inventoryButtonsController.UpdateInventoryItemsListener();
    }
}