using UnityEngine;

public class PartyPreviewUI : MonoBehaviour
{
    [Header("Slot UIs")]
    [SerializeField] private CharacterSlotUI [] slotsUI= new CharacterSlotUI[3];

    private PartyManager partyManager;

    private int selectedSlotIndex = -1;

    private void Awake()
    {
        partyManager=FindFirstObjectByType<PartyManager>();

        if (partyManager == null)
        {
            Debug.LogError("Couldn`t find PartyManager !!!");
        }
    }
    private void Start()
    {
         SetupSlots();
         RefreshSlots();
    }

    private void SetupSlots()
    {
        for (int i = 0; i < slotsUI.Length; i++)
        {
            slotsUI[i].Setup(i+1, this);
        }
    }
    public void SelectSlot(int index)
    {
        Character selectedCharacter = partyManager.GetCharacterBySlotIndex(index);
        Debug.Log("Selected Slot: " + index);
        Debug.Log("Selected Character: " + (selectedCharacter == null ? "NULL" : selectedCharacter.name));

        if (selectedCharacter == null)
        {
            Debug.Log("No character in the selected slot.");
            return;
        }
        selectedSlotIndex = index;
        RefreshSlots();

        Debug.Log($"{selectedCharacter.name} seçildi.");
    }
    public void RefreshSlots()
    {
        Character[] party = partyManager.GetPartyMembers();

        Debug.Log("Party array geldi.");
        Debug.Log("Slot1: " + (party[2] == null ? "NULL" : party[2].name));
        Debug.Log("Slot2: " + (party[1] == null ? "NULL" : party[1].name));
        Debug.Log("Slot3: " + (party[0] == null ? "NULL" : party[0].name));

        for (int i = 0; i < party.Length; i++)
        {
            slotsUI[i].SetCharacter(party[i]);
            slotsUI[i].SetSelected(i+1 == selectedSlotIndex);
        }
    }
    public void ClearSelection()
    {
        selectedSlotIndex = -1;
        RefreshSlots();
    }
    public void SwapSlot2And3()
    {
        SwapSlotUI(2, 3);
    }
    public void SwapSlot1And2()
    {
        SwapSlotUI(1, 2);
    }
    public void SwapSlotUI(int pos1, int pos2)
    {
        partyManager.SwapCharactersByPosition(pos1, pos2);
        RefreshSlots();
    }
}