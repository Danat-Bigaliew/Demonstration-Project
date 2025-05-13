using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryModel
{
    private List<InventoryNewItemEntry> defaultInventoryDatabase;
    public Dictionary<int, Button> inventoryButtonsList { get; private set; } = new();
    public Dictionary<int, InventoryItemData> itemList { get; private set; } = new();

    public event Action<int, InventoryNewItemEntry> OnAmountChanged;

    public InventoryModel(List<InventoryNewItemEntry> defaultInventoryDatabase, Dictionary<int, InventoryItemData> itemList)
    {
        this.defaultInventoryDatabase = defaultInventoryDatabase;
        this.itemList = itemList;
    }

    public void GetItemData(Dictionary<int, Dictionary<string, object>> inventoryData)
    {
        if (inventoryData.Count > 0)
            GetItemFromJSONFile(inventoryData);
        else
            GetItemFromDefaultInventoryDatabase();
    }

    private void GetItemFromJSONFile(Dictionary<int, Dictionary<string, object>> inventoryData)
    {
        foreach(var firstItem in inventoryData)
        {
            int itemIndex = firstItem.Key;
            int itemTypeID = (int)(long)firstItem.Value["itemTypeID"];
            int itemAmount = (int)(long)firstItem.Value["Amount"];
            bool itemIsWeaponIntact = (bool)firstItem.Value["isWeaponIntact"];

            InventoryNewItemEntry tempItemData = CreateInventoryNewItemEntry(itemTypeID, itemIsWeaponIntact, itemAmount);

            OnAmountChanged?.Invoke(itemIndex, tempItemData);
        }
    }
    private void GetItemFromDefaultInventoryDatabase()
    {
        int itemIndex = 0;

        foreach (var child in defaultInventoryDatabase)
        {
            int currentAmountIteration = child.amout;
            int maxStackItem = itemList[child.itemID].MaxItemStack;
            bool isWeaponIntactItem = child.isWeaponIntact;

            while (currentAmountIteration > 0)
            {
                int itemAmount = Mathf.Min(currentAmountIteration, maxStackItem);
                currentAmountIteration -= itemAmount;

                InventoryNewItemEntry tempItemData = CreateInventoryNewItemEntry(child.itemID, isWeaponIntactItem, itemAmount);

                OnAmountChanged?.Invoke(itemIndex, tempItemData);
                ++itemIndex;
            }
        }
    }

    public Vector2 NewPressedButtonPosition(BaseEventData data, Transform inventoryContent, Camera UICamera, Vector2 pointerOffset)
    {
        PointerEventData ped = (PointerEventData)data;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
          inventoryContent as RectTransform,
          ped.position,
          UICamera,
          out localPoint);

        Vector2 targetPosition = localPoint - pointerOffset;

        return targetPosition;
    }
    public int GetPressedItemsRow(RectTransform draggedItem, Transform inventoryContent, int itemsInRow)
    {
        int totalRows = Mathf.CeilToInt((float)inventoryContent.childCount / itemsInRow);
        float draggedY = draggedItem.anchoredPosition.y;

        float minDistance = float.MaxValue;
        int closestRowIndex = -1;

        for (int row = 0; row < totalRows; row++)
        {
            List<RectTransform> rowItems = new List<RectTransform>();
            float sumY = 0f;

            for (int i = 0; i < itemsInRow; i++)
            {
                int idx = row * itemsInRow + i;
                if (idx >= inventoryContent.childCount) break;

                RectTransform rect = inventoryContent.GetChild(idx).GetComponent<RectTransform>();
                if (rect != null && rect != draggedItem)
                {
                    rowItems.Add(rect);
                    sumY += rect.anchoredPosition.y;
                }
            }

            if (rowItems.Count == 0) continue;

            float avgY = sumY / rowItems.Count;
            float distance = Mathf.Abs(draggedY - avgY);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestRowIndex = row;
            }
        }

        //if (closestRowIndex != -1)
        //{
        //    //Debug.Log($"Ближайшая строка к '{draggedItem.name}': Row {closestRowIndex}");
        //}

        return closestRowIndex;
    }
    public int GetPressedItemColumn(RectTransform draggedItem, Transform content, int itemsInRow, 
        int draggedItemRow, Vector2 defaultDraggedItemPosition)
    {
        float thresholdX = draggedItem.rect.width * 0.3f;
        float thresholdY = draggedItem.rect.height * 0.3f;

        float deltaX = Mathf.Abs(draggedItem.anchoredPosition.x - defaultDraggedItemPosition.x);
        float deltaY = Mathf.Abs(draggedItem.anchoredPosition.y - defaultDraggedItemPosition.y);

        if (deltaX < thresholdX && deltaY < thresholdY)
            return -1;

        int startIndex = draggedItemRow * itemsInRow;
        int endIndex = Mathf.Min(startIndex + itemsInRow, content.childCount);

        float draggedX = draggedItem.anchoredPosition.x;
        float minDistance = float.MaxValue;
        int closestColumnIndex = -1;

        int leftCount = 0;
        int rightCount = 0;

        for (int i = startIndex; i < endIndex; i++)
        {
            RectTransform rect = content.GetChild(i).GetComponent<RectTransform>();
            if (rect == null || rect == draggedItem)
                continue;

            float itemX = rect.anchoredPosition.x;
            if (itemX < draggedX)
                leftCount++;
            else if (itemX > draggedX)
                rightCount++;

            float distance = Mathf.Abs(draggedX - itemX);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestColumnIndex = i - startIndex;
            }
        }

        if (closestColumnIndex != -1)
        {
            if (leftCount == 0 && rightCount > 0)
            {
                closestColumnIndex = 0;
            }
            else if (rightCount == 0 && leftCount > 0)
            {
                Debug.Log("Предмет находится на позиции ПОСЛЕДНЕГО элемента в строке.");
            }
            else
            {
                Debug.Log($"Ближайший столбец к '{draggedItem.name}': Column {closestColumnIndex}");
            }
        }
        else
        {
            Debug.Log("Не удалось найти ближайший столбец.");
        }

        return closestColumnIndex;
    }
    public RectTransform GetItemRect(RectTransform draggedItem, Transform content, Vector2 defaultPressedItemPosition,
    int columnInRow, int draggedItemRow, int draggedItemColumn)
    {
        int targetIndex = draggedItemRow * columnInRow + draggedItemColumn;

        if (targetIndex >= 0 && targetIndex < content.childCount)
        {
            RectTransform targetItem = content.GetChild(targetIndex).GetComponent<RectTransform>();
            return targetItem;
        }
        else
        {
            return null;
        }
    }

    public InventoryNewItemEntry CreateInventoryNewItemEntry(int itemID, bool isWeaponIntactItem, int amount = 1)
    {
        return new InventoryNewItemEntry
        {
            itemID = itemID,
            amout = amount,
            isWeaponIntact = isWeaponIntactItem
        };
    }

    public void AddInventoryButtonsList(int indexButton, Button newButton) { inventoryButtonsList.Add(indexButton, newButton); }
    public void DeleteInventoryButtonList(int indexButton)
    {
        Dictionary<int, Button> tempInventoryButtonsList = new();

        int firstDeleteIndex = indexButton + 1;
        int inventoryCount  = inventoryButtonsList.Count;

        inventoryButtonsList.Remove(indexButton);

        for (int i = firstDeleteIndex; i < inventoryCount; i++)
        {
            tempInventoryButtonsList.Add(i - 1, inventoryButtonsList[i]); 
        }

        for(int i = inventoryCount - 1; i > indexButton; i--)
        {
            inventoryButtonsList.Remove(i);
        }

        for (int i = firstDeleteIndex; i < inventoryCount; i++)
        {
            int tempIndex = i - 1;
            inventoryButtonsList.Add(tempIndex, tempInventoryButtonsList[tempIndex]);
        }
    }
}