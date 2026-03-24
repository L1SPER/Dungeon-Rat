using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Amulet", menuName = "Item/EquipmentItemData/Amulet", order = 6)]
public class Amulet : EquipmentItemData
{
    private void Awake()
    {
        this.equipmentType = EquipmentType.Amulet;
        this.itemType = ItemType.Equipment;
    }
}
