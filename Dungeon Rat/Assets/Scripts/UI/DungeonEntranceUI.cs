using Unity.VisualScripting;
using UnityEngine;

public class DungeonEntranceUI : MonoBehaviour
{
    [SerializeField] private GameObject partyPreviewPanel;
    [SerializeField] private GameObject dungeonEntrancePanel;
    [SerializeField] private PartyPreviewUI partyPreviewUI;
    private void Start()
    {
        dungeonEntrancePanel.SetActive(true);
        partyPreviewPanel.SetActive(false);
    }
    public void GoBackTown()
    {
        GameSceneManager.Instance.LoadScene("Town");
    }

    public void EnterDungeon()
    {
        DungeonManager.Instance.StartDungeon();
        GameSceneManager.Instance.LoadScene("Dungeon");
    }

    public void OpenPartyPanel()
    {
        dungeonEntrancePanel.SetActive(false);
        partyPreviewPanel.SetActive(true);

        partyPreviewUI.RefreshSlots();
    }
    public void ClosePartyPanel()
    {
        partyPreviewPanel.SetActive(false);
        dungeonEntrancePanel.SetActive(true);
    }
}
