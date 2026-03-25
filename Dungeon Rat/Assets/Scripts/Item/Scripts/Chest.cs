using UnityEngine;

[CreateAssetMenu(fileName = "Chest", menuName = "Item/EquipmentItemData/Chest", order = 2)]
public class Chest : EquipmentItemData
{
    protected override void Awake()
    {
        base.Awake();
        equipmentType = EquipmentType.Chest;
    }
}
