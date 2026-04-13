using UnityEngine;
using UnityEngine.UI;

public class ConsumableItemContextMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button useButton;

    private ConsumableItemData pendingItem;
    private InventorySlot pendingSlot;

    private InventoryBase pendingInventory;
    private int pendingSlotIndex;
    private PartyPreviewUI partyPreviewUI;

    private static ConsumableItemContextMenu _instance;
    public static ConsumableItemContextMenu Instance => _instance;

    private void Awake()
    {
        _instance = this;
        panel.SetActive(false);
        useButton.onClick.AddListener(OnUseClicked);
    }

    public void Show(ConsumableItemData item, InventorySlot slot, InventoryBase inventory, int slotIndex, PartyPreviewUI partyUI)
    {
        pendingItem = item;
        pendingSlot = slot;
        pendingInventory = inventory;
        pendingSlotIndex = slotIndex;
        partyPreviewUI = partyUI;

        panel.SetActive(true);
        transform.position = Input.mousePosition;
    }

    public void Hide()
    {
        panel.SetActive(false);
        pendingItem = null;
    }

    private void OnUseClicked()
    {
        if (pendingItem == null || partyPreviewUI == null) { Hide(); return; }

        Character target = partyPreviewUI.GetSelectedCharacter();
        if (target == null)
        {
            Debug.LogWarning("Önce bir karakter seç!");
            return;
        }

        if (target.health.currentHealth >= target.health.maxHealth)
        {
            Debug.LogWarning($"{target.name} zaten full can!");
            return;
        }

        target.Heal(pendingItem.healAmount);
        pendingSlot.RemoveAmount(1);

        partyPreviewUI.RefreshSlots();
        FindFirstObjectByType<InventoryUI>()?.RefreshUI();

        SaveSystemManager.Instance?.SaveGame();

        Hide();
    }
}