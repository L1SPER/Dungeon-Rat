using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Object", menuName = "Inventory/Inventory Object")]
public class InventoryObject : ScriptableObject
{
    public int id;
    public string savePath;
    public string inventoryName;
    public Inventory inventory;
    public ItemDatabaseObject itemDatabase;
    //    public UserInterface type;
    public InventorySlot[] GetSlots=> inventory.InventorySlots;

    private void OnValidate()
    {
        if (inventory == null)
            return;

        inventory.SyncSlotsWithCapacity();
    }
}
