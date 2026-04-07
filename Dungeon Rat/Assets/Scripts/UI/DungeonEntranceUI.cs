using Unity.VisualScripting;
using UnityEngine;

public class DungeonEntranceUI : MonoBehaviour
{
    [SerializeField] private GameObject partyPreviewPanel;
    [SerializeField] private GameObject dungeonEntrancePanel;
    [SerializeField] private PartyPreviewUI partyPreviewUI;
    [SerializeField] private InventoryUI inventoryUI;

    private void Start()
    {
        dungeonEntrancePanel.SetActive(true);
        partyPreviewPanel.SetActive(false);

        SaveSystemManager.Instance?.LoadGame();
    }

    public void GoBackTown()
    {
        SaveSystemManager.Instance?.SaveGame();
        GameSceneManager.Instance.LoadScene("Town");
    }

    public void EnterDungeon()
    {
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
