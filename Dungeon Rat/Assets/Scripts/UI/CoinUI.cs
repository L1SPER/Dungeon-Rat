using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinText;
    private void OnEnable()
    {
        UpdateCoinText();
    }
    public void UpdateCoinText()
    {
        int currentCoins = CoinManager.Instance.CurrentCoins;
        coinText.text = $"Coins: {currentCoins}";
    }
}
