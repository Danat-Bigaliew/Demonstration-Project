using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButtonsModel
{
    const int minimumValueInItem = 1;
    const int inactiveInventoryItem = -1;
    public Dictionary<string, Button> inventoryAdministrationButtonsDictionary { get; private set; }
    public Dictionary<string, object> activeItemDictionary { get; private set; }
    public int activeInventoryItem { get; private set; } = inactiveInventoryItem;

    private InventoryController inventoryController;
    private DataSaveController dataSaveController;

    public InventoryButtonsModel(InventoryController inventoryController, DataSaveController dataSaveController)
    {
        this.inventoryController = inventoryController;
        this.dataSaveController = dataSaveController;
    }

    public void SetInventoryButtonsListener()
    {
        foreach (var button in inventoryController.GetInventoryButtonsList())
        {
            int buttonIndex = button.Key;

            button.Value.onClick.RemoveAllListeners();
            button.Value.onClick.AddListener(() => UpdateActiveInventoryItem(buttonIndex));
        }
    }

    public void SetInventoryAdministrationButtonsListener(Transform inventoryAdministrationButtonsContainer)
    {
        inventoryAdministrationButtonsDictionary = new()
        {
            { "AddButton", inventoryAdministrationButtonsContainer.GetChild(0).GetComponent<Button>() },
            { "DeleteButton", inventoryAdministrationButtonsContainer.GetChild(1).GetComponent<Button>() },
            { "ChangeWeaponState", inventoryAdministrationButtonsContainer.GetChild(2).GetComponent<Button>() }
        };

        inventoryAdministrationButtonsDictionary["AddButton"].onClick.AddListener(() => ClickOnAddItemButton());
        inventoryAdministrationButtonsDictionary["DeleteButton"].onClick.AddListener(() => ClickOnDeleteItemButton());
        inventoryAdministrationButtonsDictionary["ChangeWeaponState"].onClick.AddListener(() => ClickOnChangeWeaponState());
    }

    private void UpdateActiveInventoryItem(int inventoryIndex)
    {
        if(dataSaveController == null)
            Debug.Log("dataSaveController == null");

        if (dataSaveController.GetInventoryItem(inventoryIndex) == null)
            Debug.Log("dataSaveController.GetInventoryItem == null");

        Dictionary<string, object> activeItemData = dataSaveController.GetInventoryItem(inventoryIndex);
        Button activeButton = inventoryController.GetInventoryButtonItem(inventoryIndex);

        activeInventoryItem = inventoryIndex;
    }

    private void ClickOnAddItemButton()
    {
        switch (activeInventoryItem)
        {
            case inactiveInventoryItem://Маркер отсутствия нажатого предмета инвентаря
                activeInventoryItem = GetRandomActiveButton();
                activeItemDictionary = GetInventoryItemData(activeInventoryItem);
                AddInventoryItem(activeItemDictionary);
                break;
            default:
                activeItemDictionary = GetInventoryItemData(activeInventoryItem);
                AddInventoryItem(activeItemDictionary);
                break;
        }
    }
    private void ClickOnDeleteItemButton()
    {
        switch (activeInventoryItem)
        {
            case inactiveInventoryItem://Создать метод вывода сообщения о том что кнопка не нажата
                break;
            default:
                activeItemDictionary = GetInventoryItemData(activeInventoryItem);
                DeleteInventoryItem(activeItemDictionary);
                break;
        }
    }
    private void ClickOnChangeWeaponState()
    {
        Dictionary<string, object> inventoryItemData = dataSaveController.GetInventoryItem(activeInventoryItem);
        InventoryItemData weaponItemData = inventoryController.GetItemFromInventoryDatabaseList(Convert.ToInt32(inventoryItemData["itemTypeID"]));

        switch (weaponItemData.ItemType)
        {
            case ItemType.Weapon:
                Debug.Log("Попытка изменить состояние оружия");
                ChangeWeaponState(activeInventoryItem);
                break;
        }
    }

    private void AddInventoryItem(Dictionary<string, object> activeItemDictionary)
    {
        InventoryItemData currentItem = (InventoryItemData)activeItemDictionary["currentItem"];

        int activeItemPositionIndex = Convert.ToInt32(activeItemDictionary["activeItemPositionIndex"]);
        int itemTypeID = Convert.ToInt32(activeItemDictionary["itemTypeID"]);
        int activeItemAmount = Convert.ToInt32(activeItemDictionary["itemAmount"]);

        if (activeItemAmount + 1 < currentItem.MaxItemStack)
        {
            inventoryController.UpdateInventoryAmountData(activeItemPositionIndex,
                NewItemAmountValue(activeItemPositionIndex, activeItemAmount, minimumValueInItem));
        }
        else
        {
            SearchForUnfilledItem(currentItem, itemTypeID, minimumValueInItem);
        }
    }
    private void DeleteInventoryItem(Dictionary<string, object> activeItemDictionary)
    {
        InventoryItemData currentItem = (InventoryItemData)activeItemDictionary["currentItem"];

        int activeItemPositionIndex = Convert.ToInt32(activeItemDictionary["activeItemPositionIndex"]);
        int itemTypeID = Convert.ToInt32(activeItemDictionary["itemTypeID"]);
        int activeItemAmount = Convert.ToInt32(activeItemDictionary["itemAmount"]);

        if (activeItemAmount - 1 >= minimumValueInItem)
        {
            inventoryController.UpdateInventoryAmountData(activeItemPositionIndex,
                NewItemAmountValue(activeItemPositionIndex, activeItemAmount, -minimumValueInItem));
        }
        else
        {
            //Debug.Log($"Следующий индекс 0, нужно удалить предмет. activeItemPositionIndex : {activeItemPositionIndex}");
            inventoryController.DeleteInventoryItem(activeItemPositionIndex);
        }

    }
    private void ChangeWeaponState(int itemIndex)
    {
        bool itemWeaponState = dataSaveController.ChangeItemWeaponState(itemIndex);
        inventoryController.ChangeWeaponState(itemIndex, itemWeaponState);
        //Debug.Log($"After Change State : {itemWeaponState}");
    }

    private Dictionary<string, object> GetInventoryItemData(int activeItemPositionIndex)
    {
        Dictionary<string, object> activeItemData = dataSaveController.GetInventoryItem(activeItemPositionIndex);
        InventoryItemData currentItem = inventoryController.GetItemFromInventoryDatabaseList(Convert.ToInt32(activeItemData["itemTypeID"]));

        int itemTypeID = Convert.ToInt32(activeItemData["itemTypeID"]);
        int itemAmount = Convert.ToInt32(activeItemData["Amount"]);

        return new()
        {
            { "itemData", activeItemData}, { "currentItem", currentItem},
            { "activeItemPositionIndex", activeItemPositionIndex},
            { "itemTypeID", itemTypeID},
            { "itemAmount", itemAmount}
        };
    }

    private int NewItemAmountValue(int activeItemIndex, int itemAmount, int addOrDeleteItemAmount)
    {
        //dataSaveController.ResizeAmountInventoryData(activeItemIndex, itemAmount + addOrDeleteItemAmount);
        //dataSaveModel.inventoryData[activeItemIndex]["Amount"] = itemAmount + addOrDeleteItemAmount;//Увеличиваем Amount на 1
        return dataSaveController.ResizeAmountInventoryData(activeItemIndex, itemAmount + addOrDeleteItemAmount);
    }

    private void SearchForUnfilledItem(InventoryItemData currentItem, int targetItemTypeID, int addOrDeleteItemAmount)
    {
        int itemIndexPosition = 0;
        int typeIDCompletedItem = 0;//Default value
        int newItemIndexPosition = 0;//Default value
        bool isWeaponIntactItem = false;//Default value
        bool IsUnfilledItemStack = false;

        foreach (var item in dataSaveController.GetInventoryDataDictionary())
        {
            int itemPosisionIndex = item.Key;
            int itemTypeID = Convert.ToInt32(item.Value["itemTypeID"]);
            int itemAmount = Convert.ToInt32(item.Value["Amount"]);

            if (itemTypeID == targetItemTypeID && itemAmount < currentItem.MaxItemStack)
            {
                inventoryController.UpdateInventoryAmountData(itemPosisionIndex,
                    NewItemAmountValue(itemPosisionIndex, itemAmount, addOrDeleteItemAmount));
                IsUnfilledItemStack = true;
                return;
            }
            else if (itemTypeID == targetItemTypeID && itemAmount == currentItem.MaxItemStack)
            {
                newItemIndexPosition = dataSaveController.GetInventoryDataDictionary().Count + 1;
                typeIDCompletedItem = itemTypeID;
                itemIndexPosition = item.Key;
                isWeaponIntactItem = Convert.ToBoolean(item.Value["isWeaponIntact"]);
            }
        }

        switch (IsUnfilledItemStack)
        {
            case false:
                inventoryController.CreateInventoryItem(newItemIndexPosition, typeIDCompletedItem, isWeaponIntactItem);
                break;
        }
    }
    private int GetRandomActiveButton()
    {
        int firstIndex = 0;
        int lastIndex = inventoryController.GetInventoryButtonsList().Count;

        return UnityEngine.Random.Range(firstIndex, lastIndex);
    }
}