using UnityEngine;

[CreateAssetMenu(fileName = "Equipment Inventory", menuName = "Inventory/Equipment Inventory")]
public class EquipmentInventoryObject : ScriptableObject
{
    public int id;
    public string savePath;
    public string inventoryName;
    public ItemDatabaseObject itemDatabase;
    public EquipmentInventory inventory;

    public InventorySlot[] GetSlots => inventory != null ? inventory.Slots : null;

    private void OnValidate()
    {
        if (inventory == null)
            return;

        inventory.ConfigureDefaultRestrictions();
    }
}