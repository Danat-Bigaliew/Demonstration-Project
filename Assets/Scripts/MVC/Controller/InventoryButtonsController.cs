using UnityEngine;
using Zenject;

public class InventoryButtonsController : MonoBehaviour
{
    [SerializeField] private Transform inventoryAdministrationButtonsContainer;
    [SerializeField] private InventoryDatabase inventoryDatabase;

    [Inject] private InventoryController inventoryController;
    [Inject] private DataSaveController dataSaveController;
    private void Construct(InventoryController InventoryController, DataSaveController DataSaveController) 
    { 
        inventoryController = InventoryController;
        dataSaveController = DataSaveController;
    }

    private InventoryButtonsModel model;
    private InventoryButtonsView view;

    public void SetupInventoryButtonsController()
    {
        model = new InventoryButtonsModel(inventoryController, dataSaveController);
        view = new InventoryButtonsView();

        SetButtonsListener();
    }

    private void SetButtonsListener()
    {
        model.SetInventoryButtonsListener();
        model.SetInventoryAdministrationButtonsListener(inventoryAdministrationButtonsContainer);
    }

    public void UpdateInventoryItemsListener()
    {
        model.SetInventoryButtonsListener();
    }
}