using UnityEngine;

public class EquipmentItemData : ItemData
{
    public EquipmentType equipmentType;

    [Header("Stats")] //Statlar sonradan degisebilir.
    public Stats bonusStats;
}