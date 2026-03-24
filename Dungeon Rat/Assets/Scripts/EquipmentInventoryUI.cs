using UnityEngine;

public class EquipmentInventoryUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EquipmentInventoryObject equipmentInventoryObject;

    [Header("UI Slots")]
    [SerializeField] private InventorySlotUI[] slotUIs = new InventorySlotUI[8];

    private void Start()
    {
        if (equipmentInventoryObject == null || equipmentInventoryObject.inventory == null)
        {
            Debug.LogError("EquipmentInventoryObject or inventory is missing.");
            return;
        }

        BindSlots();
        RefreshUI();
    }

    public void BindSlots()
    {
        EquipmentInventory inventory = equipmentInventoryObject.inventory;

        if (slotUIs == null || slotUIs.Length != 8)
        {
            Debug.LogError("Equipment UI must have exactly 8 slot references.");
            return;
        }

        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (slotUIs[i] == null)
                continue;

            slotUIs[i].SetSlot(inventory.GetSlot(i));
        }
    }

    public void RefreshUI()
    {
        if (equipmentInventoryObject == null || equipmentInventoryObject.inventory == null)
            return;

        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (slotUIs[i] == null)
                continue;

            slotUIs[i].SetSlot(equipmentInventoryObject.inventory.GetSlot(i));
        }
    }
}