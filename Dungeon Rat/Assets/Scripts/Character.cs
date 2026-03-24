using TreeEditor;
using UnityEngine;

[System.Serializable]
public class Character : IDamageable
{
    public string name;
    public ClassType classType;
    public int position;

    [Header("Inventory Data")]
    [SerializeField] private EquipmentInventoryObject characterInventoryObject;
    public EquipmentInventoryObject CharacterInventoryObject => characterInventoryObject;

    [Header("Stats")]
    public Stats baseStats = new Stats();
    public Stats itemBonusStats = new Stats();
    public Stats activeCardBonusStats = new Stats();
    public Stats finalStats = new Stats();

    [Header("Runtime Variables")]
    public Health health = new Health();
    public int currentShield;

    public Character(string name, ClassType classType)
    {
        this.name = name;
        this.classType = classType;
        this.position = -1;
    }

    public Character(string name, ClassType classType, int position)
    {
        this.name = name;
        this.classType = classType;
        this.position = position;
    }

    public Character(string name, ClassType classType, int position, EquipmentInventoryObject inventoryObject)
    {
        this.name = name;
        this.classType = classType;
        this.position = position;
        this.characterInventoryObject= inventoryObject;
    }

    public Character(string name, ClassType classType, EquipmentInventoryObject inventoryObject)
    {
        this.name = name;
        this.classType = classType;
        this.characterInventoryObject = inventoryObject;
    }

    public void SetInventory(EquipmentInventoryObject inventoryObject)
    {
        characterInventoryObject = inventoryObject;
    }

    public void Initialize()
    {
        RecalculateStats();
        health.Initialize(finalStats.health);
        currentShield = finalStats.shield;
    }

    public void RecalculateStats()
    {
        finalStats.Clear();

        finalStats.Add(baseStats);
        finalStats.Add(itemBonusStats);
        finalStats.Add(activeCardBonusStats);

        ApplyDerivedStats();
        ClampFinalStats();

        health.SetMaxHealth(finalStats.health);
        currentShield = Mathf.Clamp(currentShield, 0, finalStats.shield);
    }

    public void ApplyDerivedStats()
    {
        finalStats.minDamage += finalStats.strength;
        finalStats.maxDamage += finalStats.strength;

        finalStats.critChance += finalStats.agility;
        finalStats.critDamage += finalStats.agility;

        finalStats.critChance = Mathf.Clamp(finalStats.critChance, 0, 100);
    }

    public void ClampFinalStats()
    {
        if (finalStats.health < 1)
        {
            Debug.LogWarning($"{name} final health 1'in altına düştü: {finalStats.health}");
            finalStats.health = 0;
        }

        if (finalStats.shield < 0)
        {
            Debug.LogWarning($"{name} final shield 0'ın altına düştü: {finalStats.shield}");
            finalStats.shield = Mathf.Clamp(finalStats.shield,0,finalStats.shield);
        }

        if (finalStats.range < 1)
        {
            Debug.LogWarning($"{name} final range 0'ın altına düştü: {finalStats.range}");
            finalStats.range = Mathf.Clamp(finalStats.range, 1, finalStats.range);
        }

        if (finalStats.critChance < 0 || finalStats.critChance > 100)
        {
            Debug.LogWarning($"{name} final critChance sınır dışında: {finalStats.critChance}");
            finalStats.critChance = Mathf.Clamp(finalStats.critChance, 0, 100);
        }

        if (finalStats.maxDamage < finalStats.minDamage)
        {
            Debug.LogWarning($"{name} final maxDamage minDamage'dan küçük. min:{finalStats.minDamage} max:{finalStats.maxDamage}");
            finalStats.maxDamage = finalStats.minDamage;
        }
    }

    public void UpdateName(string newName)
    {
        name = newName;
    }

    public void UpdateClassType(ClassType newClassType)
    {
        classType = newClassType;
    }

    public void UpdatePosition(int newPosition)
    {
        position = newPosition;
    }

    public void UpdateCharacter(Character newCharacter)
    {
        if (newCharacter == null)
        {
            Debug.LogWarning("UpdateCharacter called with null newCharacter");
            return;
        }

        this.name = newCharacter.name;
        this.classType = newCharacter.classType;
        this.position = newCharacter.position;

        this.baseStats.CopyFrom(newCharacter.baseStats);
        this.itemBonusStats.CopyFrom(newCharacter.itemBonusStats);
        this.activeCardBonusStats.CopyFrom(newCharacter.activeCardBonusStats);

        RecalculateStats();
    }

    public void SetItemBonusStats(Stats newItemBonusStats)
    {
        itemBonusStats.CopyFrom(newItemBonusStats);
        RecalculateStats();
    }

    public void SetActiveCardBonusStats(Stats newActiveCardBonusStats)
    {
        activeCardBonusStats.CopyFrom(newActiveCardBonusStats);
        RecalculateStats();
    }

    public void ClearItemBonusStats()
    {
        itemBonusStats.Clear();
        RecalculateStats();
    }

    public void ClearActiveCardBonusStats()
    {
        activeCardBonusStats.Clear();
        RecalculateStats();
    }

    public void Heal(int amount)
    {
        health.Heal(amount);
    }

    public void RestoreShield(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("RestoreShield called with non-positive amount: " + amount);
            return;
        }

        currentShield += amount;

        if (currentShield > finalStats.shield)
            currentShield = finalStats.shield;
    }

    public void TakeDamage(int damage)
    {
        if (health.isInvulnerable || health.isDead || damage <= 0)
        {
            Debug.LogWarning($"{name} is invulnerable, dead, or damage is non-positive. No damage taken.");
            return;
        }

        int reducedDmg = Mathf.Max(damage - finalStats.armor, 0);

        if (reducedDmg <= 0)
        {
            Debug.Log($"{name} armor absorbed all damage. No health lost.");
            return;
        }

        int restDmg = 0;

        if (currentShield > 0)
        {
            restDmg = TakeDamageToShield(reducedDmg);
        }

        if (restDmg <= 0)
        {
            Debug.Log($"{name} shield absorbed all damage. No health lost.");
            return;
        }
        health.TakeDamage(restDmg);
    }

    public int TakeDamageToShield(int dmg)
    {
        if (dmg <= 0)
        {
            Debug.LogWarning("TakeShieldDamage called with non-positive damage: " + dmg);
            return 0;
        }
        int shieldAbsorb = Mathf.Min(currentShield, dmg);
        currentShield -= shieldAbsorb;

        dmg -= shieldAbsorb;
        return dmg > 0 ? dmg : 0;
    }

}
