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

    public Item(ItemData _itemData, int _amount)
    {
        this.itemData = _itemData;
        this.amount = _itemData != null ? Mathf.Max(1, _amount) : 0;
    }

    public Item(Item _item)
    {
        if (_item == null || _item.itemData == null || _item.amount <= 0)
        {
            itemData = null;
            amount = 0;
            return;
        }


        this.itemData = _item.itemData;
        this.amount = _item.amount;
    }

    public bool IsEmpty() //ItemData var mi yok mu
    {
        return itemData == null || amount<=0;
    }

    public bool IsStackable()
    {
        return itemData != null && itemData.IsStackable;
    }

    public int RemainingStackSpace()
    {
        if (!IsStackable())
            return 0;

        return Mathf.Max(0, itemData.maxStackSize - amount);
    }

}