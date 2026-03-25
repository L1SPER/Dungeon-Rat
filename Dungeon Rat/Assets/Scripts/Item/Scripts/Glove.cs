using UnityEngine;

[CreateAssetMenu(fileName = "Glove", menuName = "Item/EquipmentItemData/Glove", order = 3)]
public class Glove : EquipmentItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.equipmentType = EquipmentType.Glove;

    }
}
