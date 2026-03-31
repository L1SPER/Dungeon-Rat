using UnityEngine;

[System.Serializable]
public abstract class ContainerInventoryBase : InventoryBase
{
    [Header("Capacity")]
    [SerializeField] protected int currentCapacity;
    [SerializeField] protected int maxCapacity;
    [SerializeField] protected bool useOverflowSlots;

    public int CurrentCapacity => currentCapacity;
    public int MaxCapacity => maxCapacity;
    public bool UseOverflowSlots => useOverflowSlots;

    public virtual void Configure(int currentCapacity, int maxCapacity, bool useOverflowSlots)
    {
        this.currentCapacity = Mathf.Max(0, currentCapacity);
        this.maxCapacity = Mathf.Max(this.currentCapacity, maxCapacity);
        this.useOverflowSlots = useOverflowSlots;

        SyncSlotsWithCapacity();
    }

    public virtual void SyncSlotsWithCapacity()
    {
        if (currentCapacity < 0)
            currentCapacity = 0;

        if (maxCapacity < 0)
            maxCapacity = 0;

        if (currentCapacity > maxCapacity)
            currentCapacity = maxCapacity;

        if (slots == null || slots.Length != maxCapacity)
        {
            InventorySlot[] newSlots = new InventorySlot[maxCapacity];

            for (int i = 0; i < maxCapacity; i++)
            {
                if (slots != null && i < slots.Length && slots[i] != null)
                    newSlots[i] = slots[i];
                else
                    newSlots[i] = new InventorySlot();

                newSlots[i].slotID = i;
                newSlots[i].isOverflowSlot = useOverflowSlots && i >= currentCapacity;
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
                slots[i].isOverflowSlot = useOverflowSlots && i >= currentCapacity;
            }
        }
    }

    public virtual int AddItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null || amount <= 0 || slots == null || slots.Length == 0)
            return amount;

        int remaining = amount;

        if (itemData.IsStackable)
        {
            remaining = TryAddToStacks(itemData, remaining, false);
            remaining = TryAddToEmptySlots(itemData, remaining, false);

            if (useOverflowSlots && remaining > 0)
            {
                remaining = TryAddToStacks(itemData, remaining, true);
                remaining = TryAddToEmptySlots(itemData, remaining, true);
            }
        }
        else
        {
            remaining = TryAddToEmptySlots(itemData, remaining, false);

            if (useOverflowSlots && remaining > 0)
            {
                remaining = TryAddToEmptySlots(itemData, remaining, true);
            }
        }

        return remaining;
    }

    protected int TryAddToStacks(ItemData itemData, int remaining, bool overflowSlots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];  

            if (slot == null || slot.isOverflowSlot != overflowSlots)
                continue;

            if (!slot.CanStackWith(itemData))
                continue;

            int space = slot.RemainingStackSpace();
            if (space <= 0)
                continue;

            int addAmount = Mathf.Min(space, remaining);
            slot.AddAmount(addAmount);
            remaining -= addAmount;

            if (remaining <= 0)
                break;
        }

        return remaining;
    }

    protected int TryAddToEmptySlots(ItemData itemData, int remaining, bool overflowSlots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];

            if (slot == null || slot.isOverflowSlot != overflowSlots)
                continue;

            if (!slot.IsEmpty())
                continue;

            if (!slot.CanPlaceItem(itemData, overflowSlots))
                continue;

            int addAmount = itemData.IsStackable
                ? Mathf.Min(itemData.maxStackSize, remaining)
                : 1;

            slot.SetItem(itemData, addAmount);
            remaining -= addAmount;

            if (remaining <= 0)
                break;
        }

        return remaining;
    }

    public virtual bool SplitSlot(int fromIndex, int toIndex, int splitAmount)
    {
        if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex))
            return false;

        if (fromIndex == toIndex || splitAmount <= 0)
            return false;

        InventorySlot fromSlot = slots[fromIndex];
        InventorySlot toSlot = slots[toIndex];

        if (fromSlot == null || toSlot == null)
            return false;

        if (fromSlot.IsEmpty() || !fromSlot.item.IsStackable())
            return false;

        if (fromSlot.item.amount <= splitAmount)
            return false;

        if (!toSlot.IsEmpty())
            return false;

        if (!toSlot.CanPlaceItem(fromSlot.item.itemData, toSlot.isOverflowSlot))
            return false;

        toSlot.SetItem(fromSlot.item.itemData, splitAmount);
        fromSlot.RemoveAmount(splitAmount);
        return true;
    }

    public virtual bool SplitSlotInHalfToFirstValidEmptySlot(int fromIndex)
    {
        if (!IsValidIndex(fromIndex))
            return false;

        InventorySlot fromSlot = slots[fromIndex];
        if (fromSlot == null || fromSlot.IsEmpty())
            return false;

        if (!fromSlot.item.IsStackable())
            return false;

        if (fromSlot.item.amount < 2)
            return false;

        int splitAmount = fromSlot.item.amount / 2;
        if (splitAmount <= 0)
            return false;

        int targetIndex = FindFirstValidEmptySlotForSplit(fromSlot.item.itemData);
        if (targetIndex == -1)
            return false;

        return SplitSlot(fromIndex, targetIndex, splitAmount);
    }

    protected int FindFirstValidEmptySlotForSplit(ItemData itemData)
    {
        if (slots == null || itemData == null)
            return -1;

        // Önce normal slotlar
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];
            if (slot == null)
                continue;

            if (slot.isOverflowSlot)
                continue;

            if (!slot.IsEmpty())
                continue;

            if (!slot.CanPlaceItem(itemData, false))
                continue;

            return i;
        }

        // Eğer bu inventory overflow destekliyorsa sonra overflow slotlara bak
        if (useOverflowSlots)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                InventorySlot slot = slots[i];
                if (slot == null)
                    continue;

                if (!slot.isOverflowSlot)
                    continue;

                if (!slot.IsEmpty())
                    continue;

                if (!slot.CanPlaceItem(itemData, true))
                    continue;

                return i;
            }
        }

        return -1;
    }

    public virtual bool MoveOrMergeOrSwap(int fromIndex, int toIndex)
    {
        if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex))
            return false;

        if (fromIndex == toIndex)
            return false;

        return InventoryTransferUtility.MoveOrMergeOrSwap(slots[fromIndex], slots[toIndex]);
    }

    public virtual bool MoveItemToInventory(int fromIndex, InventoryBase targetInventory, int toIndex)
    {
        if (!IsValidIndex(fromIndex) || targetInventory == null || !targetInventory.IsValidIndex(toIndex))
            return false;

        return InventoryTransferUtility.MoveOrMergeOrSwap(slots[fromIndex], targetInventory.GetSlot(toIndex));
    }

    public virtual bool MoveItemToFirstValidSlot(int fromIndex, InventoryBase targetInventory)
    {
        if (!IsValidIndex(fromIndex) || targetInventory == null)
            return false;

        InventorySlot fromSlot = slots[fromIndex];
        if (fromSlot == null || fromSlot.IsEmpty())
            return false;

        return InventoryTransferUtility.MoveItemToFirstValidSlot(fromSlot, targetInventory);
    }

    public int GetFirstEmptySlotIndex(bool overflowSlot)
    {
        if (slots == null)
            return -1;

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];

            if (slot == null || slot.isOverflowSlot != overflowSlot)
                continue;

            if (slot.IsEmpty())
                return i;
        }

        return -1;
    }
    //Var olan slotun uzerine yazabilir dikkat!!!
    public virtual bool TrySetItemToSlot(int slotIndex, ItemData itemData, int amount = 1, bool allowOverflowSlot = false)
    {
        if (!IsValidIndex(slotIndex))
            return false;

        if (itemData == null || amount <= 0)
            return false;

        InventorySlot slot = slots[slotIndex];
        if (slot == null)
            return false;

        if (!slot.CanPlaceItem(itemData, allowOverflowSlot))
            return false;

        if (itemData.IsStackable)
            amount = Mathf.Min(amount, itemData.maxStackSize);
        else
            amount = 1;

        slot.SetItem(itemData, amount);
        return true;
    }

    public virtual bool TryClearSlot(int slotIndex)
    {
        if (!IsValidIndex(slotIndex))
            return false;

        slots[slotIndex].ClearSlot();
        return true;
    }

    public virtual void UnsafeForceSetItemToSlot(int slotIndex, ItemData itemData, int amount = 1)
    {
        if (!IsValidIndex(slotIndex))
            return;

        if (itemData == null || amount <= 0)
        {
            slots[slotIndex].ClearSlot();
            return;
        }

        if (!itemData.IsStackable)
            amount = 1;
        else
            amount = Mathf.Min(amount, itemData.maxStackSize);

        slots[slotIndex].SetItem(itemData, amount);
    }
}