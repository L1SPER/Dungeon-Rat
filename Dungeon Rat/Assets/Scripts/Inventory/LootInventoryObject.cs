using UnityEngine;

[CreateAssetMenu(fileName = "Loot Inventory", menuName = "Inventory/Loot Inventory")]
public class LootInventoryObject : ScriptableObject
{
    public int id;
    public string savePath;
    public string inventoryName;
    public ItemDatabaseObject itemDatabase;
    public LootInventory inventory;

    public InventorySlot[] GetSlots => inventory != null ? inventory.Slots : null;

    private void OnValidate()
    {
        if (inventory == null)
            return;

        inventory.SyncSlotsWithCapacity();
    }
    [ContextMenu("CLEAR")]
    private void Clear()
    {
        if (inventory == null)
            return;
        inventory.Clear();
    }
}