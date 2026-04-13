using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }
    private int currentCoins = 0;

    public int CurrentCoins => currentCoins;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        Debug.Log($"Added {amount} coins. Total coins: {currentCoins}");
    }

    public void SpendCoins(int amount)
    {
        if (amount < 0|| currentCoins < amount)
        {
            Debug.LogWarning("Cannot spend a negative amount of coins.");
            return;
        }

        currentCoins -= amount;
    }
    
    public bool HasEnoughCoins(int amount)
    {
        return currentCoins >= amount;
    }
    
    [ContextMenu("Add 1000 Coins")]
    public void AddTestCoins()
    {
        AddCoins(1000);
    }
    public void ResetCoins()
    {
        currentCoins = 0;
    }
    public void SetCoins(int amount)
    {
        currentCoins = Mathf.Max(0, amount);
    }
}