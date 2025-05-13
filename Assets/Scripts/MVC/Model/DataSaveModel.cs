using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataSaveModel
{
    public string documentToSaveIsEmpty { get; private set; } = "";
    public string filePath { get; private set; }

    public Dictionary<int, Dictionary<string, object>> inventoryData { get; private set; } = new Dictionary<int, Dictionary<string, object>>();

    public DataSaveModel(string filePath) { this.filePath = filePath; }

    public bool SearchJsonFile()
    {
        bool TestVariable = false;

        if (File.Exists(filePath))
        {
            string text = File.ReadAllText(filePath);

            if (text != documentToSaveIsEmpty)
            {
                TestVariable = true;
                inventoryData = GetInventoryItemsData();
            }
        }
        else
        {
            File.Create(filePath).Close();
        }

        return TestVariable;
    }

    public void SwapInventoryItem(int targetItemsIndex, int draggedItemsIndex)
    {
        Dictionary<string, object> tempTargetItem = inventoryData[targetItemsIndex];

        inventoryData[targetItemsIndex] = inventoryData[draggedItemsIndex];
        inventoryData[draggedItemsIndex] = tempTargetItem;
    }

    public Dictionary<int, Dictionary<string, object>> GetInventoryItemsData()
    {
        string json = File.ReadAllText(filePath);

        return JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, object>>>(json);
    }

    public void RecordingAllInventoryData(Dictionary<int, Dictionary<string, object>> inventoryData)
    {
        string jsonData = JsonConvert.SerializeObject(inventoryData, Formatting.Indented);

        File.WriteAllText(filePath, jsonData);
    }

    public void RemoveInventoryItem(int targetItemIndex)
    {
        Dictionary<int, Dictionary<string, object>> newInventoryData = new();

        inventoryData.Remove(targetItemIndex);

        int inventoryCount = inventoryData.Count + 1;

        for (int i = targetItemIndex + 1; i < inventoryCount; i++)
        {
            int itemTypeID = Convert.ToInt32(inventoryData[i]["itemTypeID"]);
            int itemAmount = Convert.ToInt32(inventoryData[i]["Amount"]);
            bool itemIsWeaponIntact = Convert.ToBoolean(inventoryData[i]["isWeaponIntact"]);

            Dictionary<string, object> tempItemData = new()
            {
                { "itemTypeID", itemTypeID},
                { "Amount", itemAmount},
                { "isWeaponIntact", itemIsWeaponIntact}
            };

            newInventoryData.Add(i, tempItemData);
        }

        for (int i = inventoryCount - 1; i > targetItemIndex; i--)
        {
            inventoryData.Remove(i);
        }
        foreach (var itemData in newInventoryData)
        {
            inventoryData.Add(itemData.Key - 1, itemData.Value);
        }
    }
}