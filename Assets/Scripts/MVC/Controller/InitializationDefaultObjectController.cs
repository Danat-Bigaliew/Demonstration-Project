using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InitializationDefaultObjectController : MonoBehaviour
{
    [SerializeField] private InventoryDatabase inventoryDatabase;
    [SerializeField] private List<InventoryNewItemEntry> defaultInventoryDatabase;

    private InitializationDefaultObjectModel model;
    private InitializationDefaultObjectView view;

    private Dictionary<int, InventoryItemData> itemList;

    private GameObject itemPrefab;
    private Transform inventoryContent;

    private InventoryController inventoryController;
    [Inject]
    public void Construct(InventoryController inventoryController)
    {
        this.inventoryController = inventoryController;
    }

    public void SetupInitializationDefaultObject()
    {
        inventoryContent = gameObject.transform;
        itemList = inventoryDatabase.GetDictionaryItems();
        itemPrefab = Resources.Load<GameObject>("Prefabs/Inventory Item");

        model = new InitializationDefaultObjectModel(defaultInventoryDatabase, itemList);
        view = new InitializationDefaultObjectView();

        model.OnAmountChanged += OnAmountChanged;
        //view.DefaultListenerButton += SendDefaultInventoryButton;

        model.GetItemData();
    }

    private void OnAmountChanged(InventoryNewItemEntry tempItemData)
    {
        view.SetupInventoryItem(tempItemData, itemList, inventoryContent, itemPrefab);
    }

    //private void SendDefaultInventoryButton(RectTransform pressedButton)
    //{
    //    inventoryController.SetButtonListener(pressedButton);
    //}
}