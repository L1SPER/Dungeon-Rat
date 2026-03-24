using UnityEngine;

[CreateAssetMenu(fileName = "Ring", menuName = "Item/EquipmentItemData/Ring", order = 7)]
public class Ring : EquipmentItemData
{
    private void Awake()
    {
        this.equipmentType=EquipmentType.Ring;
        this.itemType = ItemType.Equipment;
    }
}
