using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public Item item = new Item();
    public int slotID;
    public bool isLocked;

    [Header("Restrictions")]
    [SerializeField] private EquipmentType[] allowedEquipmentTypes = new EquipmentType[0];

    public InventorySlot()
    {
        slotID = -1;
        item = null;
        isLocked = false;
    }
    public InventorySlot(Item _item, int _id, bool _isLocked = false)
    {
        item = _item;
        slotID = _id;
        isLocked = _isLocked;
    }
    public bool IsEmpty()
    {
        return slotID < 0;
    }
    public void SetLocked(bool _isLocked)
    {
        isLocked = _isLocked;
    }
    public void SetItem(ItemData itemData, int amount)
    {
        item = new Item(itemData, amount);
    }
    public bool AreSameItem(Item _item)
    {
        return item == _item;
    }
    public void ClearSlot()
    {
        item = new Item();
        slotID = -1;
    }
    public void UpdateAllowedEquipmentTypes(EquipmentType[] newAllowedTypes)
    {
        for (int i = 0; i < allowedEquipmentTypes.Length; i++)
        {
            allowedEquipmentTypes[i] = newAllowedTypes[i];
        }
    }
    public void AddAmount(int amount)
    {
        item.amount += amount;
    }
}
