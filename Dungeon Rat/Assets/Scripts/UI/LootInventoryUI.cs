using UnityEngine;

public class LootInventoryUI : MonoBehaviour
{
    [Header("Inventory Data")]
    [SerializeField] private LootInventoryObject lootInventoryObject;

    [Header("UI")]
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform slotsParent;

    private InventorySlotUI[] slotUIArray;

    public void OpenAndRefresh()
    {
        if (slotUIArray == null || slotUIArray.Length == 0)
            CreateSlots();

        RefreshUI();
    }
    public void CreateSlots()
    {
        if (lootInventoryObject == null)
        {
            Debug.LogError("LootInventoryObject reference is missing.");
            return;
        }

        if (lootInventoryObject.inventory == null)
        {
            Debug.LogError("Loot inventory reference is missing.");
            return;
        }

        if (lootInventoryObject.GetSlots == null || lootInventoryObject.GetSlots.Length == 0)
        {
            Debug.LogError("Loot inventory has no slots.");
            return;
        }

        if (slotPrefab == null || slotsParent == null)
        {
            Debug.LogError("slotPrefab or slotsParent is missing.");
            return;
        }

        InventorySlot[] inventorySlots = lootInventoryObject.GetSlots;
        slotUIArray = new InventorySlotUI[inventorySlots.Length];

        for (int i = 0; i < slotUIArray.Length; i++)
        {
            InventorySlotUI newSlotUI = Instantiate(slotPrefab, slotsParent);
            newSlotUI.Bind(lootInventoryObject.inventory, i, null, null, null, this);
            slotUIArray[i] = newSlotUI;
        }
    }

    public void RefreshUI()
    {
        if (slotUIArray == null || lootInventoryObject == null)
        {
            Debug.LogError("Cannot refresh Loot UI: slotUIArray or lootInventoryObject is null.");
            return;
        }

        InventorySlot[] slots = lootInventoryObject.GetSlots;
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            slotUIArray[i].Bind(lootInventoryObject.inventory, i, null, null, null, this);
        }
    }
}
