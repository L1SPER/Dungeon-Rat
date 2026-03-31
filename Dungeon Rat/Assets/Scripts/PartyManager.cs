using UnityEngine;
using System;

public class PartyManager : MonoBehaviour
{
    private Character[] partySlots = new Character[3];

    [SerializeField] private EquipmentInventoryObject[] characterInventoryObject = new EquipmentInventoryObject[3];
    [SerializeField] private EquipmentItemData[] weapons = new EquipmentItemData[3];

    private void Start()
    {
        CreateStartingParty();
    }

    public void CreateStartingParty()
    {
        if (!IsPartyEmpty())
            return;

        Character warrior = new Character(
            "Borin",
            ClassType.Warrior,
            characterInventoryObject[0]
        );

        Character archer = new Character(
            "Lira",
            ClassType.Archer,
            characterInventoryObject[1]
        );

        Character mage = new Character(
            "Mira",
            ClassType.Mage,
            characterInventoryObject[2]
        );

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

        character.RecalculateStats();
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

                Debug.Log($"{character.name} added party. Position: {character.position}");
                return true;
            }
        }

        Debug.Log("Parti dolu.");
        return false;
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
    }

    public Character[] GetPartyMembers()
    {
        return partySlots;
    }
}