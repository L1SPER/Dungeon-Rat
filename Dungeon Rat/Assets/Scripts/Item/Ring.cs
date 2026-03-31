using UnityEngine;

[CreateAssetMenu(fileName = "Ring", menuName = "Item/EquipmentItemData/Ring", order = 7)]
public class Ring : EquipmentItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.equipmentType = EquipmentType.Ring;
    }
}
