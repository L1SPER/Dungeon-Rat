using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image bgImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;

    private InventorySlot currentSlot;
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
        }
        else
        {
            iconImage.sprite = currentSlot.item.itemData.icon;
            iconImage.enabled = true;
            amountText.text = currentSlot.item.amount > 0 ? currentSlot.item.amount.ToString() : "";

            if(currentSlot.isLocked)
            {
                bgImage.color = Color.red; // Locked slot color
            }
            else
            {
                bgImage.color = Color.white; // Normal slot color
            }
        }
    }
    public void ClearUI()
    {
        iconImage.sprite = null;
        iconImage.enabled = false;
        amountText.text = "";
    }
}
