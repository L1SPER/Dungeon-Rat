using Unity.VisualScripting;
using UnityEngine;

public class DungeonEntranceUI : MonoBehaviour
{
    [SerializeField] private GameObject partyPreviewPanel;
    [SerializeField] private GameObject dungeonEntrancePanel;
    [SerializeField] private PartyPreviewUI partyPreviewUI;
    [SerializeField] private InventoryUI inventoryUI;

    private PartyManager partyManager;

    private void Awake()
    {
        partyManager = FindFirstObjectByType<PartyManager>();
    }

    private void Start()
    {
        dungeonEntrancePanel.SetActive(true);
        partyPreviewPanel.SetActive(false);
    }

    public void GoBackTown()
    {
        SaveSystemManager.Instance?.SaveGame();
        GameSceneManager.Instance.LoadScene("Town");
    }

    public void EnterDungeon()
    {
        if (partyManager == null)
            partyManager = FindFirstObjectByType<PartyManager>();

        if (partyManager == null || partyManager.GetAlivePartySize() <= 0)
        {
            Debug.LogWarning("Partide hiç yaşayan karakter yok. Dungeon'a girilemez.");
            return;
        }

        if (DungeonManager.Instance != null && !DungeonManager.Instance.HasActiveRun)
            DungeonManager.Instance.StartDungeon();

        SaveSystemManager.Instance?.SaveGame();
        GameSceneManager.Instance.LoadScene("Dungeon");
    }

    public void OpenPartyPanel()
    {
        dungeonEntrancePanel.SetActive(false);
        partyPreviewPanel.SetActive(true);

        partyPreviewUI.RefreshSlots();
        inventoryUI.RefreshUI();
    }

    public void ClosePartyPanel()
    {
        partyPreviewPanel.SetActive(false);
        dungeonEntrancePanel.SetActive(true);
    }
}