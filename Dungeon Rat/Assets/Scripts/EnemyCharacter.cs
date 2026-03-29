using UnityEngine;

[System.Serializable]
public class EnemyCharacter
{
    public EnemyData enemyData;
    public int position;

    public int currentHealth;
    public int currentShield;
    public int currentArmor;
    public int stunTurns;
    public bool isDead;

    public string EnemyName => enemyData != null ? enemyData.enemyName : string.Empty;
    public Sprite Sprite => enemyData != null ? enemyData.enemySprite : null;
    public int MaxHealth => enemyData != null ? enemyData.maxHealth : 0;
    public int Damage => enemyData != null ? enemyData.damage : 0;
    public int Range => enemyData != null ? enemyData.range : 0;

    public EnemyCharacter(EnemyData data, int position)
    {
        enemyData = data;
        this.position = position;

        currentHealth = data != null ? data.maxHealth : 0;
        currentShield = 0;
        currentArmor = data != null ? data.armor : 0;
        stunTurns = 0;
        isDead = false;
    }
    public void TakeDamage(int damage)
    {
        ApplyDamage(damage);
    }
    public int ApplyDamage(int damage)
    {
        if (isDead || damage <= 0)
            return 0;

        int beforeShield = currentShield;
        int beforeHealth = currentHealth;

        int finalDamage = Mathf.Max(damage - currentArmor, 0);

        if (currentShield > 0)
        {
            if (finalDamage <= currentShield)
            {
                currentShield -= finalDamage;
                finalDamage = 0;
            }
            else
            {
                finalDamage -= currentShield;
                currentShield = 0;
            }
        }

        currentHealth -= finalDamage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
        }

        int shieldDamage = beforeShield - currentShield;
        int healthDamage = beforeHealth - currentHealth;

        return shieldDamage + healthDamage;
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0)
            return;

        currentHealth += amount;
        if (currentHealth > MaxHealth)
            currentHealth = MaxHealth;
    }

    public void AddShield(int amount)
    {
        if (isDead || amount <= 0)
            return;

        currentShield += amount;
    }

    public void BreakArmor(int amount)
    {
        if (amount <= 0)
            return;

        currentArmor = Mathf.Max(0, currentArmor - amount);
    }

    public void ApplyStun(int turns)
    {
        if (turns <= 0 || isDead)
            return;

        stunTurns += turns;
    }

    public bool ConsumeOneStunTurn()
    {
        if (stunTurns <= 0)
            return false;

        stunTurns--;
        return true;
    }
}