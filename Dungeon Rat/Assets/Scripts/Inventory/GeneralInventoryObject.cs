using UnityEngine;

[CreateAssetMenu(fileName = "General Inventory", menuName = "Inventory/General Inventory")]
public class GeneralInventoryObject : ScriptableObject
{
    public int id;
    public string savePath;
    public string inventoryName;
    public ItemDatabaseObject itemDatabase;
    public GeneralInventory inventory;

    public InventorySlot[] GetSlots => inventory != null ? inventory.Slots : null;

    private void OnValidate()
    {
        if (inventory == null)
            return;

        inventory.RefreshCapacityByCurrentLevel();
    }
    [ContextMenu("CLEAR")]
    private void Clear()
    {
        if (inventory == null)
            return;
        inventory.Clear();
    }
}