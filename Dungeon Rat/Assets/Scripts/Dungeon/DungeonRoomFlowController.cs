using UnityEngine;

public class DungeonRoomFlowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DungeonRoomCanvasManager canvasManager;
    [SerializeField] private BattleTurnManager battleTurnManager;
    [SerializeField] private LootInventoryUI lootInventoryUI;
    [SerializeField] private LootInventoryObject lootInventoryObject;
    [SerializeField] private GameObject restRoomCanvas;
    private EnemyPartyManager enemyPartyManager;

    private void Awake()
    {
        enemyPartyManager = FindFirstObjectByType<EnemyPartyManager>(); 
    }
    private void Start()
    {
        EnterCurrentRoom();
    }

    public void EnterCurrentRoom()
    {
        if (DungeonManager.Instance == null || !DungeonManager.Instance.HasActiveRun)
        {
            Debug.LogWarning("Aktif dungeon run bulunamadı.");
            return;
        }

        DungeonRoomData roomData = DungeonManager.Instance.GetCurrentRoomData();

        if (roomData == null)
        {
            Debug.LogWarning("Current room data bulunamadı.");
            return;
        }
        
        if (roomData.isRestRoom)
            EnterRestRoom();
        else
            EnterBattleRoom();
    }

    public void GoToNextRoom()
    {
        if (DungeonManager.Instance == null || !DungeonManager.Instance.HasActiveRun)
        {
            Debug.LogWarning("Aktif dungeon run bulunamadı.");
            return;
        }

        DungeonManager.Instance.CompleteCurrentRoom();

        if (!DungeonManager.Instance.HasActiveRun)
        {
            Debug.Log("Dungeon tamamlandı. Burada istersen sonuç ekranı ya da town dönüşü açarsın.");

            if (canvasManager != null)
                canvasManager.HideAll();

            return;
        }

        EnterCurrentRoom();
    }

    public void EnterBattleRoom()
    {
        if (lootInventoryUI != null)
            lootInventoryUI.gameObject.SetActive(false);

        if (lootInventoryObject != null && lootInventoryObject.inventory != null)
            lootInventoryObject.inventory.Clear();

        if (enemyPartyManager != null)
            enemyPartyManager.ClearEnemyParty();

        if (canvasManager != null)
            canvasManager.ShowBattleUI();

        if (battleTurnManager != null)
            battleTurnManager.InitializeBattle();
        else
            Debug.LogWarning("BattleTurnManager referansı eksik.");
    }

    public void EnterRestRoom()
    {
        if (lootInventoryUI != null)
            lootInventoryUI.gameObject.SetActive(false);

        if (lootInventoryObject != null && lootInventoryObject.inventory != null)
            lootInventoryObject.inventory.Clear();

        if (enemyPartyManager != null)
            enemyPartyManager.ClearEnemyParty();

        if (canvasManager != null)
            canvasManager.ShowRestRoomUI();

        //if (battleTurnManager != null)
        //    battleTurnManager.InitializeBattle();
        //else
        //    Debug.LogWarning("BattleTurnManager referansı eksik.");
    }
}