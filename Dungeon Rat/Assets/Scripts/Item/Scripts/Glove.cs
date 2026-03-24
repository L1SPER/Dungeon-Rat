using UnityEngine;

[CreateAssetMenu(fileName = "Glove", menuName = "Item/EquipmentItemData/Glove", order = 3)]
public class Glove : EquipmentItemData
{
    private void Awake()
    {
        this.equipmentType=EquipmentType.Glove;
        this.itemType = ItemType.Equipment;
    }
}
