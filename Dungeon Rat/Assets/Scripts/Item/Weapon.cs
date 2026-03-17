using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Item/EquipmentItemData/Weapon", order = 1)]
public class Weapon:EquipmentItemData
{
    public WeaponType WeaponType;
    public int durability;
    public int minDamage;
    public int maxDamage;
    public int range;
    public bool isBroken;
}
