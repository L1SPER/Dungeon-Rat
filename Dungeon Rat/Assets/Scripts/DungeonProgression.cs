using UnityEngine;

public class DungeonProgression
{
    [SerializeField] private DungeonTier currentTier = DungeonTier.Common;
    [SerializeField] private int flawlessClearStreak = 0;

    public DungeonTier CurrentTier => currentTier;
    public int FlawlessClearStreak => flawlessClearStreak;
    public int CurrentRoomCount => DungeonRules.GetRoomCount(currentTier);

    public void OnDungeonCompleted(bool anyCharacterDied)
    {
        if (anyCharacterDied)
        {
            currentTier = DungeonRules.DecreaseTier(currentTier);
            flawlessClearStreak = 0;
            return;
        }

        flawlessClearStreak++;

        if (flawlessClearStreak >= 5)
        {
            currentTier = DungeonRules.IncreaseTier(currentTier);
            flawlessClearStreak = 0;
        }
    }

    public void ResetProgress()
    {
        currentTier = DungeonTier.Common;
        flawlessClearStreak = 0;
    }
}