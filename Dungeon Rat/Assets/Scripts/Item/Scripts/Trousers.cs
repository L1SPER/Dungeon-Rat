using UnityEngine;

[CreateAssetMenu(fileName = "Trousers", menuName = "Item/EquipmentItemData/Trousers", order = 4)]
public class Trousers : EquipmentItemData
{
    private void Awake()
    {
        this.equipmentType=EquipmentType.Trousers;
        this.itemType = ItemType.Equipment;
    }
}
