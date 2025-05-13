using UnityEngine;
using Zenject;

public class InventoryInstaller : MonoInstaller
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private InventoryButtonsController inventoryButtonsController;
    private DataSaveController dataSaveController;
    

    public override void InstallBindings()
    {
        dataSaveController = GetComponent<DataSaveController>();

        Container.Bind<InventoryController>().FromInstance(inventoryController).AsSingle();
        Container.Bind<DataSaveController>().FromInstance(dataSaveController).AsSingle();
        Container.Bind<InventoryButtonsController>().FromInstance(inventoryButtonsController).AsSingle();
    }
}