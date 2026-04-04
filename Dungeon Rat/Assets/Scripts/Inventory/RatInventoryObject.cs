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

        inventory.ConfigureByLevel(Mathf.Max(1, inventory.Level));
    }
    [ContextMenu("CLEAR")]
    private void Clear()
    {
        if (inventory == null)
            return;
        inventory.Clear();
    }
}