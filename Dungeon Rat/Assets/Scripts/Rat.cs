using UnityEngine;

[System.Serializable]
public class Rat
{
    public string ratName;
    public int level = 1;

    public RatInventory inventory = new RatInventory();

    public void Initialize()
    {
        inventory.ConfigureByLevel(level);
    }

    public void SetLevel(int newLevel)
    {
        level = Mathf.Max(1, newLevel);
        inventory.ConfigureByLevel(level);
    }

    public void ClearInventory()
    {
        inventory.Clear();
    }
}