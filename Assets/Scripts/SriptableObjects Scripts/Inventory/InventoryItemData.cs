using UnityEngine;

public enum ItemType { Weapon, Resource, Consumable }

[CreateAssetMenu(fileName = "InventoryItemData", menuName = "Scriptable Objects/InventoryItemData")]
public class InventoryItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private ItemType itemType;
    [SerializeField] private int maxItemStack;
    [SerializeField] private bool isWeaponIntact;

    public string ItemName => itemName;
    public Sprite ItemSprite => itemSprite;
    public ItemType ItemType => itemType;
    public int MaxItemStack => maxItemStack;
    public bool IsWeaponIntact => isWeaponIntact;
}
