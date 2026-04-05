using UnityEngine;

public class AfterRoomCanvasUI : MonoBehaviour
{
    [SerializeField] private LootInventoryUI lootInventoryUI;
    [SerializeField] private DungeonRoomFlowController roomFlowController;
    [SerializeField] private RatInventoryUI ratInventoryUI;

    public void RefreshUI()
    {
        ratInventoryUI?.RefreshUI();
    }

    public void OpenChestLoot()
    {
        if (lootInventoryUI == null)
        {
            Debug.LogWarning("LootInventoryUI referansı eksik.");
            return;
        }

        lootInventoryUI.gameObject.SetActive(true);
        lootInventoryUI.OpenAndRefresh();
    }

    public void OnClickNextRoom()
    {
        if (roomFlowController == null)
        {
            Debug.LogWarning("DungeonRoomFlowController referansı eksik.");
            return;
        }

        roomFlowController.GoToNextRoom();
    }
}