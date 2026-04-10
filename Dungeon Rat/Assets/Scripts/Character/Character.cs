using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Character : IDamageable
{
    // UI'daki hangi slota bağlı olduğunu tutar
    public BattleSlotUI SlotUI { get; set; }
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
        SetInventory(inventoryObject);
    }

    public Character(string name, ClassType classType, EquipmentInventoryObject inventoryObject)
    {
        this.name = name;
        this.classType = classType;
        SetInventory(inventoryObject);
    }

    public void SetInventory(EquipmentInventoryObject inventoryObject)
    {
        characterInventoryObject = inventoryObject;
        characterInventoryObject?.BindOwner(this);
    }

    public void RefreshEquipmentBonusStatsFromInventory()
    {
        Stats calculatedItemStats = new Stats();

        if (characterInventoryObject != null && characterInventoryObject.inventory != null)
        {
            EquipmentInventory equipmentInventory = characterInventoryObject.inventory;
            equipmentInventory.ConfigureDefaultRestrictions();

            InventorySlot[] slots = equipmentInventory.Slots;
            if (slots != null)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    InventorySlot slot = slots[i];
                    if (slot == null || slot.IsEmpty())
                        continue;

                    EquipmentItemData equippedItem = slot.item.itemData as EquipmentItemData;
                    if (equippedItem == null)
                        continue;

                    if (equippedItem.bonusStats != null)
                        calculatedItemStats.Add(equippedItem.bonusStats);

                    Weapon weapon = equippedItem as Weapon;
                    if (weapon != null)
                    {
                        calculatedItemStats.minDamage += weapon.minDamage;
                        calculatedItemStats.maxDamage += weapon.maxDamage;
                    }
                }
            }
        }

        SetItemBonusStats(calculatedItemStats);
    }

    public void Initialize()
    {
        RefreshEquipmentBonusStatsFromInventory();
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

        finalStats.critChance += finalStats.agility * 5; ;
        finalStats.critDamage += finalStats.agility * 5; ;

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

    public void ResetAbilityCooldowns()
    {
        abilityCooldowns.Clear();
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
            return 0;

        if (firstHitDamageReductionActive)
        {
            damage = Mathf.RoundToInt(damage * (1f - firstHitDamageReductionPercent / 100f));
            firstHitDamageReductionActive = false;
        }

        int remainingAfterShield = ApplyDamageToShield(damage);
        if (remainingAfterShield > 0)
            health.TakeDamage(remainingAfterShield);

        return damage;
    }

    private int ApplyDamageToShield(int damage)
    {
        if (shield == null || shield.currentShield <= 0)
            return damage;

        return shield.Absorb(damage);
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

    public void ReduceCooldownsAtTurnStart()
    {
        if (abilityCooldowns.Count == 0)
            return;

        List<AbilityBase> keys = new List<AbilityBase>(abilityCooldowns.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            AbilityBase ability = keys[i];
            abilityCooldowns[ability] = Mathf.Max(0, abilityCooldowns[ability] - 1);
        }
    }
    public void ReduceAbilityCooldowns()
    {
        if (abilityCooldowns.Count == 0)
            return;

        List<AbilityBase> keys = new List<AbilityBase>(abilityCooldowns.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            AbilityBase ability = keys[i];
            abilityCooldowns[ability] = Mathf.Max(0, abilityCooldowns[ability] - 1);
        }
    }

    public void ClearEquipmentInventoryOnDeath()
    {
        if (characterInventoryObject == null || characterInventoryObject.inventory == null)
            return;

        characterInventoryObject.inventory.Clear();
        RefreshEquipmentBonusStatsFromInventory();
    }
}