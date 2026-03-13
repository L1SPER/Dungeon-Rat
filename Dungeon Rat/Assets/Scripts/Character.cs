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

    public Character(string name, ClassType classType, Weapon weapon)
    {
        this.name = name;
        this.classType = classType;
        this.weapon = weapon;
        this.position = -1; 
    }
    public Character(string name, ClassType classType, Weapon weapon, int position)
    {
        this.name = name;
        this.classType = classType;
        this.weapon = weapon;
        this.position = position;
    }
    public void UpdateName(string newName)
    {
        name = newName;
    }
    public void UpdateClassType(ClassType newClassType)
    {
        classType = newClassType;
    }
    public void UpdateWeapon(Weapon newWeapon)
    {
        weapon = newWeapon;
    }
    public void UpdatePosition(int newPosition)
    {
        position = newPosition;
    }
    public void UpdateCharacter(Character newCharacter)
    {
        this.name = newCharacter.name;
        this.classType = newCharacter.classType;
        this.weapon = newCharacter.weapon;
        this.position = newCharacter.position;
    }
}
