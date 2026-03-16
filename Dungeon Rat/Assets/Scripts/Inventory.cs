using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory:ISerializationCallbackReceiver
{
    [SerializeField] private int currentCapacity;
    [SerializeField] private int maxCapacity;

    //[HideInInspector]
    [SerializeField] private InventorySlot[] inventorySlots;
    public Dictionary<int, InventorySlot> inventoryDictionary = new Dictionary<int, InventorySlot>();

    public InventorySlot[] InventorySlots => inventorySlots;
    public int CurrentCapacity => currentCapacity;
    public int MaxCapacity => maxCapacity;

    public void SyncSlotsWithCapacity()
    {
        if (currentCapacity < 0)
            currentCapacity = 0;

        if (inventorySlots == null || inventorySlots.Length != currentCapacity)
        {
            InventorySlot[] newSlots = new InventorySlot[currentCapacity];

            for (int i = 0; i < currentCapacity; i++)
            {
                if (inventorySlots != null && i < inventorySlots.Length && inventorySlots[i] != null)
                    newSlots[i] = inventorySlots[i];
                else
                    newSlots[i] = new InventorySlot();

                newSlots[i].slotID = i;
            }

            inventorySlots = newSlots;
        }
        else
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i] == null)
                    inventorySlots[i] = new InventorySlot();

                inventorySlots[i].slotID = i;
            }
        }
    }
    public void Clear()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].ClearSlot();
        }
    }

    public int FindInventorySlotId(InventorySlot _slot)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].slotID == _slot.slotID)
            {
                return inventorySlots[i].slotID;
            }
        }
        return -1;
    }

    public InventorySlot FindInventorySlot(int _slotId)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].slotID == _slotId)
            {
                return inventorySlots[i];
            }
        }
        return null;
    }

    public void OnBeforeSerialize()
    {
        // No special handling needed before serialization
    }
    
    public void OnAfterDeserialize()
    {
        inventoryDictionary = new Dictionary<int, InventorySlot>();
        for (int i = 0;i < inventorySlots.Length;i++)
        {
            inventorySlots[i].slotID = i;
        }
    }
   
    //public InventorySlot GetSlot(int index)
    //{
    //    if(!IsValidIndex(index))
    //    {
    //        Debug.LogError($"Invalid inventory slot index: {index}");
    //        return null;
    //    }
    //    return inventorySlots[index];
    //}
    //private bool IsValidIndex(int index)
    //{
    //    return inventorySlots != null && index >= 0 && index < inventorySlots.Length;   
    //}
    //public bool RemoveItem(int index, int amount = 1)
    //{
    //    if (!IsValidIndex(index))
    //    {
    //        Debug.LogError($"Invalid inventory slot index: {index}");
    //        return false;
    //    }
    //    InventorySlot slot = inventorySlots[index];
    //    if (slot.IsEmpty())
    //    {
    //        Debug.LogWarning($"Slot is empty at {index}");
    //        return false;
    //    }
    //    if (amount <= 0)
    //    {
    //        Debug.LogWarning($"Amount to remove must be greater than zero. Given: {amount}");
    //        return false;
    //    }

    //    if (slot.item.amount < amount)
    //    {
    //        Debug.LogWarning($"Not enough items in slot {index} to remove. Available: {slot.item.amount}, Requested: {amount}");
    //        return false;
    //    }

    //    if (slot.item.amount > amount)
    //    { 
    //        slot.RemoveAmount(amount);
    //        if (slot.item.amount <= 0)
    //        {
    //            slot.ClearSlot();
    //        }
    //    }
    //    return true;
    //}
    //public void ClearSlot(int index)
    //{
    //    if (!IsValidIndex(index))
    //    {
    //        Debug.LogError($"Invalid inventory slot index: {index}");
    //        return;
    //    }
    //    InventorySlot slot = inventorySlots[index];
    //    if (slot.IsEmpty())
    //    {
    //        Debug.LogWarning($"Slot is already empty at {index}");
    //        return;
    //    }
    //    slot.ClearSlot();
    //}
}
