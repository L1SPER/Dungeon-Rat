using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
[System.Serializable]
public class ItemData : ScriptableObject
{
    [Header("General")]
    public string itemName;
    public int itemID;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Item Type")]
    public ItemType itemType;
    public EquipmentType equipmentType;

    [Header("Stacking")]
    public int maxStackSize;
    public bool isStackable => maxStackSize>1;
}
