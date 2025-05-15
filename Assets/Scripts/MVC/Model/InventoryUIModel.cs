using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIModel
{
    private Transform inventoryContent;

    public Dictionary<string, Dictionary<string, float>> inventoryUIData { get; private set; } = new();

    public event Action<Dictionary<string, Dictionary<string, float>>> SendUIData;

    public InventoryUIModel(Transform inventroyContent)
    {
        this.inventoryContent = inventroyContent;
    }

    public void GetInventoryUIData()
    {
        Vector2 currentSize = new Vector2(Screen.width, Screen.height);

        inventoryUIData.Add("DataGameContainer", DataGameContainer(currentSize));
        inventoryUIData.Add("DataInventoryContainer", DataInventoryContainer(currentSize));
        inventoryUIData.Add("DataInventoryItem", 
            DataInventoryItem(currentSize, inventoryUIData["DataInventoryContainer"]["inventoryContainerWidth"]));

        SendUIData(inventoryUIData);
    }

    private Dictionary<string, float> DataGameContainer(Vector2 currentSize)
    {
        float backgroundPadding = currentSize.x * 0.07f;
        float UIButtonsContainerSize = currentSize.y * 0.161f;
        float OnOffInventoryButtonSize = UIButtonsContainerSize * 0.67f;

        Dictionary<string, float> gameCanvas = new()
        {
            { "backgroundPadding", backgroundPadding },
            { "UIButtonsContainerSize", UIButtonsContainerSize },
            { "OnOffInventoryButtonSize", OnOffInventoryButtonSize }
        };

        return gameCanvas;
    }
    private Dictionary<string, float> DataInventoryContainer(Vector2 currentSize)
    {
        float messageForPlayerHorizontalPadding = currentSize.x * 0.234f;
        float messageForPlayerHeight = currentSize.y * 0.075f;
        float messageForPlayerPoxY = messageForPlayerHorizontalPadding / 2f;

        float inventoryContainerWidth = currentSize.x * 0.721f;
        float inventoryContainerHeight = currentSize.y * 0.57085f;
        float inventoryContainerPosX = (currentSize.x - inventoryContainerWidth) / 2f;
        float inventoryContainerPosY = -(messageForPlayerPoxY + (messageForPlayerHeight * 2f));

        float inventoryButtonsHeight = inventoryContainerHeight * 0.1128f;
        float inventoryButtonsPosY = Mathf.Abs(inventoryContainerPosY) * 0.0158f;

        float scrollViewHorizontalPadding = inventoryContainerWidth * 0.052f / 2f;
        float scrollViewVerticalPadding = (inventoryContainerHeight * 0.0095f) * 2f;
        float scrollViewWidth = inventoryContainerWidth - (scrollViewHorizontalPadding * 2f);
        float scrollViewHeight = inventoryContainerHeight - (inventoryButtonsPosY + inventoryButtonsHeight) - scrollViewVerticalPadding;
        float scrollViewPosY = inventoryButtonsHeight + (scrollViewVerticalPadding / 2f);

        Dictionary<string, float> inventoryData = new()
        {
            { "messageForPlayerHorizontalPadding", messageForPlayerHorizontalPadding },
            { "messageForPlayerHeight", messageForPlayerHeight },
            { "messageForPlayerPoxY", messageForPlayerPoxY },
            { "inventoryContainerWidth", inventoryContainerWidth },
            { "inventoryContainerHeight", inventoryContainerHeight },
            { "inventoryContainerPosX", inventoryContainerPosX },
            { "inventoryContainerPosY", inventoryContainerPosY },
            { "inventoryButtonsHeight", inventoryButtonsHeight },
            { "inventoryButtonsPosY", inventoryButtonsPosY },
            { "scrollViewHorizontalPadding", scrollViewHorizontalPadding },
            { "scrollViewVerticalPadding", scrollViewVerticalPadding },
            { "scrollViewWidth", scrollViewWidth},
            { "scrollViewHeight", scrollViewHeight },
            { "scrollViewPosY", scrollViewPosY }
        };

        return inventoryData;
    }
    private Dictionary<string, float> DataInventoryItem(Vector2 currentSize, float inventoryContainerWidth)
    {
        GridLayoutGroup gridLayoutGroup = inventoryContent.GetComponent<GridLayoutGroup>();

        float itemPadding = currentSize.x * 0.018f;
        float itemPaddingForSize = currentSize.x * 0.0242f;
        float itemSize = (inventoryContainerWidth - (itemPaddingForSize * gridLayoutGroup.constraintCount)) / gridLayoutGroup.constraintCount;
        float itemImageSize = itemSize * 0.8f;
        float itemImagePosX = (itemSize - itemImageSize) / 2f;
        float itemImagePosY = (itemImagePosX * 2f) / 4f;
        float itemImageTextHeight = itemSize * 0.2f;
        float itemImageTextSize = itemImageTextHeight * 1.08f;

        Dictionary<string, float> inventoryItemData = new()
        {
            { "itemPadding", itemPadding },
            { "itemSize", itemSize },
            { "itemImageSize", itemImageSize },
            { "itemImagePosX", itemImagePosX },
            { "itemImagePosY", itemImagePosY },
            { "itemImageTextHeight", itemImageTextHeight },
            { "itemImageTextSize", itemImageTextSize }
        };

        return inventoryItemData;
    }
}