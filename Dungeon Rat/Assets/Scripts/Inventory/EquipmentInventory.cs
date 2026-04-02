using UnityEngine;

[System.Serializable]
public class EquipmentInventory : InventoryBase
{
    [SerializeField] private int slotCount = 8;

    [System.NonSerialized] private EquipmentInventoryObject ownerObject;

    public void ConfigureEquipmentInventory()
    {
        if (slotCount < 1)
            slotCount = 8;

        if (slots == null || slots.Length != slotCount)
        {
            InventorySlot[] newSlots = new InventorySlot[slotCount];

            for (int i = 0; i < slotCount; i++)
            {
                if (slots != null && i < slots.Length && slots[i] != null)
                    newSlots[i] = slots[i];
                else
                    newSlots[i] = new InventorySlot();

                newSlots[i].slotID = i;
                newSlots[i].isOverflowSlot = false;
                newSlots[i].SetOwnerInventory(this);
            }

            slots = newSlots;
        }
        else
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                    slots[i] = new InventorySlot();

                slots[i].slotID = i;
                slots[i].isOverflowSlot = false;
                slots[i].SetOwnerInventory(this);
            }
        }
    }

    public void BindOwnerObject(EquipmentInventoryObject equipmentInventoryObject)
    {
        ownerObject = equipmentInventoryObject;
    }

    public override void NotifyInventoryChanged()
    {
        ownerObject?.NotifyEquipmentInventoryChanged();
    }

    public void ConfigureDefaultRestrictions()
    {
        ConfigureEquipmentInventory();

        if (slots.Length < 8)
            return;

        SetSlotRestriction(0, EquipmentType.Helmet);
        SetSlotRestriction(1, EquipmentType.Amulet);
        SetSlotRestriction(2, EquipmentType.Chest);
        SetSlotRestriction(3, EquipmentType.Glove);
        SetSlotRestriction(4, EquipmentType.Trousers);
        SetSlotRestriction(5, EquipmentType.Ring);
        SetSlotRestriction(6, EquipmentType.Shoe);
        SetSlotRestriction(7, EquipmentType.Weapon);
    }

    public void SetSlotRestriction(int slotIndex, EquipmentType equipmentType)
    {
        if (!IsValidIndex(slotIndex))
            return;

        slots[slotIndex].SetAllowedEquipmentType(equipmentType);
    }

    public EquipmentType GetSlotRestriction(int slotIndex)
    {
        if (!IsValidIndex(slotIndex))
            return EquipmentType.None;

        return slots[slotIndex].AllowedEquipmentType;
    }

    public int FindSlotIndexByEquipmentType(EquipmentType equipmentType)
    {
        if (slots == null)
            return -1;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            if (slots[i].AllowedEquipmentType == equipmentType)
                return i;
        }

        return -1;
    }

    public int FindFirstValidSlotForItem(ItemData itemData)
    {
        if (itemData == null || slots == null)
            return -1;

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];
            if (slot == null)
                continue;

            if (!slot.CanPlaceItem(itemData, false))
                continue;

            return i;
        }

        return -1;
    }

    public bool CanEquip(ItemData itemData, int slotIndex)
    {
        if (!IsValidIndex(slotIndex) || itemData == null)
            return false;

        InventorySlot targetSlot = slots[slotIndex];
        if (targetSlot == null)
            return false;

        return targetSlot.CanPlaceItem(itemData, false);
    }

    public bool TrySetItemToSlot(int slotIndex, ItemData itemData, int amount = 1)
    {
        if (!IsValidIndex(slotIndex))
            return false;

        if (itemData == null || amount <= 0)
            return false;

        InventorySlot slot = slots[slotIndex];
        if (slot == null)
            return false;

        if (!slot.CanPlaceItem(itemData, false))
            return false;

        if (itemData.IsStackable)
            amount = Mathf.Min(amount, itemData.maxStackSize);
        else
            amount = 1;

        slot.SetItem(itemData, amount);
        NotifySlotChanged(slotIndex);
        return true;
    }

    public bool EquipFromInventory(ContainerInventoryBase sourceInventory, int sourceIndex, int targetEquipmentSlotIndex)
    {
        if (sourceInventory == null)
            return false;

        if (!sourceInventory.IsValidIndex(sourceIndex) || !IsValidIndex(targetEquipmentSlotIndex))
            return false;

        InventorySlot sourceSlot = sourceInventory.GetSlot(sourceIndex);
        InventorySlot targetSlot = GetSlot(targetEquipmentSlotIndex);

        return InventoryTransferUtility.MoveOrMergeOrSwap(sourceSlot, targetSlot);
    }

    public bool EquipFromInventoryAuto(ContainerInventoryBase sourceInventory, int sourceIndex)
    {
        if (sourceInventory == null || !sourceInventory.IsValidIndex(sourceIndex))
            return false;

        InventorySlot sourceSlot = sourceInventory.GetSlot(sourceIndex);
        if (sourceSlot == null || sourceSlot.IsEmpty())
            return false;

        int slotIndex = FindFirstValidSlotForItem(sourceSlot.item.itemData);
        if (slotIndex == -1)
            return false;

        return EquipFromInventory(sourceInventory, sourceIndex, slotIndex);
    }

    public bool UnequipToInventory(int equipmentSlotIndex, ContainerInventoryBase targetInventory)
    {
        if (!IsValidIndex(equipmentSlotIndex) || targetInventory == null)
            return false;

        InventorySlot equipmentSlot = GetSlot(equipmentSlotIndex);
        if (equipmentSlot == null || equipmentSlot.IsEmpty())
            return false;

        return InventoryTransferUtility.MoveItemToFirstValidSlot(equipmentSlot, targetInventory);
    }

    public bool MoveOrSwapEquipmentSlots(int fromIndex, int toIndex)
    {
        if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex))
            return false;

        if (fromIndex == toIndex)
            return false;

        InventorySlot fromSlot = slots[fromIndex];
        InventorySlot toSlot = slots[toIndex];

        return InventoryTransferUtility.MoveOrMergeOrSwap(fromSlot, toSlot);
    }

    public EquipmentItemData GetEquippedItem(int slotIndex)
    {
        if (!IsValidIndex(slotIndex))
            return null;

        InventorySlot slot = slots[slotIndex];
        if (slot == null || slot.IsEmpty())
            return null;

        return slot.item.itemData as EquipmentItemData;
    }

    public bool IsSlotEmpty(int slotIndex)
    {
        if (!IsValidIndex(slotIndex))
            return true;

        return slots[slotIndex] == null || slots[slotIndex].IsEmpty();
    }
}