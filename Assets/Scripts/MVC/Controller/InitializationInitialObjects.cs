using System.Collections.Generic;
using UnityEngine;

public class InitializationInitialObjects : MonoBehaviour
{
    [SerializeField] private InventoryDatabase inventoryDatabase;
    [SerializeField] private List<InventoryNewItemEntry> defaultInventoryDatabase;

    private Dictionary<int, InventoryItemData> itemList;

    private Transform inventoryContent;

    public void SetupInitializationInitialObjects()
    {
        itemList = inventoryDatabase.GetDictionaryItems();

        //int index = 1;

        foreach(var child in defaultInventoryDatabase)
        {
            InventoryNewItemEntry currentItemData = defaultInventoryDatabase[child.itemID];

            int currentAmountIteration = currentItemData.amout;
            int maxStackItem = itemList[child.itemID].MaxItemStack;
            bool isWeaponIntactItem = currentItemData.isWeaponIntact;

            Debug.Log($"Item ID : {child.itemID}, Item Amount : {currentAmountIteration}, Item IsWeaponIntact : {isWeaponIntactItem}");
            Debug.Log($"Item ID : {child.itemID}, Item Max In Stack Amount : {maxStackItem}");

            while(currentAmountIteration > 0)
            {
                int stackAmount = Mathf.Min(currentAmountIteration, maxStackItem);
                Debug.Log($"New Item Created ID : {child.itemID}, Amount : {stackAmount}, IsWeaponIntact : {isWeaponIntactItem}");

                currentAmountIteration -= stackAmount;
            }

            return;
            //++index;
        }
    }
}