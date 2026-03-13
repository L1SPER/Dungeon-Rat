using UnityEngine;

public class PartyPreviewUI : MonoBehaviour
{
    [Header("Slot UIs")]
    [SerializeField] private CharacterSlotUI slot1UI;
    [SerializeField] private CharacterSlotUI slot2UI;
    [SerializeField] private CharacterSlotUI slot3UI;

    private PartyManager partyManager;
    private void Awake()
    {
        partyManager=FindFirstObjectByType<PartyManager>();

        if (partyManager == null)
        {
            Debug.LogError("Couldn`t find PartyManager !!!");
        }
    }
    public void RefreshSlots()
    {
        Character[] party = partyManager.GetPartyMembers();

        slot1UI.SetCharacter(party[0]);
        slot2UI.SetCharacter(party[1]);
        slot3UI.SetCharacter(party[2]);
    }
}