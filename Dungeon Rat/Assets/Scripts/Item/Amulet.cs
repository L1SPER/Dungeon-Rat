using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Amulet", menuName = "Item/EquipmentItemData/Amulet", order = 6)]
public class Amulet : EquipmentItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.equipmentType = EquipmentType.Amulet;
    }
}
