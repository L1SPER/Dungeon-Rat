using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public List<InventorySaveData> inventories = new List<InventorySaveData>();
    public DungeonProgressionSaveData dungeonProgression = new DungeonProgressionSaveData();
    public DungeonRunSaveData activeDungeonRun = new DungeonRunSaveData();
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