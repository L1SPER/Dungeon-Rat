using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Character : IDamageable
{
    public string name;
    public ClassType classType;
    public int position;
    public Sprite characterSprite;

    [Header("Temporary Battle Effects")]
    public bool skipNextTurnOnce;
    private bool firstHitDamageReductionActive;
    private int firstHitDamageReductionPercent;

    private Dictionary<AbilityBase, int> abilityCooldowns = new Dictionary<AbilityBase, int>();

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
    public Shield shield = new Shield();

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
        this.characterInventoryObject = inventoryObject;
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
        shield.Initialize(finalStats.shield);
    }

    public void RecalculateStats()
    {
        finalStats.Clear();

        finalStats.Add(baseStats);
        finalStats.Add(itemBonusStats);
        finalStats.Add(activeCardBonusStats);

        ApplyDerivedStats();
        ClampFinalStats();

        if (health != null && health.maxHealth > 0)
            health.SetMaxHealth(finalStats.health);

        if (shield != null)
            shield.SetMaxShield(finalStats.shield);
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
            finalStats.health = 1;
        }

        if (finalStats.shield < 0)
        {
            Debug.LogWarning($"{name} final shield 0'ın altına düştü: {finalStats.shield}");
            finalStats.shield = 0;
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
        if (shield == null)
            return;

        shield.Restore(amount);
    }

    public void TakeDamage(int damage)
    {
        ApplyDamage(damage);
    }

    public int ApplyDamage(int damage)
    {
        if (health.isInvulnerable)
        {
            BattleDebugLogger.LogCharacterDamageIgnored(name, damage, "Invulnerable");
            return 0;
        }

        if (health.isDead)
        {
            BattleDebugLogger.LogCharacterDamageIgnored(name, damage, "AlreadyDead");
            return 0;
        }

        if (damage <= 0)
        {
            BattleDebugLogger.LogCharacterDamageIgnored(name, damage, "NonPositiveDamage");
            return 0;
        }

        int rawDamage = damage;
        int incomingDamage = damage;

        string firstHitReductionInfo = "none";
        if (firstHitDamageReductionActive)
        {
            firstHitReductionInfo = $"%{firstHitDamageReductionPercent}";
            incomingDamage = Mathf.CeilToInt(incomingDamage * (100 - firstHitDamageReductionPercent) / 100f);
            firstHitDamageReductionActive = false;
        }

        int damageReductionFromArmor = finalStats.armor * 5;
        int afterArmor = Mathf.Max(incomingDamage - damageReductionFromArmor, 0);

        int shieldBefore = shield.currentShield;
        int healthBefore = health.currentHealth;

        if (afterArmor <= 0)
        {
            BattleDebugLogger.LogCharacterDamage(
                name,
                rawDamage,
                firstHitReductionInfo,
                incomingDamage,
                finalStats.armor,
                damageReductionFromArmor,
                afterArmor,
                shieldBefore,
                shield.currentShield,
                healthBefore,
                health.currentHealth,
                0
            );

            return 0;
        }

        int remainingDamage = afterArmor;

        if (shield.currentShield > 0)
            remainingDamage = TakeDamageToShield(afterArmor);

        if (remainingDamage > 0)
            health.TakeDamage(remainingDamage);

        int shieldAfter = shield.currentShield;
        int healthAfter = health.currentHealth;

        int shieldDamage = shieldBefore - shieldAfter;
        int healthDamage = healthBefore - healthAfter;
        int appliedDamage = shieldDamage + healthDamage;

        BattleDebugLogger.LogCharacterDamage(
            name,
            rawDamage,
            firstHitReductionInfo,
            incomingDamage,
            finalStats.armor,
            damageReductionFromArmor,
            afterArmor,
            shieldBefore,
            shieldAfter,
            healthBefore,
            healthAfter,
            appliedDamage
        );

        return appliedDamage;
    }

    // Eski method adını korudum, dışarıdan çağrılıyorsa patlamasın
    public int TakeDamageToShield(int dmg)
    {
        if (shield == null)
            return dmg;

        return shield.Absorb(dmg);
    }

    private bool HasCompatibleWeapon(Character character)
    {
        if (character == null)
            return false;

        Weapon weapon = character.GetEquippedWeapon();
        if (weapon == null)
            return false;

        switch (character.classType)
        {
            case ClassType.Warrior:
                return weapon.WeaponType == WeaponType.Sword || weapon.WeaponType == WeaponType.Axe;
            case ClassType.Archer:
                return weapon.WeaponType == WeaponType.Bow || weapon.WeaponType == WeaponType.Crossbow;
            case ClassType.Mage:
                return weapon.WeaponType == WeaponType.Staff || weapon.WeaponType == WeaponType.Wand;
        }

        return false;
    }

    public Weapon GetEquippedWeapon()
    {
        if (CharacterInventoryObject == null || CharacterInventoryObject.inventory == null)
            return null;

        EquipmentItemData equippedItem = CharacterInventoryObject.inventory.GetEquippedItem(7);
        return equippedItem as Weapon;
    }

    public void ApplySkipNextTurn()
    {
        skipNextTurnOnce = true;
    }

    public bool ConsumeSkipNextTurn()
    {
        if (!skipNextTurnOnce)
            return false;

        skipNextTurnOnce = false;
        return true;
    }

    public void SetFirstHitDamageReduction(int percent)
    {
        firstHitDamageReductionActive = true;
        firstHitDamageReductionPercent = Mathf.Clamp(percent, 0, 100);
    }

    public bool IsAbilityOnCooldown(AbilityBase ability)
    {
        if (ability == null)
            return false;

        if (!abilityCooldowns.TryGetValue(ability, out int remainingTurns))
            return false;

        return remainingTurns > 0;
    }

    public int GetAbilityCooldownRemaining(AbilityBase ability)
    {
        if (ability == null)
            return 0;

        if (!abilityCooldowns.TryGetValue(ability, out int remainingTurns))
            return 0;

        return Mathf.Max(0, remainingTurns);
    }

    public void StartAbilityCooldown(AbilityBase ability)
    {
        if (ability == null || ability.cooldownTurns <= 0)
            return;

        abilityCooldowns[ability] = ability.cooldownTurns;
    }

    public void ReduceAbilityCooldowns()
    {
        if (abilityCooldowns.Count == 0)
            return;

        List<AbilityBase> keys = new List<AbilityBase>(abilityCooldowns.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            AbilityBase ability = keys[i];
            abilityCooldowns[ability]--;

            if (abilityCooldowns[ability] <= 0)
                abilityCooldowns[ability] = 0;
        }
    }
}