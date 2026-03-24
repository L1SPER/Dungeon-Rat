using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image bgImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;

    private InventorySlot currentSlot;
    private ContainerInventoryBase ownerInventory;
    private int slotIndex = -1;

    public InventorySlot CurrentSlot => currentSlot;
    public int SlotIndex => slotIndex;

    public void Bind(ContainerInventoryBase inventory, int index)
    {
        ownerInventory = inventory;
        slotIndex = index;

        if (ownerInventory != null && ownerInventory.IsValidIndex(index))
            currentSlot = ownerInventory.GetSlot(index);
        else
            currentSlot = null;

        UpdateUI();
    }

    public void SetSlot(InventorySlot _slot)
    {
        currentSlot = _slot;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (currentSlot == null || currentSlot.IsEmpty())
        {
            ClearUI();
            return;
        }
        else
        {
            iconImage.sprite = currentSlot.item.itemData.icon;
            iconImage.enabled = true;
            if (currentSlot.item.itemData != null && currentSlot.item.itemData.IsStackable && currentSlot.item.amount > 1)
                amountText.text = currentSlot.item.amount.ToString();
            else
                amountText.text = "";

            bgImage.color = currentSlot.isOverflowSlot ? Color.grey : Color.white;
        }
    }

    public void ClearUI()
    {
        iconImage.sprite = null;
        iconImage.enabled = false;
        amountText.text = "";

        if (currentSlot != null && currentSlot.isOverflowSlot)
            bgImage.color = Color.grey;
        else
            bgImage.color = Color.white;
    }
    public void Refresh()
    {
        UpdateUI();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (ownerInventory == null || currentSlot == null)
            return;

        bool altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

        if (eventData.button == PointerEventData.InputButton.Left && altPressed)
        {
            bool success = ownerInventory.SplitSlotInHalfToFirstValidEmptySlot(slotIndex);

            if (success)
            {
                UpdateUI();

                // Eğer parent inventory UI scriptin varsa burada onu refresh etmen daha doğru olur.
                // Örnek:
                // inventoryUI.RefreshAllSlots();
            }
        }
    }

}