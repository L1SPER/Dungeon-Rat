using UnityEngine;

[CreateAssetMenu(fileName = "Shoe", menuName = "Item/EquipmentItemData/Shoe", order = 5)]
public class Shoe : EquipmentItemData
{
    private void Awake()
    {
        this.equipmentType = EquipmentType.Shoe;
        this.itemType = ItemType.Equipment;
    }
}
