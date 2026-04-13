using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemBuyMenu : MonoBehaviour
{
    public GameObject panel;

    public Image itemImage;
    public TextMeshProUGUI itemAmountText;

    int currentAmount = 1;
    ItemData currentItem;

    public GeneralInventoryObject playerInventory;

    public void Open(ItemData item)
    {
        panel.SetActive(true);

        currentItem = item;
        currentAmount = 1;

        itemImage.sprite = item.icon;

        UpdateUI();
    }

    void UpdateUI()
    {
        itemAmountText.text = currentAmount.ToString();
    }

    int GetMaxAmount()
    {
        int coins = CoinManager.Instance.CurrentCoins;

        if (currentItem.price <= 0)
            return 1;

        int max = coins / currentItem.price;

        return Mathf.Max(1, max);
    }

    // --- BUTTONS ---

    public void OnMinus()
    {
        currentAmount--;

        if (currentAmount < 1)
            currentAmount = 1;

        UpdateUI();
    }

    public void OnPlus()
    {
        int max = GetMaxAmount();

        currentAmount++;

        if (currentAmount > max)
            currentAmount = max;

        UpdateUI();
    }

    public void OnMax()
    {
        currentAmount = GetMaxAmount();
        UpdateUI();
    }

    public void Buy()
    {
        int totalPrice = currentItem.price * currentAmount;

        if (!CoinManager.Instance.HasEnoughCoins(totalPrice))
            return;

        CoinManager.Instance.SpendCoins(totalPrice);
        playerInventory.inventory.AddItem(currentItem, currentAmount);
        SaveSystemManager.Instance?.SaveGame();
    }
}