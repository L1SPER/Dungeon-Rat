using UnityEngine;

[System.Serializable]
public class RatInventory : ContainerInventoryBase
{
    [SerializeField] private int level = 1;

    public int Level => level;

    public void ConfigureByLevel(int level)
    {
        this.level = Mathf.Clamp(level, 1, 3);

        switch (this.level)
        {
            case 1:
                Configure(10, 10, false);
                break;
            case 2:
                Configure(15, 15, false);
                break;
            case 3:
                Configure(20, 20, false);
                break;
        }
    }

    public void UpgradeLevel()
    {
        if (level >= 3)
            return;

        ConfigureByLevel(level + 1);
    }
}