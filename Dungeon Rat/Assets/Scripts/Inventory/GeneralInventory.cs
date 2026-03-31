using UnityEngine;

[System.Serializable]
public class GeneralInventory : ContainerInventoryBase
{
    [SerializeField] private int level = 1;

    public int Level => level;

    public void ConfigureByLevel(int level)
    {
        this.level = Mathf.Clamp(level, 1, 4);

        switch (this.level)
        {
            case 1:
                Configure(50, 75, true);
                break;
            case 2:
                Configure(75, 100, true);
                break;
            case 3:
                Configure(100, 125, true);
                break;
            case 4:
                Configure(125, 150, true);
                break;
        }
    }
    public void RefreshCapacityByCurrentLevel()
    {
        ConfigureByLevel(level);
    }
    public bool HasOverflowItems()
    {
        if (!useOverflowSlots || slots == null)
            return false;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            if (slots[i].isOverflowSlot && !slots[i].IsEmpty())
                return true;
        }

        return false;
    }

    public bool CanEnterDungeon()
    {
        return !HasOverflowItems();
    }

    public void UpgradeLevel()
    {
        if (level >= 4)
            return;

        ConfigureByLevel(level + 1);
    }
}