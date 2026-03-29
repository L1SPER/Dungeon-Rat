using UnityEngine;

public class DungeonTester : MonoBehaviour
{
    [SerializeField] private DungeonManager dungeonManager;

    [ContextMenu("Start Dungeon")]
    public void StartDungeon()
    {
        dungeonManager.StartDungeon();
    }

    [ContextMenu("Complete Current Room")]
    public void CompleteCurrentRoom()
    {
        dungeonManager.CompleteCurrentRoom();
    }

    [ContextMenu("Mark Character Death")]
    public void MarkCharacterDeath()
    {
        dungeonManager.MarkCharacterDeath();
    }

    [ContextMenu("Reset Dungeon Progress")]
    public void ResetProgress()
    {
        dungeonManager.ResetDungeonProgress();
    }
}