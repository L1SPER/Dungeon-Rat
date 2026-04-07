using System.Collections.Generic;

[System.Serializable]
public class DungeonRun
{
    public string dungeonId;
    public DungeonTier tier;
    public int totalRooms;
    public int currentRoomIndex;
    public bool anyCharacterDied;
    public bool isCompleted;

    public List<DungeonRoomData> rooms = new List<DungeonRoomData>();

    public DungeonRun(string dungeonId, DungeonTier tier)
    {
        this.dungeonId = string.IsNullOrWhiteSpace(dungeonId) ? "CommonDungeon" : dungeonId;
        this.tier = tier;
        totalRooms = DungeonRules.GetRoomCount(tier);
        currentRoomIndex = 0;
        anyCharacterDied = false;
        isCompleted = false;
    }

    public bool IsCurrentRoomRestRoom()
    {
        return DungeonRules.IsRestRoom(currentRoomIndex, totalRooms);
    }

    public bool HasNextRoom()
    {
        return currentRoomIndex < totalRooms - 1;
    }

    public void MoveNextRoom()
    {
        if (HasNextRoom())
            currentRoomIndex++;
        else
            isCompleted = true;
    }

    public DungeonRoomData GetCurrentRoomData()
    {
        if (rooms == null || currentRoomIndex < 0 || currentRoomIndex >= rooms.Count)
            return null;

        return rooms[currentRoomIndex];
    }
}