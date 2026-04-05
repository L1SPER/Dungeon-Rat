using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private DungeonProgression progression = new DungeonProgression();
    [SerializeField] private DungeonLayoutData commonLayout;

    private DungeonRun currentRun;

    public DungeonProgression Progression => progression;
    public DungeonRun CurrentRun => currentRun;

    public DungeonTier CurrentTier => progression.CurrentTier;
    public int CurrentRoomCount => progression.CurrentRoomCount;
    public int FlawlessClearStreak => progression.FlawlessClearStreak;

    public bool HasActiveRun => currentRun != null && !currentRun.isCompleted;

    public static DungeonManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartDungeon()
    {
        if (HasActiveRun)
        {
            Debug.LogWarning("Zaten aktif bir dungeon run var.");
            return;
        }

        currentRun = new DungeonRun(progression.CurrentTier);

        BuildRoomsFromLayout(currentRun, commonLayout);

        Debug.Log(
            $"Dungeon Started | Tier: {currentRun.tier} | Total Rooms: {currentRun.totalRooms} | Current Room: {currentRun.currentRoomIndex + 1}"
        );

        PrintCurrentRoomInfo();

        List<EnemyCharacter> enemyParty = CreateCurrentEnemyParty();
        PrintEnemyParty(enemyParty);
    }

    public void CompleteCurrentRoom()
    {
        if (currentRun == null)
        {
            Debug.LogWarning("Active dungeon run yok.");
            return;
        }

        if (currentRun.isCompleted)
        {
            Debug.LogWarning("Dungeon zaten tamamlanmış.");
            return;
        }

        if (currentRun.HasNextRoom())
        {
            currentRun.MoveNextRoom();
            PrintCurrentRoomInfo();

            List<EnemyCharacter> enemyParty = CreateCurrentEnemyParty();
            PrintEnemyParty(enemyParty);
        }
        else
        {
            currentRun.isCompleted = true;
            EndDungeon();
        }
    }

    public void MarkCharacterDeath()
    {
        if (currentRun == null)
            return;

        currentRun.anyCharacterDied = true;
    }

    public void ResetDungeonProgress()
    {
        progression.ResetProgress();
        currentRun = null;
    }

    public DungeonRoomData GetCurrentRoomData()
    {
        if (currentRun == null)
            return null;

        return currentRun.GetCurrentRoomData();
    }

    public bool IsCurrentRoomRestRoom()
    {
        if (currentRun == null)
            return false;

        DungeonRoomData roomData = currentRun.GetCurrentRoomData();
        return roomData != null && roomData.isRestRoom;
    }

    public List<EnemyCharacter> CreateCurrentEnemyParty()
    {
        if (currentRun == null)
            return null;

        DungeonRoomData roomData = currentRun.GetCurrentRoomData();

        if (roomData == null || roomData.isRestRoom || roomData.enemySetup == null)
            return null;

        return EnemyPartyBuilder.Build(roomData.enemySetup);
    }

    private void BuildRoomsFromLayout(DungeonRun run, DungeonLayoutData layout)
    {
        run.rooms.Clear();

        for (int i = 0; i < run.totalRooms; i++)
        {
            DungeonRoomData roomData = new DungeonRoomData();
            roomData.roomIndex = i;
            roomData.isRestRoom = DungeonRules.IsRestRoom(i, run.totalRooms);

            if (!roomData.isRestRoom && layout != null && i < layout.roomEnemySetups.Count)
            {
                roomData.enemySetup = layout.roomEnemySetups[i];
            }
            else
            {
                roomData.enemySetup = null;
            }

            run.rooms.Add(roomData);
        }
    }

    public void EndDungeon()
    {
        if (currentRun == null)
            return;

        progression.OnDungeonCompleted(currentRun.anyCharacterDied);
        GameSceneManager.Instance.LoadScene("AfterDungeon");

        Debug.Log(
            $"Dungeon Ended | Death Occurred: {currentRun.anyCharacterDied} | New Tier: {progression.CurrentTier} | Rooms: {progression.CurrentRoomCount} | Streak: {progression.FlawlessClearStreak}"
        );
    }

    private void PrintCurrentRoomInfo()
    {
        if (currentRun == null)
            return;

        string roomType = currentRun.IsCurrentRoomRestRoom() ? "Rest Room" : "Battle Room";

        Debug.Log(
            $"Room Entered | Room: {currentRun.currentRoomIndex + 1}/{currentRun.totalRooms} | Type: {roomType}"
        );
    }

    private void PrintEnemyParty(List<EnemyCharacter> enemyParty)
    {
        if (enemyParty == null || enemyParty.Count == 0)
        {
            Debug.Log($"Room {currentRun.currentRoomIndex + 1} enemy party: None");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.Append($"Room {currentRun.currentRoomIndex + 1} enemy party: ");

        for (int i = 0; i < enemyParty.Count; i++)
        {
            sb.Append($"{enemyParty[i].enemyData.enemyName} pos{enemyParty[i].position}");

            if (i < enemyParty.Count - 1)
                sb.Append(", ");
        }

        Debug.Log(sb.ToString());
    }
}