using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InventoryView
{
    public void SetInventoryItemUI
        (InventoryNewItemEntry tempItemData, Dictionary<int, InventoryItemData> iventoryDatabaseList, GameObject newItem, string itemName)
    {
        Image newItemSprite = newItem.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI newItemText = newItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        newItem.name = $"{itemName} - {tempItemData.itemID}";
        newItemSprite.sprite = iventoryDatabaseList[tempItemData.itemID].ItemSprite;
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

    public void NewButtonPosition(RectTransform button, Vector2 newButtonPosition)
    {
        button.anchoredPosition = newButtonPosition;
    }

    public void ChangeAlphaChannel(Transform item, float alfaValue)
    {
        Image childImage = item.GetChild(0).GetComponent<Image>();
        Color alfa = childImage.color;

        alfa.a = alfaValue;
        childImage.color = alfa;

        Debug.Log($"Item Color Alpha : {alfaValue}");
    }

    public IEnumerator AnimationInventoryItem(Dictionary<string, object> dataForDOTAnimation)
    {
        RectTransform draggedItem = (RectTransform)dataForDOTAnimation["draggedItem"];
        RectTransform targetItem = (RectTransform)dataForDOTAnimation["targetItem"];

        Vector2 defaultDraggedItemPosition = (Vector2)dataForDOTAnimation["defaultDraggedItemPosition"];

        float animationDuration = (float)dataForDOTAnimation["animationDuration"];
        int localIndexDraggedItem = (int)dataForDOTAnimation["localIndexDraggedItem"];
        int localIndexTargetItem = (int)dataForDOTAnimation["localIndexTargetItem"];

        Sequence moveSequence = DOTween.Sequence();//Для двух одновременных анимаций
        moveSequence.Join(draggedItem.DOAnchorPos(targetItem.anchoredPosition, animationDuration).SetEase(Ease.Linear));
        moveSequence.Join(targetItem.DOAnchorPos(defaultDraggedItemPosition, animationDuration).SetEase(Ease.Linear));

        yield return moveSequence.WaitForCompletion();//Ждет фактического завершения анимаций - moveSequence

        draggedItem.transform.SetSiblingIndex(localIndexDraggedItem);
        targetItem.transform.SetSiblingIndex(localIndexTargetItem);
    }

    public void UpdateInventoryItemAmountUI(Transform item, int newItemAmount)
    {
        TextMeshProUGUI itemText = item.GetChild(1).GetComponent<TextMeshProUGUI>();

        itemText.text = $"{newItemAmount}";
    }
    public void DeleteInventoryItem(Transform content, Transform item)
    {
        GameObject.Destroy(item.gameObject);
    }
}