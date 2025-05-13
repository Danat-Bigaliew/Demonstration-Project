using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    private InventoryUIModel model;
    private InventoryUIView view;

    private Transform inventoryUIContainer;
    private Transform inventoryContent;

    public void SetupInventoryUIController()
    {
        inventoryUIContainer = gameObject.transform;
        inventoryContent = transform.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Transform>();

        model = new InventoryUIModel(inventoryContent);
        view = new InventoryUIView(inventoryUIContainer, inventoryContent);

        model.SendUIData += SendInventorySceneUI;
        model.GetInventoryUIData();
    }

    private void SendInventorySceneUI(Dictionary<string, Dictionary<string, float>> inventoryUIData)
    {
        view.InventoryUIScene(inventoryUIData);
    }
}