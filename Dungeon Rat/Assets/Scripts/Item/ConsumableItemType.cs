using UnityEngine;

public class ConsumableItemData : ItemData
{
    public int healAmount;
    virtual protected void Awake()
    {
        itemType = ItemType.Consumable;
    }
    virtual public void Consume()
    {
        Debug.Log("Consumed " + itemName);
    }
}
