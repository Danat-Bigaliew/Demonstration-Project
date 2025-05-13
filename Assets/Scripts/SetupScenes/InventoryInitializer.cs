using UnityEngine;

public class InventoryInitializer : MonoBehaviour
{
    [SerializeField] private DataSaveController dataSaveController;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private InventoryButtonsController inventoryButtonsController;

    private void Start()
    {
        //dataSaveController.SetupDataSaveController();
        inventoryController.SetupInventoryController();
        inventoryButtonsController.SetupInventoryButtonsController();
        inventoryUIController.SetupInventoryUIController();
    }
}