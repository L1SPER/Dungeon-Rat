using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class InventorySlot
{
    public Item item = new Item();
    public int slotID;
    public bool isOverflowSlot;

    [System.NonSerialized] private InventoryBase ownerInventory;
    public InventoryBase OwnerInventory => ownerInventory;

    [Header("Restrictions")]
    [SerializeField] private EquipmentType allowedEquipmentType = EquipmentType.None;
    public EquipmentType AllowedEquipmentType => allowedEquipmentType;

    public InventorySlot()
    {
        item = new Item();
        slotID = -1;
        isOverflowSlot = false;
        allowedEquipmentType = EquipmentType.None;
    }

    public void SetOwnerInventory(InventoryBase owner)
    {
        ownerInventory = owner;
    }

    public bool IsEmpty()
    {
        return item == null || item.IsEmpty();
    }

    public void SetItem(ItemData itemData, int amount)
    {
        item = new Item(itemData, amount);
    }

    public void SetItem(Item newItem)
    {
        item = newItem == null ? new Item() : new Item(newItem);
    }

    public void AddAmount(int amount)
    {
        if (IsEmpty() || amount <= 0)
            return;

        item.amount += amount;
    }

    public void RemoveAmount(int amount)
    {
        if (IsEmpty() || amount <= 0)
            return;

        item.amount -= amount;

        if (item.amount <= 0)
            ClearSlot();
    }

    public void ClearSlot()
    {
        item = new Item();
    }

    public bool CanStackWith(ItemData itemData)
    {
        if (itemData == null)
            return false;

        if (IsEmpty())
            return false;

        if (!item.IsStackable())
            return false;

        return item.itemData == itemData;
    }

    public int RemainingStackSpace()
    {
        if (IsEmpty() || !item.IsStackable())
            return 0;

        return Mathf.Max(0, item.itemData.maxStackSize - item.amount);
    }

    public bool CanPlaceItem(ItemData itemData, bool allowOverflowSlot = false)
    {
        if (itemData == null)
            return false;

        if (isOverflowSlot && !allowOverflowSlot)
            return false;

        if (allowedEquipmentType == EquipmentType.None)
            return true;

        EquipmentItemData equipmentItem = itemData as EquipmentItemData;
        if (equipmentItem == null)
            return false;

        return equipmentItem.equipmentType == allowedEquipmentType;
    }

    public void SetAllowedEquipmentType(EquipmentType equipmentType)
    {
        allowedEquipmentType = equipmentType;
    }
}