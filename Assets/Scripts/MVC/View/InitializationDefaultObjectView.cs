using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InitializationDefaultObjectView
{
    //public event Action<RectTransform> DefaultListenerButton;

    public void SetupInventoryItem
        (InventoryNewItemEntry tempItemData, Dictionary<int, InventoryItemData> itemList, Transform inventoryContent, GameObject itemPrefab)
    {
        GameObject newItem = GameObject.Instantiate(itemPrefab, inventoryContent);
        Button newItemButton = newItem.GetComponent<Button>();
        Image newItemSprite = newItem.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI newItemText = newItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        //DefaultListenerButton(newItem.GetComponent<RectTransform>());

        newItem.name = $"{inventoryContent.name} - {tempItemData.itemID}";
        newItemSprite.sprite = itemList[tempItemData.itemID].ItemSprite;
        newItemText.text = $"{tempItemData.amout}";

        switch (tempItemData.isWeaponIntact)
        {
            case true:
                Image newItemBackground = newItem.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                Color alfa = newItemBackground.color;

                alfa.a = 0.6f;
                newItemBackground.color = alfa;
                break;
        }
    }
}