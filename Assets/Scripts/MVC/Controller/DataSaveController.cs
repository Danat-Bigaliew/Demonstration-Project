using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

public class DataSaveController : MonoBehaviour
{
    [SerializeField] private string fileName = "Test.txt";// Имя файла в корне проекта
    private string projectRootPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));// Путь к корню проекта - Assets

    public string filePath { get; private set; }
    public DataSaveModel model { get; private set; }

    [Inject] private InventoryController inventoryController;
    private void Construct(InventoryController InventoryController) { inventoryController = InventoryController; }

    public void SearchInventoryDataFile()
    {
        filePath = Path.Combine(projectRootPath, fileName);

        model = new DataSaveModel(filePath);
    }

    private void OnApplicationQuit()
    {
        Dictionary<int, Dictionary<string, object>> savedData = model.GetInventoryItemsData();

        if (savedData != model.inventoryData)
        {
            Debug.Log("Игра закрывается. Сохраняем данные");

            RecordingInventroyData();
        }
        else
        {
            Debug.Log("Игра закрывается. Сохранять данные не нужно");
        }
    }

    public bool SearchForJSONFile() { return model.SearchJsonFile(); }
    public Dictionary<int, Dictionary<string, object>> GetInventoryData() { return model.inventoryData; }
    public void RecordingInventroyData() { model.RecordingAllInventoryData(); }
    public void SwapInventoryItem(int targetItemIndex, int draggedItemIndex) { model.SwapInventoryItem(targetItemIndex, draggedItemIndex); }
    public Dictionary<int, Dictionary<string, object>> GetInventoryDataDictionary() { return model.inventoryData; }
    public Dictionary<string, object> GetInventoryItem(int itemIndex) { return model.GetInventoryItem(itemIndex); }
    public int ResizeAmountInventoryData(int itemIndex, int newAmount) { return model.AddOrDeleteItemAmount(itemIndex, newAmount); }
    public void AddItemForInventoryData(int indexKey, Dictionary<string, object> itemValue) { model.AddInventoryDataItem(indexKey, itemValue); }
    public void RemoveInventoryItem(int deletedItemIndexPosition) { model.RemoveInventoryItem(deletedItemIndexPosition); }
    public bool ChangeItemWeaponState(int itemIndex) { return model.ChangeInventoryItemState(itemIndex); }
}