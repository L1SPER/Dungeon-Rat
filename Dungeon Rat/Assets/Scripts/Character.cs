using UnityEngine;

[System.Serializable]
public class Character : IDamageable
{
    public string name;
    public ClassType classType;
    public Weapon weapon;
    public int position;


    public Stats baseStats;
    public Stats currentStats;
    public Health health= new Health();

    public void Initialize()
    {
        health.Initialize(currentStats.health);
        RecalculateStats();
    }

    public void RecalculateStats()
    {
        currentStats.health = baseStats.health;
        currentStats.armor = baseStats.armor;
        currentStats.shield = baseStats.shield;

        currentStats.strength = baseStats.strength;
        currentStats.agility = baseStats.agility;
        currentStats.intelligence = baseStats.intelligence;
        
        currentStats.minDamage = baseStats.minDamage;
        currentStats.maxDamage = baseStats.maxDamage;
        
        currentStats.critChance = baseStats.critChance;
        currentStats.range = baseStats.range;

        health.SetMaxHealth(currentStats.health);
    }
    
    public Character(string name, ClassType classType)//(string name, ClassType classType, Weapon weapon)
    {
        this.name = name;
        this.classType = classType;
        //this.weapon = weapon;
        this.position = -1; 
    }

    public Character(string name, ClassType classType,int position)//(string name, ClassType classType, Weapon weapon, int position)
    {
        this.name = name;
        this.classType = classType;
        //this.weapon = weapon;
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

    //public void UpdateWeapon(Weapon newWeapon)
    //{
    //    weapon = newWeapon;
    //}
    
    public void UpdatePosition(int newPosition)
    {
        position = newPosition;
    }

    public void UpdateCharacter(Character newCharacter)
    {
        this.name = newCharacter.name;
        this.classType = newCharacter.classType;
        //this.weapon = newCharacter.weapon;
        this.position = newCharacter.position;
    }

    public void TakeDamage(int damage)
    {
        int reducedDmg = Mathf.Max(damage - currentStats.armor, 0);
        health.TakeDamage(reducedDmg);
    }
}
