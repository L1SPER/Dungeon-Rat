using UnityEngine;

public class PartyPreviewUI : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] private GameObject[] characters = new GameObject[3];

    private PartyManager partyManager;

    private int selectedSlotIndex = -1;

    private void Awake()
    {
        partyManager = FindFirstObjectByType<PartyManager>();

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
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].transform.GetChild(0).GetComponent<CharacterSlotUI>().Setup(i + 1, this);
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
            characters[i].transform.GetChild(0).GetComponent<CharacterSlotUI>().SetCharacter(party[i]);
            characters[i].transform.GetChild(1).GetComponent<CharacterStatsUI>().SetStats(party[i]);
            characters[i].transform.GetChild(2).GetComponent<EquipmentInventoryUI>().SetInventory(party[i]?.CharacterInventoryObject);
            characters[i].transform.GetChild(0).GetComponent<CharacterSlotUI>().SetSelected(i + 1 == selectedSlotIndex);
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
    public void OpenStatsPanel(int slotIndex)
    {
        RefreshSlots();
        characters[slotIndex-1].transform.GetChild(0).gameObject.SetActive(false);
        characters[slotIndex-1].transform.GetChild(1).gameObject.SetActive(true);
    }
    public void CloseStatsPanel(int slotIndex)
    {
        RefreshSlots();
        characters[slotIndex - 1].transform.GetChild(0).gameObject.SetActive(true);
        characters[slotIndex - 1].transform.GetChild(1).gameObject.SetActive(false);
    }
    public void OpenEquipmentPanel(int slotIndex)
    {
        RefreshSlots();
        characters[slotIndex - 1].transform.GetChild(0).gameObject.SetActive(false);
        characters[slotIndex - 1].transform.GetChild(2).gameObject.SetActive(true);
    }
    public void CloseEquipmentPanel(int slotIndex)
    {
        RefreshSlots();
        characters[slotIndex - 1].transform.GetChild(0).gameObject.SetActive(true);
        characters[slotIndex - 1].transform.GetChild(2).gameObject.SetActive(false);
    }    
}