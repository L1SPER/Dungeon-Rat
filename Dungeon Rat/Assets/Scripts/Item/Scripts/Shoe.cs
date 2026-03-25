using UnityEngine;

[CreateAssetMenu(fileName = "Shoe", menuName = "Item/EquipmentItemData/Shoe", order = 5)]
public class Shoe : EquipmentItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.equipmentType = EquipmentType.Shoe;
    }
}
