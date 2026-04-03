using UnityEngine;

public class RestRoomCanvasUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DungeonRoomFlowController roomFlowController;
    [SerializeField] private PartyPreviewUI partyPreviewUI;

    private PartyManager partyManager;

    [Header("Rest Settings")]
    [SerializeField] private int healAmount;
    [SerializeField] private bool healOnlyOnce = true;

    private bool healUsed;

    private void Awake()
    {
        partyManager= FindFirstObjectByType<PartyManager>();
    }

    private void OnEnable()
    {
        healUsed = false;
    }

    public void OnClickHeal()
    {
        if (healOnlyOnce && healUsed)
        {
            Debug.Log("Bu odada heal zaten kullanıldı.");
            return;
        }

        if (partyManager == null)
        {
            Debug.LogWarning("PartyManager bulunamadı.");
            return;
        }

        Character[] partyMembers = partyManager.GetPartyMembers();

        if (partyMembers == null)
            return;

        for (int i = 0; i < partyMembers.Length; i++)
        {
            Character character = partyMembers[i];

            if (character == null || character.health == null || character.health.isDead)
                continue;

            character.Heal(healAmount);
        }

        healUsed = true;
        partyManager.NotifyPartyChanged();
        partyPreviewUI.RefreshSlots();

        Debug.Log($"Rest room heal uygulandı. Heal Amount: {healAmount}");
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