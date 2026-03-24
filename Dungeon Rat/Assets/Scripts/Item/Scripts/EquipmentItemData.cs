using UnityEngine;

public class EquipmentItemData : ItemData
{
    public EquipmentType equipmentType;
    public int durability;

    [Header("Bonus Stats")]
    public Stats bonusStats = new Stats();
}