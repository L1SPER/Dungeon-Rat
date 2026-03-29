using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour,
    IPointerClickHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    [SerializeField] private Image bgImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;

    private InventorySlot currentSlot;
    private InventoryBase ownerInventory;
    private int slotIndex = -1;

    private InventoryUI ownerInventoryUI;
    private EquipmentInventoryUI ownerEquipmentUI;

    public InventorySlot CurrentSlot => currentSlot;
    public int SlotIndex => slotIndex;
    public InventoryBase OwnerInventory => ownerInventory;

    public void Bind(InventoryBase inventory, int index, InventoryUI inventoryUI = null, EquipmentInventoryUI equipmentUI = null)
    {
        ownerInventory = inventory;
        slotIndex = index;
        ownerInventoryUI = inventoryUI;
        ownerEquipmentUI = equipmentUI;

        if (ownerInventory != null && ownerInventory.IsValidIndex(index))
            currentSlot = ownerInventory.GetSlot(index);
        else
            currentSlot = null;

        UpdateUI();
    }

    public void SetSlot(InventorySlot slot)
    {
        currentSlot = slot;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (currentSlot == null || currentSlot.IsEmpty())
        {
            ClearUI();
            return;
        }

        iconImage.sprite = currentSlot.item.itemData.icon;
        iconImage.enabled = true;

        if (currentSlot.item.itemData != null && currentSlot.item.itemData.IsStackable && currentSlot.item.amount > 1)
            amountText.text = currentSlot.item.amount.ToString();
        else
            amountText.text = "";

        bgImage.color = GetSlotBackgroundColor();
    }

    private Color GetSlotBackgroundColor()
    {
        if (currentSlot != null && currentSlot.isOverflowSlot)
            return Color.grey;

        if (currentSlot == null || currentSlot.IsEmpty() || currentSlot.item.itemData == null)
            return Color.white;

        switch (currentSlot.item.itemData.itemRarity)
        {
            case ItemRarity.Common:
                return Color.white;

            case ItemRarity.Uncommon:
                return Color.green;

            case ItemRarity.Rare:
                return Color.blue;

            case ItemRarity.Epic:
                return new Color(0.6f, 0.2f, 0.8f); // Mor

            case ItemRarity.Legendary:
                return Color.yellow;

            default:
                return Color.white;
        }
    }

    public void ClearUI()
    {
        iconImage.sprite = null;
        iconImage.enabled = false;
        amountText.text = "";
        bgImage.color = (currentSlot != null && currentSlot.isOverflowSlot) ? Color.grey : Color.white;
    }

    public void Refresh()
    {
        UpdateUI();
    }

    public void RefreshOwnerUI()
    {
        if (ownerInventoryUI != null)
            ownerInventoryUI.RefreshUI();

        if (ownerEquipmentUI != null)
            ownerEquipmentUI.RefreshUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ownerInventory == null || currentSlot == null)
            return;

        bool altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

        if (eventData.button == PointerEventData.InputButton.Left && altPressed)
        {
            if (ownerInventory is ContainerInventoryBase containerInventory)
            {
                bool success = containerInventory.SplitSlotInHalfToFirstValidEmptySlot(slotIndex);

                if (success)
                    RefreshOwnerUI();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (currentSlot == null || currentSlot.IsEmpty())
            return;

        InventoryDragManager.Instance.BeginDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Drag icon manager Update içinde takip ediyor.
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (InventoryDragManager.Instance != null && InventoryDragManager.Instance.IsDragging)
        {
            InventoryDragManager.Instance.CompleteDrag();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryDragManager dragManager = InventoryDragManager.Instance;

        if (dragManager == null || !dragManager.IsDragging)
            return;

        InventorySlotUI sourceSlotUI = dragManager.SourceSlotUI;

        if (sourceSlotUI == null || sourceSlotUI == this)
        {
            dragManager.CompleteDrag();
            return;
        }

        if (sourceSlotUI.OwnerInventory == null || ownerInventory == null)
        {
            dragManager.CompleteDrag();
            return;
        }

        InventorySlot sourceSlot = sourceSlotUI.OwnerInventory.GetSlot(sourceSlotUI.SlotIndex);
        InventorySlot targetSlot = ownerInventory.GetSlot(slotIndex);

        bool success = InventoryTransferUtility.MoveOrMergeOrSwap(sourceSlot, targetSlot);

        if (success)
        {
            sourceSlotUI.RefreshOwnerUI();
            RefreshOwnerUI();
        }

        dragManager.CompleteDrag();
    }
}