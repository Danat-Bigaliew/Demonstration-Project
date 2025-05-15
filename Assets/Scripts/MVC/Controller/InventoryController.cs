using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Zenject;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private Camera UICamera;

    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private InventoryDatabase inventoryDatabase;
    [SerializeField] private List<InventoryNewItemEntry> defaultInventoryDatabase;

    [SerializeField] private float animationDuration;
    [SerializeField] private float defaultAlphaPercent = 1f;
    [SerializeField] private float unusualAlphaPercent = 0.6f;

    private float defaultInventoryItemPosX;
    private bool isButtonDown = false;

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
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        itemPrefab = Resources.Load<GameObject>("Prefabs/Inventory Item");

        dataSaveController.SearchInventoryDataFile();
        bool fileIsExist = dataSaveController.SearchForJSONFile();

        model = new InventoryModel(defaultInventoryDatabase);
        view = new InventoryView();

        model.SetInventoryDataBaseList(inventoryDatabase);
        model.OnAmountChanged += SetItemData;

        switch (fileIsExist)
        {
            case false://If JSON File Is empty
                model.OnAmountChanged += AddInventoryItemInInventoryDataDictionary;
                break;
        }

        model.GetItemData(dataSaveController.GetInventoryData());

        switch (fileIsExist)
        {
            case false:
                dataSaveController.RecordingInventroyData();
                break;
        }
    }

    public void SetItemData(int indexItem, InventoryNewItemEntry newItemData)
    {
        GameObject newItem = Instantiate(itemPrefab, inventoryContent);
        Button newItemButton = newItem.GetComponent<Button>();

        model.AddInventoryButtonsList(indexItem, newItemButton);
        view.SetInventoryItemUI(newItemData, model.iventoryDatabaseList, newItem, itemPrefab.name);
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

            ChangeAlphaUnpressedButtons(newItem.transform, unusualAlphaPercent);
        });
        trigger.triggers.Add(pointerDown);

        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) =>
        {
            StartCoroutine(GetTargetItem(newItem, indexItem, originalPosition));
            ChangeAlphaUnpressedButtons(newItem.transform, defaultAlphaPercent);
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

    private void ChangeAlphaUnpressedButtons(Transform pressedItem, float procentInvisible)
    {
        for (int i = 0; i < inventoryContent.childCount; i++)
        {
            Transform item = inventoryContent.transform.GetChild(i);

            if (item != pressedItem)
            {
                view.ChangeAlphaChannel(item, procentInvisible);
            }
        }
    }
    
    public void ChangeWeaponState(int weaponIndex, bool weaponState)
    {
        Transform item = inventoryContent.GetChild(weaponIndex).GetChild(0);
        Debug.Log($"weaponIndex : {weaponIndex}, itemBackground : {item}");

        switch (weaponState)
        {
            case true:
                view.ChangeAlphaChannel(item, unusualAlphaPercent);
                break;
            case false:
                float weaponAlphaValue = 0f;
                view.ChangeAlphaChannel(item, weaponAlphaValue);
                break;
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

            dataSaveController.SwapInventoryItem(localIndexTargetItem, localIndexDraggedItem);

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

    public Dictionary<int, Button> GetInventoryButtonsList() { return model.inventoryButtonsList; }
    public Button GetInventoryButtonItem(int indexButton) { return model.inventoryButtonsList[indexButton]; }
    public InventoryItemData GetItemFromInventoryDatabaseList(int itemIndex) { return model.iventoryDatabaseList[itemIndex]; }
    public void AddInventoryItemInInventoryDataDictionary(int indexItem, InventoryNewItemEntry newItemData)
    {
        Dictionary<string, object> tempDictionaryToSaveData = new()
           {
              { "itemTypeID", newItemData.itemID },
              { "Amount", newItemData.amout },
              { "isWeaponIntact", newItemData.isWeaponIntact }
           };

        dataSaveController.AddItemForInventoryData(indexItem, tempDictionaryToSaveData);
    }
    public void UpdateInventoryAmountData(int itemIndex, int newItemAmount)
    {
        if (itemIndex >= inventoryContent.childCount)
            itemIndex = inventoryContent.childCount - 1;

        Transform item = inventoryContent.GetChild(itemIndex).GetComponent<Transform>();

        view.UpdateInventoryItemAmountUI(item, newItemAmount);
    }
    public void DeleteInventoryItem(int deletedItemIndexPosition)
    {
        Transform deletedItem = inventoryContent.GetChild(deletedItemIndexPosition);

        model.DeleteInventoryButtonList(deletedItemIndexPosition);
        view.DeleteInventoryItem(inventoryContent, deletedItem);
        dataSaveController.RemoveInventoryItem(deletedItemIndexPosition);
        Debug.Log("Обновление слушателя");
        inventoryButtonsController.UpdateInventoryItemsListener();
    }
}