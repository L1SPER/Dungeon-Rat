using UnityEngine;

[CreateAssetMenu(fileName = "Helmet", menuName = "Item/EquipmentItemData/Helmet", order = 1)]
public class Helmet : EquipmentItemData
{
    private void Awake()
    {
        this.equipmentType = EquipmentType.Helmet;
        this.itemType = ItemType.Equipment;
    }
}
