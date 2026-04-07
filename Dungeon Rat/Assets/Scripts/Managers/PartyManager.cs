using UnityEngine;
using System;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour
{
    private Character[] partySlots = new Character[3];

    [SerializeField] private EquipmentInventoryObject[] characterInventoryObject = new EquipmentInventoryObject[3];
    [SerializeField] private EquipmentItemData[] weapons = new EquipmentItemData[3];

    public event Action OnPartyChanged;

    private void Start()
    {
        CreateStartingParty();
    }

    public void CreateStartingParty()
    {
        if (!IsPartyEmpty())
            return;

        Character warrior = new Character("Borin", ClassType.Warrior, characterInventoryObject[0]);
        Character archer = new Character("Lira", ClassType.Archer, characterInventoryObject[1]);
        Character mage = new Character("Mira", ClassType.Mage, characterInventoryObject[2]);

        ApplyStartingBaseStats(warrior);
        ApplyStartingBaseStats(archer);
        ApplyStartingBaseStats(mage);

        AddCharacter(warrior);
        AddCharacter(archer);
        AddCharacter(mage);

        GiveStartingWeapon(warrior, weapons[0]);
        GiveStartingWeapon(archer, weapons[1]);
        GiveStartingWeapon(mage, weapons[2]);

        warrior.Initialize();
        archer.Initialize();
        mage.Initialize();

        Debug.Log("Starting party is created!");
        PrintParty();
        OnPartyChanged?.Invoke();
    }

    private void ApplyStartingBaseStats(Character character)
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

    private void GiveStartingWeapon(Character character, EquipmentItemData weapon)
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
            {
                partySlots[i] = character;
                character.position = i + 1;

                RegisterCharacter(character);

                Debug.Log($"{character.name} added party. Position: {character.position}");
                OnPartyChanged?.Invoke();
                return true;
            }
        }

        Debug.Log("Parti dolu.");
        return false;
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
        ClearDeadCharactersInventories();
        CompactParty();
        PrintParty();
        OnPartyChanged?.Invoke();
    }

    private void ClearDeadCharactersInventories()
    {
        for (int i = 0; i < partySlots.Length; i++)
        {
            Character character = partySlots[i];

            if (character == null || character.health == null)
                continue;

            if (!character.health.isDead)
                continue;

            character.ClearEquipmentInventoryOnDeath();
        }
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

        if (invokeEvent)
            OnPartyChanged?.Invoke();
    }
    public void NotifyPartyChanged()
    {
        OnPartyChanged?.Invoke();
    }
}