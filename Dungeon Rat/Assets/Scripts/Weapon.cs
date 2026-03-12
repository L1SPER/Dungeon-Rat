using Unity;
using UnityEngine;
public enum WeaponType
{
    Sword,
    Axe,
    Bow,
    Crossbow,
    Staff,
    Wand
}
public class Weapon
{
    public string weaponName;
    public WeaponType weaponType;
    public int minDamage;
    public int maxDamage;
    public int range;
    public Weapon(string weaponName, WeaponType weaponType, int minDamage, int maxDamage, int range)
    {
        this.weaponName = weaponName;
        this.weaponType = weaponType;
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
        this.range = range;
    }
    public int GetDamage()
    {
        return Random.Range(minDamage, maxDamage + 1);
    }
}