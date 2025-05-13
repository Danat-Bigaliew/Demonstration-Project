using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItemEntry
{
    public int itemID;
    public InventoryItemData itemData;
}

[Serializable]
public class InventoryNewItemEntry
{
    public int itemID;
    public int amout;
    public bool isWeaponIntact = false;
}

[CreateAssetMenu(fileName = "InventoryDatabase", menuName = "Scriptable Objects/InventoryDatabase")]
public class InventoryDatabase : ScriptableObject
{
    [SerializeField] private List<InventoryItemEntry> allItems;

    private Dictionary<int, InventoryItemData> dict = new Dictionary<int, InventoryItemData>();

    public List<InventoryItemEntry> AllItems => allItems;

    public Dictionary<int, InventoryItemData> GetDictionaryItems()
    {
        foreach(var entry in allItems)
        {
            if (!dict.ContainsKey(entry.itemID))
            {
                dict.Add(entry.itemID, entry.itemData);
            }
        }

        return dict;
    }
}