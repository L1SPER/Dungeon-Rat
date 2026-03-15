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
        // Update character positions
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
