using UnityEngine;
[System.Serializable]
public class Item 
{
    public ItemData itemData;
    public int amount;
    public Item()
    {
        itemData = null;
        amount = 0;
    }
    public Item(ItemData itemData, int amount)
    {
        this.itemData = itemData;
        this.amount = itemData != null ? amount:0;
    }
    public Item(Item item)
    {
        if (item == null)
            return;

        this.itemData = item.itemData;
        this.amount = item.amount;
    }
    public bool IsEmpty() //ItemData var mi yok mu
    {
        return itemData == null;
    }
    public bool IsStackable()
    {
        return itemData!=null && itemData.isStackable;
    }
    public int GetItemID()
    {
        return itemData != null ? itemData.itemID : -1;
    }
    public string GetItemName()
    {
        return itemData != null ? itemData.itemName : string.Empty;
    }
    public int RemainingStackSpace()
    {
        if (!IsStackable())
            return 0;
        return itemData.maxStackSize - amount;
    }
}