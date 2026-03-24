using UnityEngine;

[System.Serializable]
public abstract class InventoryBase : ISerializationCallbackReceiver
{
    [UnityEngine.SerializeField] protected InventorySlot[] slots;

    public InventorySlot[] Slots => slots;

    public virtual void OnBeforeSerialize()
    {
    }

    public virtual void OnAfterDeserialize()
    {
        RepairSlots();
    }

    protected void RepairSlots()
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                slots[i] = new InventorySlot();

            slots[i].slotID = i;
        }
    }

    public bool IsValidIndex(int index)
    {
        return slots != null && index >= 0 && index < slots.Length;
    }

    public InventorySlot GetSlot(int index)
    {
        if (!IsValidIndex(index))
            return null;

        return slots[index];
    }

    public virtual void Clear()
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i]?.ClearSlot();
        }
    }

    public virtual bool MoveOrMergeOrSwapSlots(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA) || !IsValidIndex(indexB))
            return false;

        if (indexA == indexB)
            return false;

        return InventoryTransferUtility.MoveOrMergeOrSwap(slots[indexA], slots[indexB]);
    }
}