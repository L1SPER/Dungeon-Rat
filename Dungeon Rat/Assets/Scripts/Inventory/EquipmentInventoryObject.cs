using UnityEngine;

[CreateAssetMenu(fileName = "Equipment Inventory", menuName = "Inventory/Equipment Inventory")]
public class EquipmentInventoryObject : ScriptableObject
{
    [System.NonSerialized] private Character ownerCharacter;

    public int id;
    public string savePath;
    public string inventoryName;
    public ItemDatabaseObject itemDatabase;
    public EquipmentInventory inventory;

    public InventorySlot[] GetSlots => inventory != null ? inventory.Slots : null;

    public void BindOwner(Character character)
    {
        ownerCharacter = character;

        if (inventory == null)
            return;

        inventory.BindOwnerObject(this);
        inventory.ConfigureDefaultRestrictions();
    }

    public void NotifyEquipmentInventoryChanged()
    {
        ownerCharacter?.RefreshEquipmentBonusStatsFromInventory();
    }

    private void OnEnable()
    {
        if (inventory == null)
            return;

        inventory.BindOwnerObject(this);
        inventory.ConfigureDefaultRestrictions();
    }

    private void OnValidate()
    {
        if (inventory == null)
            return;

        inventory.BindOwnerObject(this);
        inventory.ConfigureDefaultRestrictions();
    }
    [ContextMenu("CLEAR")]
    private void Clear()
    {
        if (inventory == null)
            return;
        inventory.Clear();
    }
}