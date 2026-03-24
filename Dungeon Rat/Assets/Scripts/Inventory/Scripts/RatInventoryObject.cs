using UnityEngine;

[CreateAssetMenu(fileName = "Rat Inventory", menuName = "Inventory/Rat Inventory")]
public class RatInventoryObject : ScriptableObject
{
    public int id;
    public string savePath;
    public string inventoryName;
    public ItemDatabaseObject itemDatabase;
    public RatInventory inventory;

    public InventorySlot[] GetSlots => inventory != null ? inventory.Slots : null;

    private void OnValidate()
    {
        if (inventory == null)
            return;

        inventory.SyncSlotsWithCapacity();
    }
}