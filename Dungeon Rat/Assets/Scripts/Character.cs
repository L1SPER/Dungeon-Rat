using UnityEngine;

public enum ClassType
{
    Warrior,
    Archer,
    Mage
}

public class Character 
{
    public string name;
    public ClassType classType;
    public Weapon weapon;
    public int position;
    public Character(string name, ClassType classType, Weapon weapon, int position)
    {
        this.name = name;
        this.classType = classType;
        this.weapon = weapon;
        this.position = position;
    }
}
