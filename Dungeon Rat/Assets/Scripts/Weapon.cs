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

    public void UpdateWeaponName(string newName)
    {
        weaponName = newName;
    }

    public void UpdateWeaponType(WeaponType newType)
    {
        weaponType = newType;
    }

    public void UpdateMinDamage(int newMinDamage)
    {
        minDamage = newMinDamage;
    }

    public void UpdateMaxDamage(int newMaxDamage)
    {
        maxDamage = newMaxDamage;
    }

    public void UpdateDamage(int newMinDamage, int newMaxDamage)
    {
        minDamage = newMinDamage;
        maxDamage = newMaxDamage;
    }

    public void UpdateRange(int newRange)
    {
        range = newRange;
    }

    public void UpdateWeapon(string newName, WeaponType newType, int newMinDamage, int newMaxDamage, int newRange)
    {
        weaponName = newName;
        weaponType = newType;
        minDamage = newMinDamage;
        maxDamage = newMaxDamage;
        range = newRange;
    }

    public void UpdateWeapon(Weapon newWeapon)
    {
        weaponName = newWeapon.weaponName;
        weaponType = newWeapon.weaponType;
        minDamage = newWeapon.minDamage;
        maxDamage = newWeapon.maxDamage;
        range = newWeapon.range;
    }
}