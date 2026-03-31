using UnityEngine;

[CreateAssetMenu(fileName = "Trousers", menuName = "Item/EquipmentItemData/Trousers", order = 4)]
public class Trousers : EquipmentItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.equipmentType = EquipmentType.Trousers;
    }
}
