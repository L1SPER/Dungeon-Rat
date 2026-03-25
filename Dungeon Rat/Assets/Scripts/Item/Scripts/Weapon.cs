using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Item/EquipmentItemData/Weapon", order = 8)]
public class Weapon:EquipmentItemData
{
    public WeaponType WeaponType;
    public int minDamage;
    public int maxDamage;
    public int range;
    public bool isBroken;
    protected override void Awake()
    {
        base.Awake();
        this.maxStackSize = 1;
        this.equipmentType = EquipmentType.Weapon;
    }
}
