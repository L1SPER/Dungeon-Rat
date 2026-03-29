 using UnityEngine;

public static class DungeonRules
{
    public static int GetRoomCount(DungeonTier tier)
    {
        switch (tier)
        {
            case DungeonTier.Common: return 5;
            case DungeonTier.Uncommon: return 7;
            case DungeonTier.Rare: return 9;
            case DungeonTier.Epic: return 11;
            case DungeonTier.Legendary: return 13;
            default: return 5;
        }
    }

    public static DungeonTier IncreaseTier(DungeonTier tier)
    {
        if (tier == DungeonTier.Legendary)
            return DungeonTier.Legendary;

        return tier + 1;
    }

    public static DungeonTier DecreaseTier(DungeonTier tier)
    {
        if (tier == DungeonTier.Common)
            return DungeonTier.Common;

        return tier - 1;
    }

    public static bool IsRestRoom(int roomIndex, int totalRoomCount)
    {
        return roomIndex == totalRoomCount / 2;
    }
}