public static class InventoryTransferUtility
{
    public static bool MoveOrMergeOrSwap(InventorySlot fromSlot, InventorySlot toSlot)
    {
        if (fromSlot == null || toSlot == null)
            return false;

        if (fromSlot.IsEmpty())
            return false;

        ItemData movingItemData = fromSlot.item.itemData;

        if (!CanSlotAcceptItem(toSlot, movingItemData))
            return false;

        if (toSlot.IsEmpty())
        {
            toSlot.SetItem(fromSlot.item);
            fromSlot.ClearSlot();
            NotifyChangedInventories(fromSlot, toSlot);
            return true;
        }

        if (toSlot.CanStackWith(movingItemData))
        {
            int space = toSlot.RemainingStackSpace();
            if (space > 0)
            {
                int moveAmount = System.Math.Min(space, fromSlot.item.amount);
                toSlot.AddAmount(moveAmount);
                fromSlot.RemoveAmount(moveAmount);
                NotifyChangedInventories(fromSlot, toSlot);
                return true;
            }
        }

        Item targetItemCopy = new Item(toSlot.item);

        if (!CanSlotAcceptItem(fromSlot, targetItemCopy.itemData))
            return false;

        toSlot.SetItem(fromSlot.item);
        fromSlot.SetItem(targetItemCopy);
        NotifyChangedInventories(fromSlot, toSlot);
        return true;
    }

    public static bool MoveItemToFirstValidSlot(InventorySlot fromSlot, InventoryBase targetInventory)
    {
        if (fromSlot == null || fromSlot.IsEmpty() || targetInventory == null)
            return false;

        ItemData movingItemData = fromSlot.item.itemData;

        if (movingItemData.IsStackable)
        {
            for (int i = 0; i < targetInventory.Slots.Length; i++)
            {
                InventorySlot targetSlot = targetInventory.Slots[i];
                if (targetSlot == null)
                    continue;

                if (!targetSlot.CanStackWith(movingItemData))
                    continue;

                return MoveOrMergeOrSwap(fromSlot, targetSlot);
            }
        }

        for (int i = 0; i < targetInventory.Slots.Length; i++)
        {
            InventorySlot targetSlot = targetInventory.Slots[i];
            if (targetSlot == null)
                continue;

            if (!targetSlot.IsEmpty())
                continue;

            if (!targetSlot.CanPlaceItem(movingItemData, targetSlot.isOverflowSlot))
                continue;

            return MoveOrMergeOrSwap(fromSlot, targetSlot);
        }

        return false;
    }

    public static bool CanSlotAcceptItem(InventorySlot slot, ItemData itemData)
    {
        if (slot == null)
            return false;

        if (itemData == null)
            return true;

        return slot.CanPlaceItem(itemData, slot.isOverflowSlot);
    }

    private static void NotifyChangedInventories(InventorySlot fromSlot, InventorySlot toSlot)
    {
        InventoryBase fromInventory = fromSlot != null ? fromSlot.OwnerInventory : null;
        InventoryBase toInventory = toSlot != null ? toSlot.OwnerInventory : null;

        if (fromInventory != null)
            fromInventory.NotifyInventoryChanged();

        if (toInventory != null && toInventory != fromInventory)
            toInventory.NotifyInventoryChanged();
    }
}