using UnityEngine;
using System;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour
{
    private Character[] partySlots = new Character[3];

    [SerializeField] private EquipmentInventoryObject[] characterInventoryObject = new EquipmentInventoryObject[3];
    [SerializeField] private EquipmentItemData[] weapons = new EquipmentItemData[3];

    private readonly Queue<EquipmentInventoryObject> availableDeadInventories = new Queue<EquipmentInventoryObject>();

    private bool hasReceivedStartingParty;

    public event Action OnPartyChanged;

    public bool HasReceivedStartingParty => hasReceivedStartingParty;

    private void Start()
    {
        if (SaveSystemManager.Instance != null && SaveSystemManager.Instance.TryApplyLoadedParty())
        {
            Debug.Log("Party loaded from save.");
            return;
        }

        if (!hasReceivedStartingParty)
            CreateStartingParty();
    }

    public void CreateStartingParty()
    {
        if (!IsPartyEmpty())
            return;

        Character warrior = new Character("Borin", ClassType.Warrior, characterInventoryObject[0]);
        Character archer = new Character("Lira", ClassType.Archer, characterInventoryObject[1]);
        Character mage = new Character("Mira", ClassType.Mage, characterInventoryObject[2]);

        ApplyBaseStatsByClass(warrior);
        ApplyBaseStatsByClass(archer);
        ApplyBaseStatsByClass(mage);

        AddCharacterToSlot(warrior, 0);
        AddCharacterToSlot(archer, 1);
        AddCharacterToSlot(mage, 2);

        GiveWeaponToCharacter(warrior, weapons[0]);
        GiveWeaponToCharacter(archer, weapons[1]);
        GiveWeaponToCharacter(mage, weapons[2]);

        warrior.Initialize();
        archer.Initialize();
        mage.Initialize();

        hasReceivedStartingParty = true;
        RebuildAvailableDeadInventories();

        Debug.Log("Starting party is created!");
        PrintParty();
        OnPartyChanged?.Invoke();
    }

    public void ApplyBaseStatsByClass(Character character)
    {
        if (character == null)
            return;

        character.baseStats.Clear();

        switch (character.classType)
        {
            case ClassType.Warrior:
                character.baseStats.health = 100;
                character.baseStats.armor = 0;
                character.baseStats.shield = 5;
                character.baseStats.strength = 1;
                character.baseStats.agility = 0;
                character.baseStats.intelligence = 0;
                character.baseStats.minDamage = 3;
                character.baseStats.maxDamage = 5;
                character.baseStats.critChance = 0;
                character.baseStats.critDamage = 0;
                break;

            case ClassType.Archer:
                character.baseStats.health = 90;
                character.baseStats.armor = 0;
                character.baseStats.shield = 0;
                character.baseStats.strength = 0;
                character.baseStats.agility = 1;
                character.baseStats.intelligence = 0;
                character.baseStats.minDamage = 2;
                character.baseStats.maxDamage = 4;
                character.baseStats.critChance = 0;
                character.baseStats.critDamage = 0;
                break;

            case ClassType.Mage:
                character.baseStats.health = 80;
                character.baseStats.armor = 0;
                character.baseStats.shield = 0;
                character.baseStats.strength = 0;
                character.baseStats.agility = 0;
                character.baseStats.intelligence = 1;
                character.baseStats.minDamage = 1;
                character.baseStats.maxDamage = 3;
                character.baseStats.critChance = 0;
                character.baseStats.critDamage = 0;
                break;
        }
    }

    private void GiveWeaponToCharacter(Character character, EquipmentItemData weapon)
    {
        if (character == null || weapon == null)
        {
            Debug.LogWarning("Character or weapon is null.");
            return;
        }

        if (character.CharacterInventoryObject == null || character.CharacterInventoryObject.inventory == null)
        {
            Debug.LogWarning($"Character inventory is missing for {character.name}");
            return;
        }

        EquipmentInventory equipmentInventory = character.CharacterInventoryObject.inventory;
        equipmentInventory.ConfigureDefaultRestrictions();

        int weaponSlotIndex = equipmentInventory.FindSlotIndexByEquipmentType(EquipmentType.Weapon);

        if (weaponSlotIndex == -1)
        {
            Debug.LogWarning($"Weapon slot not found for {character.name}");
            return;
        }

        bool success = equipmentInventory.TrySetItemToSlot(weaponSlotIndex, weapon, 1);

        if (!success)
        {
            Debug.LogWarning($"Could not equip starting weapon to {character.name}");
            return;
        }
    }

    public bool RecruitCharacter(Character character, EquipmentItemData randomWeapon)
    {
        if (character == null)
        {
            Debug.LogWarning("RecruitCharacter called with null character.");
            return false;
        }

        if (GetAlivePartySize() >= partySlots.Length)
        {
            Debug.Log("Parti dolu. Recruit yapılamadı.");
            return false;
        }

        EquipmentInventoryObject reusableInventory = GetReusableDeadInventory();
        if (reusableInventory == null)
        {
            Debug.LogWarning("Recruit için kullanılabilir boş equipment inventory bulunamadı.");
            return false;
        }

        character.SetInventory(reusableInventory);
        ApplyBaseStatsByClass(character);

        if (!AddCharacter(character))
            return false;

        if (randomWeapon != null)
            GiveWeaponToCharacter(character, randomWeapon);

        character.Initialize();
        hasReceivedStartingParty = true;
        NotifyPartyChanged();
        return true;
    }

    public bool AddCharacter(Character character)
    {
        if (character == null)
        {
            Debug.LogWarning("Character is null");
            return false;
        }

        for (int i = 0; i < partySlots.Length; i++)
        {
            if (partySlots[i] == null)
                return AddCharacterToSlot(character, i);
        }

        Debug.Log("Parti dolu.");
        return false;
    }

    private bool AddCharacterToSlot(Character character, int slotIndex)
    {
        if (character == null)
            return false;

        if (slotIndex < 0 || slotIndex >= partySlots.Length)
        {
            Debug.LogWarning("Invalid slot index.");
            return false;
        }

        if (partySlots[slotIndex] != null)
        {
            Debug.LogWarning($"Slot {slotIndex + 1} is already occupied.");
            return false;
        }

        partySlots[slotIndex] = character;
        character.position = slotIndex + 1;

        RegisterCharacter(character);

        Debug.Log($"{character.name} added party. Position: {character.position}");
        OnPartyChanged?.Invoke();
        return true;
    }

    private void RegisterCharacter(Character character)
    {
        if (character == null || character.health == null)
            return;

        character.health.OnDeath -= OnAnyCharacterDeath;
        character.health.OnDeath += OnAnyCharacterDeath;
    }

    private void OnAnyCharacterDeath()
    {
        PrepareDeadCharactersInventoriesForRecruitment();
        CompactParty();
        RebuildAvailableDeadInventories();
        PrintParty();
        OnPartyChanged?.Invoke();
    }

    private void PrepareDeadCharactersInventoriesForRecruitment()
    {
        for (int i = 0; i < partySlots.Length; i++)
        {
            Character character = partySlots[i];

            if (character == null || character.health == null)
                continue;

            if (!character.health.isDead)
                continue;

            EquipmentInventoryObject inventoryObject = character.CharacterInventoryObject;
            if (inventoryObject == null || inventoryObject.inventory == null)
                continue;

            character.health.OnDeath -= OnAnyCharacterDeath;
            character.ClearEquipmentInventoryOnDeath();
            character.SetInventory(null);

            if (!availableDeadInventories.Contains(inventoryObject))
                availableDeadInventories.Enqueue(inventoryObject);
        }
    }

    private EquipmentInventoryObject GetReusableDeadInventory()
    {
        while (availableDeadInventories.Count > 0)
        {
            EquipmentInventoryObject inventoryObject = availableDeadInventories.Dequeue();
            if (inventoryObject == null || inventoryObject.inventory == null)
                continue;

            inventoryObject.inventory.ConfigureDefaultRestrictions();
            inventoryObject.inventory.Clear();
            return inventoryObject;
        }

        return null;
    }

    private int GetInventoryIndex(EquipmentInventoryObject inventoryObject)
    {
        if (inventoryObject == null || characterInventoryObject == null)
            return -1;

        for (int i = 0; i < characterInventoryObject.Length; i++)
        {
            if (characterInventoryObject[i] == inventoryObject)
                return i;
        }

        return -1;
    }

    private void RebuildAvailableDeadInventories()
    {
        availableDeadInventories.Clear();

        HashSet<EquipmentInventoryObject> usedInventories = new HashSet<EquipmentInventoryObject>();

        for (int i = 0; i < partySlots.Length; i++)
        {
            Character character = partySlots[i];
            if (character == null || character.CharacterInventoryObject == null)
                continue;

            usedInventories.Add(character.CharacterInventoryObject);
        }

        if (characterInventoryObject == null)
            return;

        for (int i = 0; i < characterInventoryObject.Length; i++)
        {
            EquipmentInventoryObject inventoryObject = characterInventoryObject[i];
            if (inventoryObject == null)
                continue;

            if (inventoryObject.inventory != null)
                inventoryObject.inventory.ConfigureDefaultRestrictions();

            if (!usedInventories.Contains(inventoryObject))
                availableDeadInventories.Enqueue(inventoryObject);
        }
    }

    public PartySaveData CreatePartySaveData()
    {
        PartySaveData saveData = new PartySaveData();
        saveData.hasReceivedStartingParty = hasReceivedStartingParty;

        for (int i = 0; i < partySlots.Length; i++)
        {
            Character character = partySlots[i];
            if (character == null)
                continue;

            CharacterSaveData characterSave = new CharacterSaveData();
            characterSave.characterName = character.name;
            characterSave.classType = (int)character.classType;
            characterSave.slotIndex = i;
            characterSave.equipmentInventoryIndex = GetInventoryIndex(character.CharacterInventoryObject);
            characterSave.currentHealth = character.health != null ? character.health.currentHealth : 0;
            characterSave.currentShield = character.shield != null ? character.shield.currentShield : 0;
            characterSave.baseStats = ConvertStatsToSaveData(character.baseStats);
            characterSave.activeCardBonusStats = ConvertStatsToSaveData(character.activeCardBonusStats);

            saveData.members.Add(characterSave);
        }

        return saveData;
    }

    public bool TryLoadParty(PartySaveData partySaveData)
    {
        if (partySaveData == null || !partySaveData.hasReceivedStartingParty)
            return false;

        ClearParty(false);
        hasReceivedStartingParty = true;

        if (partySaveData.members != null)
        {
            for (int i = 0; i < partySaveData.members.Count; i++)
            {
                CharacterSaveData memberSave = partySaveData.members[i];
                if (memberSave == null)
                    continue;

                if (memberSave.slotIndex < 0 || memberSave.slotIndex >= partySlots.Length)
                    continue;

                if (memberSave.equipmentInventoryIndex < 0 || memberSave.equipmentInventoryIndex >= characterInventoryObject.Length)
                {
                    Debug.LogWarning($"Invalid equipment inventory index for {memberSave.characterName}");
                    continue;
                }

                EquipmentInventoryObject inventoryObject = characterInventoryObject[memberSave.equipmentInventoryIndex];
                Character loadedCharacter = new Character(
                    memberSave.characterName,
                    (ClassType)memberSave.classType,
                    inventoryObject
                );

                ApplyBaseStatsByClass(loadedCharacter);

                Stats loadedBaseStats = ConvertSaveDataToStats(memberSave.baseStats);
                if (loadedBaseStats != null)
                    loadedCharacter.baseStats.CopyFrom(loadedBaseStats);

                Stats loadedActiveCardStats = ConvertSaveDataToStats(memberSave.activeCardBonusStats);
                if (loadedActiveCardStats != null)
                    loadedCharacter.activeCardBonusStats.CopyFrom(loadedActiveCardStats);

                loadedCharacter.Initialize();

                if (loadedCharacter.health != null)
                {
                    int clampedHealth = Mathf.Clamp(memberSave.currentHealth, 1, loadedCharacter.health.maxHealth);
                    loadedCharacter.health.SetCurrentHealth(clampedHealth);
                    loadedCharacter.health.isDead = false;
                }

                if (loadedCharacter.shield != null)
                {
                    int clampedShield = Mathf.Clamp(memberSave.currentShield, 0, loadedCharacter.shield.maxShield);
                    loadedCharacter.shield.SetCurrentShield(clampedShield);
                }

                AddCharacterToSlot(loadedCharacter, memberSave.slotIndex);
            }
        }

        RebuildAvailableDeadInventories();
        PrintParty();
        OnPartyChanged?.Invoke();
        return true;
    }

    private StatsSaveData ConvertStatsToSaveData(Stats stats)
    {
        StatsSaveData saveData = new StatsSaveData();

        if (stats == null)
            return saveData;

        saveData.health = stats.health;
        saveData.armor = stats.armor;
        saveData.shield = stats.shield;
        saveData.strength = stats.strength;
        saveData.agility = stats.agility;
        saveData.intelligence = stats.intelligence;
        saveData.minDamage = stats.minDamage;
        saveData.maxDamage = stats.maxDamage;
        saveData.critChance = stats.critChance;
        saveData.critDamage = stats.critDamage;

        return saveData;
    }

    private Stats ConvertSaveDataToStats(StatsSaveData saveData)
    {
        Stats stats = new Stats();

        if (saveData == null)
            return stats;

        stats.health = saveData.health;
        stats.armor = saveData.armor;
        stats.shield = saveData.shield;
        stats.strength = saveData.strength;
        stats.agility = saveData.agility;
        stats.intelligence = saveData.intelligence;
        stats.minDamage = saveData.minDamage;
        stats.maxDamage = saveData.maxDamage;
        stats.critChance = saveData.critChance;
        stats.critDamage = saveData.critDamage;

        return stats;
    }

    public void CompactParty()
    {
        Character[] newSlots = new Character[partySlots.Length];
        int writeIndex = 0;

        for (int i = 0; i < partySlots.Length; i++)
        {
            Character character = partySlots[i];

            if (character == null)
                continue;

            if (character.health != null && character.health.isDead)
                continue;

            newSlots[writeIndex] = character;
            newSlots[writeIndex].position = writeIndex + 1;
            writeIndex++;
        }

        partySlots = newSlots;
    }

    public int GetPartySize()
    {
        int count = 0;

        for (int i = 0; i < partySlots.Length; i++)
        {
            if (partySlots[i] != null)
                count++;
        }

        return count;
    }

    public int GetAlivePartySize()
    {
        int count = 0;

        for (int i = 0; i < partySlots.Length; i++)
        {
            Character character = partySlots[i];
            if (character == null || character.health == null || character.health.isDead)
                continue;

            count++;
        }

        return count;
    }

    public Character GetCharacterBySlotIndex(int position)
    {
        int index = position - 1;

        if (index < 0 || index >= partySlots.Length)
            return null;

        return partySlots[index];
    }

    public bool HasEmptySlot()
    {
        for (int i = 0; i < partySlots.Length; i++)
        {
            if (partySlots[i] == null)
                return true;
        }

        return false;
    }

    public void PrintParty()
    {
        Debug.Log("=== PARTY ===");

        for (int i = 0; i < partySlots.Length; i++)
        {
            Character c = partySlots[i];

            if (c == null)
                Debug.Log($"Slot {i + 1}: Empty");
            else
                Debug.Log($"Slot {i + 1}: {c.name} | {c.classType}");
        }
    }

    public bool IsPartyEmpty()
    {
        for (int i = 0; i < partySlots.Length; i++)
        {
            if (partySlots[i] != null)
                return false;
        }

        return true;
    }

    public void SwapCharacter(Character character1, Character character2)
    {
        Character temp = character1;
        character1 = character2;
        character2 = temp;
    }

    public void SwapCharactersByPosition(int pos1, int pos2)
    {
        if (pos1 < 1 || pos1 > partySlots.Length || pos2 < 1 || pos2 > partySlots.Length)
        {
            Debug.LogWarning("Invalid positions for swapping.");
            return;
        }

        int index1 = pos1 - 1;
        int index2 = pos2 - 1;

        Character temp = partySlots[index1];
        partySlots[index1] = partySlots[index2];
        partySlots[index2] = temp;

        if (partySlots[index1] != null)
            partySlots[index1].position = index1 + 1;

        if (partySlots[index2] != null)
            partySlots[index2].position = index2 + 1;

        Debug.Log($"Swapped characters in positions {pos1} and {pos2}.");
        OnPartyChanged?.Invoke();
    }

    public Character[] GetPartyMembers()
    {
        return partySlots;
    }

    public List<Character> GetAliveMembers()
    {
        List<Character> result = new List<Character>();

        for (int i = 0; i < partySlots.Length; i++)
        {
            Character character = partySlots[i];

            if (character == null || character.health == null || character.health.isDead)
                continue;

            result.Add(character);
        }

        return result;
    }

    public List<Character> GetAllMembersForLog()
    {
        List<Character> result = new List<Character>();

        for (int i = 0; i < partySlots.Length; i++)
        {
            if (partySlots[i] != null)
                result.Add(partySlots[i]);
        }

        return result;
    }

    public List<Character> GetAliveMembersInRange(int range)
    {
        List<Character> aliveMembers = GetAliveMembers();
        List<Character> result = new List<Character>();

        int clampedRange = Mathf.Clamp(range, 1, 99);

        for (int i = 0; i < aliveMembers.Count; i++)
        {
            if (i >= clampedRange)
                break;

            result.Add(aliveMembers[i]);
        }

        return result;
    }

    public Character GetFrontAliveMember()
    {
        List<Character> aliveMembers = GetAliveMembers();
        return aliveMembers.Count > 0 ? aliveMembers[0] : null;
    }

    public Character GetFrontAliveMemberInRange(int range)
    {
        List<Character> targets = GetAliveMembersInRange(range);
        return targets.Count > 0 ? targets[0] : null;
    }

    public Character GetRandomAliveMemberInRange(int range)
    {
        List<Character> targets = GetAliveMembersInRange(range);

        if (targets.Count == 0)
            return null;

        int randomIndex = UnityEngine.Random.Range(0, targets.Count);
        return targets[randomIndex];
    }

    public bool AreAllDead()
    {
        return GetAliveMembers().Count == 0;
    }

    public void ClearParty(bool invokeEvent = true)
    {
        for (int i = 0; i < partySlots.Length; i++)
        {
            if (partySlots[i] != null && partySlots[i].health != null)
                partySlots[i].health.OnDeath -= OnAnyCharacterDeath;

            partySlots[i] = null;
        }

        availableDeadInventories.Clear();

        if (invokeEvent)
            OnPartyChanged?.Invoke();
    }

    public void NotifyPartyChanged()
    {
        OnPartyChanged?.Invoke();
    }
}