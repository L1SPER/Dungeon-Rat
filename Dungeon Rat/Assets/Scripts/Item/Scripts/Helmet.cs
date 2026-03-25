using UnityEngine;

[CreateAssetMenu(fileName = "Helmet", menuName = "Item/EquipmentItemData/Helmet", order = 1)]
public class Helmet : EquipmentItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.equipmentType = EquipmentType.Helmet;

    }
}
