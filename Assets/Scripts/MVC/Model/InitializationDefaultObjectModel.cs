using System;
using System.Collections.Generic;
using UnityEngine;

public class InitializationDefaultObjectModel
{
    private List<InventoryNewItemEntry> defaultInventoryDatabase;
    private Dictionary<int, InventoryItemData> itemList;

    public event Action<InventoryNewItemEntry> OnAmountChanged;

    public InitializationDefaultObjectModel(List<InventoryNewItemEntry> defaultInventoryDatabase, Dictionary<int, InventoryItemData> itemList)
    {
        this.defaultInventoryDatabase = defaultInventoryDatabase;
        this.itemList = itemList;
    }

    public void GetItemData()
    {
        foreach (var child in defaultInventoryDatabase)
        {
            int currentAmountIteration = child.amout;
            int maxStackItem = itemList[child.itemID].MaxItemStack;
            bool isWeaponIntactItem = child.isWeaponIntact;

            while (currentAmountIteration > 0)
            {
                int stackAmount = Mathf.Min(currentAmountIteration, maxStackItem);
                currentAmountIteration -= stackAmount;

                InventoryNewItemEntry tempItemData = new InventoryNewItemEntry
                {
                    itemID = child.itemID,
                    amout = stackAmount,
                    isWeaponIntact = isWeaponIntactItem
                };

                OnAmountChanged?.Invoke(tempItemData);
            }
        }
    }
}