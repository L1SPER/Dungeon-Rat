using UnityEngine;
using System.Collections.Generic;
using System;
public class PartyManager : MonoBehaviour
{
    private Character[] partySlots = new Character [3];
    private void Start()
    {
        CreateStartingParty();    
    }
    
    public void CreateStartingParty()
    {
        if(!IsPartyEmpty())
            return;

        Character warrior = new Character(
            "Borin",
            ClassType.Warrior,
            new Weapon("Basic Sword", WeaponType.Sword, 5, 15, 1)
        );

        Character archer = new Character(
            "Lira",
            ClassType.Archer,
            new Weapon("Basic Bow", WeaponType.Bow, 4, 12, 3)
        );

        Character mage = new Character(
            "Mira",
            ClassType.Mage,
            new Weapon("Basic Wand", WeaponType.Wand, 2, 8, 2)
        );

        AddCharacter(warrior);
        AddCharacter(archer);
        AddCharacter(mage);

        Debug.Log("Starting party is created!");
        PrintParty();
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

    public Character GetCharacter(int position)
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
                Debug.Log($"Slot {i + 1}: {c.name} | {c.classType} | {c.weapon.weaponName}");
        }
    }

    public bool IsPartyEmpty()
    {
        for (int i = 0; i < partySlots.Length; i++)
        {
            if (partySlots[i] != null)
            {
                return false;
            }
        }
        return true;
    }

    public Character[] GetPartyMembers()
    {
        return partySlots;
    }
}
