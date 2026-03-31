using UnityEngine;

[System.Serializable]
public class Shield
{
    public int maxShield;
    public int currentShield;

    public void Initialize(int maxShield)
    {
        this.maxShield = Mathf.Max(0, maxShield);
        currentShield = this.maxShield;
    }

    public void SetMaxShield(int value)
    {
        maxShield = Mathf.Max(0, value);
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);
    }

    public void SetCurrentShield(int value)
    {
        currentShield = Mathf.Clamp(value, 0, maxShield);
    }

    public void Restore(int amount)
    {
        if (amount <= 0)
            return;

        currentShield = Mathf.Clamp(currentShield + amount, 0, maxShield);
    }

    public int Absorb(int damage)
    {
        if (damage <= 0)
            return 0;

        int absorbed = Mathf.Min(currentShield, damage);
        currentShield -= absorbed;

        return damage - absorbed;
    }

    public void Clear()
    {
        maxShield = 0;
        currentShield = 0;
    }
}