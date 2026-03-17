using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public int health;
    public int armor;
    public int shield;

    public int strength;
    public int agility;
    public int intelligence;

    public int minDamage;
    public int maxDamage;
    public int critChance;

    public int range;

    public Stats()
    {
        health = 0;
        armor = 0;
        shield = 0;
        
        strength = 0;
        agility = 0;
        intelligence = 0;
        
        minDamage = 0;
        maxDamage = 0;
        critChance = 0;
        
        range = 0;
    }
    public Stats(int health, int armor, int shield, int strength, int agility, int intelligence, int minDamage, int maxDamage, int critChance, int range)
    {
        this.health = health;
        this.armor = armor;
        this.shield = shield;

        this.strength = strength;
        this.agility = agility;
        this.intelligence = intelligence;

        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
        this.critChance = critChance;

        this.range = range;
    }

    public void Clear()
    {
        health = 0;
        armor = 0;
        shield = 0;

        strength = 0;
        agility = 0;
        intelligence = 0;

        minDamage = 0;
        maxDamage = 0;
        critChance = 0;

        range = 0;
    }

    public void Add(Stats other)
    {
        health += other.health;
        armor += other.armor;
        shield += other.shield;

        strength += other.strength;
        agility += other.agility;
        intelligence += other.intelligence;

        minDamage += other.minDamage;
        maxDamage += other.maxDamage;
        critChance += other.critChance;

        range += other.range;
    }

    public void Subtract(Stats other)
    {
        health -= other.health;
        armor -= other.armor;
        shield -= other.shield;

        strength -= other.strength;
        agility -= other.agility;
        intelligence -= other.intelligence;

        minDamage -= other.minDamage;
        maxDamage -= other.maxDamage;
        critChance -= other.critChance;

        range -= other.range;
    }

    public Stats Clone()
    {
        return new Stats(this.health,
            this.armor,
            this.shield,
            this.strength,
            this.agility,
            this.intelligence,
            this.minDamage,
            this.maxDamage,
            this.critChance,
            this.range
            );
    }

    public void CopyFrom(Stats other)
    {
        if (other == null)
            return;

        this.health = other.health;
        this.armor = other.armor;
        this.shield = other.shield;

        this.strength = other.strength;
        this.agility = other.agility;
        this.intelligence = other.intelligence;

        this.minDamage = other.minDamage;
        this.maxDamage = other.maxDamage;
        this.critChance = other.critChance;

        this.range = other.range;
    }

    public static Stats operator+(Stats a, Stats b)
    {
        Stats result = new Stats();

        if(a!= null) result.Add(a);
        if(b!= null) result.Add(b);

        return result;
    }

    public static Stats operator-(Stats a, Stats b)
    {
        Stats result = new Stats();
        if(a != null) result.Add(a);
        if(b != null) result.Subtract(b);

        return result;
    }
}
