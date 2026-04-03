using UnityEngine;

public class RestRoomCanvasUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DungeonRoomFlowController roomFlowController;
    [SerializeField] private PartyManager partyManager;

    [Header("Rest Settings")]
    [SerializeField] private int healAmount = 25;
    [SerializeField] private bool healOnlyOnce = true;

    private bool healUsed;

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
            partyManager = FindFirstObjectByType<PartyManager>();

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

        Debug.Log($"Rest room heal uygulandı. Heal Amount: {healAmount}");
    }

    public void OnClickContinue()
    {
        if (roomFlowController == null)
        {
            Debug.LogWarning("DungeonRoomFlowController referansı eksik.");
            return;
        }

        roomFlowController.GoToNextRoom();
    }
}