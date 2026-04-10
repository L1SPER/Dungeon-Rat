using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public List<InventorySaveData> inventories = new List<InventorySaveData>();
    public DungeonProgressionSaveData dungeonProgression = new DungeonProgressionSaveData();
    public DungeonRunSaveData activeDungeonRun = new DungeonRunSaveData();
    public PartySaveData party = new PartySaveData();
    public string lastSceneName;
    public string savedAtUtc;
}

[Serializable]
public class InventorySaveData
{
    public string inventoryKey;
    public int level;
    public int currentCapacity;
    public int maxCapacity;
    public bool useOverflowSlots;
    public List<InventorySlotSaveData> slots = new List<InventorySlotSaveData>();
}

[Serializable]
public class InventorySlotSaveData
{
    public int slotIndex;
    public int itemId;
    public int amount;
    public bool isOverflowSlot;
    public int allowedEquipmentType;
}

[Serializable]
public class DungeonProgressionSaveData
{
    public int currentTier;
    public int flawlessClearStreak;
}

[Serializable]
public class DungeonRunSaveData
{
    public bool hasActiveRun;
    public string dungeonId;
    public int tier;
    public int totalRooms;
    public int currentRoomIndex;
    public bool anyCharacterDied;
    public bool isCompleted;
}

[Serializable]
public class PartySaveData
{
    public bool hasReceivedStartingParty;
    public List<CharacterSaveData> members = new List<CharacterSaveData>();
}

[Serializable]
public class CharacterSaveData
{
    public string characterName;
    public int classType;
    public int slotIndex;
    public int equipmentInventoryIndex;

    public int currentHealth;
    public int currentShield;

    public StatsSaveData baseStats = new StatsSaveData();
    public StatsSaveData activeCardBonusStats = new StatsSaveData();
}

[Serializable]
public class StatsSaveData
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
    public int critDamage;
}