using UnityEngine;

[CreateAssetMenu(fileName = "Chest", menuName = "Item/EquipmentItemData/Chest", order = 2)]
public class Chest : EquipmentItemData
{
    private void Awake()
    {
        this.equipmentType = EquipmentType.Chest;
        this.itemType = ItemType.Equipment;
    }
}
