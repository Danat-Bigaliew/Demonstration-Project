using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

public class DataSaveController : MonoBehaviour
{
    [SerializeField] private string fileName = "Test.txt";// ��� ����� � ����� �������

    public string filePath { get; private set; }
    public DataSaveModel model { get; private set; }

    private string projectRootPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));// ���� � ����� ������� - Assets

    [Inject] private InventoryController inventoryController;
    private void Construct(InventoryController InventoryController) { inventoryController = InventoryController; }

    public void SearchInventoryDataFile()
    {
        filePath = Path.Combine(projectRootPath, fileName);

        model = new DataSaveModel(filePath);
    }

    public void RemoveInventoryItem(int deletedItemIndexPosition)
    {
        model.RemoveInventoryItem(deletedItemIndexPosition);
    }

    private void OnApplicationQuit()
    {
        Dictionary<int, Dictionary<string, object>> savedData = model.GetInventoryItemsData();

        if (savedData != model.inventoryData)
        {
            Debug.Log("���� �����������. ��������� ������");

            model.RecordingAllInventoryData(model.inventoryData);
        }
        else
        {
            Debug.Log("���� �����������. ��������� ������ �� �����");
        }
    }
}