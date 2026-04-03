using UnityEngine;

public class RatInventoryUI : MonoBehaviour
{
    [Header("Inventory Data")]
    [SerializeField] private RatInventoryObject ratInventoryObject;

    [Header("UI")]
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform slotsParent;

    private InventorySlotUI[] slotUIArray;

    private void Start()
    {
        if (ratInventoryObject == null)
        {
            Debug.LogError("RatInventoryObject reference is missing.");
            return;
        }

        if (ratInventoryObject.inventory == null)
        {
            Debug.LogError("RatInventory reference is missing. Rat Inventory asset içindeki inventory null.");
            return;
        }

        if (ratInventoryObject.GetSlots == null || ratInventoryObject.GetSlots.Length == 0)
        {
            Debug.LogError("Rat inventory has no slots. Asset içinde level değiştirip OnValidate tetiklenmiş olmalı.");
            return;
        }

        CreateSlots();
        RefreshUI();
    }

    private void CreateSlots()
    {
        if (slotPrefab == null || slotsParent == null)
        {
            Debug.LogError("slotPrefab or slotsParent is missing.");
            return;
        }

        InventorySlot[] inventorySlots = ratInventoryObject.GetSlots;
        slotUIArray = new InventorySlotUI[inventorySlots.Length];

        for (int i = 0; i < slotUIArray.Length; i++)
        {
            InventorySlotUI newSlotUI = Instantiate(slotPrefab, slotsParent);
            newSlotUI.Bind(ratInventoryObject.inventory, i, null, null, this);
            slotUIArray[i] = newSlotUI;
        }
    }

    public void RefreshUI()
    {
        if (slotUIArray == null || ratInventoryObject == null)
            return;

        InventorySlot[] slots = ratInventoryObject.GetSlots;
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            slotUIArray[i].Bind(ratInventoryObject.inventory, i, null, null, this);
        }
    }
}