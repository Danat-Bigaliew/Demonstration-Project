using NUnit.Framework.Constraints;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIView
{
    private Transform inventoryUIContainer;
    private Transform inventoryContent;

    public InventoryUIView(Transform inventoryUIContainer, Transform inventoryContent)
    {
        this.inventoryUIContainer = inventoryUIContainer;
        this.inventoryContent = inventoryContent;
    }

    public void InventoryUIScene(Dictionary<string, Dictionary<string, float>> inventoryUIData)
    {
        GameContainerUI(inventoryUIData);
        InventoryContainerUI(inventoryUIData);
        InventoryItems(inventoryUIData);
    }

    private void GameContainerUI(Dictionary<string, Dictionary<string, float>> inventoryUIData)
    {
        RectTransform inventoryBackground = inventoryUIContainer.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        RectTransform UIButtonsContainer = inventoryUIContainer.GetChild(0).GetChild(1).GetComponent<RectTransform>();
        RectTransform OnOffInventoryButton = UIButtonsContainer.transform.GetChild(0).GetComponent<RectTransform>();
        
        float OnOffInventoryButtonSize = inventoryUIData["DataGameContainer"]["OnOffInventoryButtonSize"];

        inventoryBackground.sizeDelta = new Vector2(
            inventoryUIData["DataGameContainer"]["backgroundPadding"],inventoryBackground.sizeDelta.y);
        UIButtonsContainer.sizeDelta = new Vector2(
            UIButtonsContainer.sizeDelta.x, inventoryUIData["DataGameContainer"]["UIButtonsContainerSize"]);
        OnOffInventoryButton.sizeDelta = new Vector2(
            inventoryUIData["DataGameContainer"]["OnOffInventoryButtonSize"], inventoryUIData["DataGameContainer"]["OnOffInventoryButtonSize"]);
    }

    private void InventoryContainerUI(Dictionary<string, Dictionary<string, float>> inventoryUIData)
    {
        Transform inventory = inventoryUIContainer.GetChild(1).transform;
        RectTransform messageForPlayer = inventory.GetChild(0).GetComponent<RectTransform>();
        RectTransform inventoryContainer = inventory.GetChild(1).GetComponent<RectTransform>();
        RectTransform inventoryButtons = inventory.GetChild(1).GetChild(0).GetComponent<RectTransform>();
        RectTransform scrollView = inventory.GetChild(1).GetChild(1).GetComponent<RectTransform>();
        GridLayoutGroup gridLayoutGroup = inventoryContent.GetComponent<GridLayoutGroup>();

        messageForPlayer.sizeDelta = new Vector2(inventoryUIData["DataInventoryContainer"]["messageForPlayerHorizontalPadding"], 
            inventoryUIData["DataInventoryContainer"]["messageForPlayerHeight"]);
        messageForPlayer.anchoredPosition = new Vector2(messageForPlayer.anchoredPosition.x,
            inventoryUIData["DataInventoryContainer"]["messageForPlayerPoxY"]);
        inventoryContainer.sizeDelta = new Vector2(inventoryUIData["DataInventoryContainer"]["inventoryContainerWidth"],
            inventoryUIData["DataInventoryContainer"]["inventoryContainerHeight"]);
        inventoryContainer.anchoredPosition = new Vector2(inventoryUIData["DataInventoryContainer"]["inventoryContainerPosX"],
            inventoryUIData["DataInventoryContainer"]["inventoryContainerPosY"]);
        inventoryButtons.sizeDelta = new Vector2(inventoryButtons.sizeDelta.x,
            inventoryUIData["DataInventoryContainer"]["inventoryButtonsHeight"]);
        inventoryButtons.anchoredPosition = new Vector2(inventoryButtons.anchoredPosition.x, 
            inventoryUIData["DataInventoryContainer"]["inventoryButtonsPosY"]);
        scrollView.sizeDelta = new Vector2(inventoryUIData["DataInventoryContainer"]["scrollViewWidth"],
            inventoryUIData["DataInventoryContainer"]["scrollViewHeight"]);
        scrollView.anchoredPosition = new Vector2(inventoryUIData["DataInventoryContainer"]["scrollViewHorizontalPadding"], 
            -inventoryUIData["DataInventoryContainer"]["scrollViewPosY"]);
        gridLayoutGroup.spacing = new Vector2(inventoryUIData["DataInventoryItem"]["itemPadding"],
            inventoryUIData["DataInventoryItem"]["itemPadding"]);
        gridLayoutGroup.cellSize = new Vector2(inventoryUIData["DataInventoryItem"]["itemSize"], 
            inventoryUIData["DataInventoryItem"]["itemSize"]);
    }

    private void InventoryItems(Dictionary<string, Dictionary<string, float>> inventoryUIData)
    {
        foreach(Transform itemChild in inventoryContent)
        {
            RectTransform itemImage = itemChild.GetChild(0).GetComponent<RectTransform>();
            RectTransform itemTextRect = itemChild.GetChild(1).GetComponent<RectTransform>();
            TextMeshProUGUI itemText = itemTextRect.GetComponent<TextMeshProUGUI>();

            itemImage.sizeDelta = new Vector2(inventoryUIData["DataInventoryItem"]["itemImageSize"], 
                inventoryUIData["DataInventoryItem"]["itemImageSize"]);
            itemImage.anchoredPosition = new Vector2(inventoryUIData["DataInventoryItem"]["itemImagePosX"], 
                -inventoryUIData["DataInventoryItem"]["itemImagePosY"]);
            itemTextRect.sizeDelta = new Vector2(itemTextRect.sizeDelta.x, inventoryUIData["DataInventoryItem"]["itemImageTextHeight"]);
            itemText.fontSize = inventoryUIData["DataInventoryItem"]["itemImageTextSize"];
        }
    }
}