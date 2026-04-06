using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventory Data")]
    [SerializeField] private GeneralInventoryObject inventoryObject;

    [Header("UI")]
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform slotsParent;

    private InventorySlotUI[] slotUIArray;

    private void OnEnable()
    {
        if (inventoryObject == null || inventoryObject.inventory == null)
            return;

        if (slotUIArray == null || slotUIArray.Length == 0)
            CreateSlots();

        RefreshUI();
    }
    private void Start()
    {
        if (inventoryObject == null)
        {
            Debug.LogError("InventoryObject reference is missing in InventoryUI.");
            return;
        }

        if (inventoryObject.inventory == null)
        {
            Debug.LogError("Inventory reference is missing in InventoryObject.");
            return;
        }

        if (inventoryObject.GetSlots == null || inventoryObject.GetSlots.Length == 0)
        {
            Debug.LogError("Inventory has no slots!");
            return;
        }

        if (slotUIArray == null || slotUIArray.Length == 0)
            CreateSlots();

        RefreshUI();
    }

    private void CreateSlots()
    {
        InventorySlot[] inventorySlots = inventoryObject.GetSlots;
        slotUIArray = new InventorySlotUI[inventorySlots.Length];

        for (int i = 0; i < slotUIArray.Length; i++)
        {
            InventorySlotUI newSlotUI = Instantiate(slotPrefab, slotsParent);
            newSlotUI.Bind(inventoryObject.inventory, i, this);
            slotUIArray[i] = newSlotUI;
        }
    }

    public void RefreshUI()
    {
        if (slotUIArray == null || inventoryObject == null)
        {
            Debug.LogError("Cannot refresh UI: slotUIArray or inventoryObject is null.");
            return;
        }

        InventorySlot[] slots = inventoryObject.GetSlots;
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            slotUIArray[i].Bind(inventoryObject.inventory, i, this, null);
        }
    }
}
