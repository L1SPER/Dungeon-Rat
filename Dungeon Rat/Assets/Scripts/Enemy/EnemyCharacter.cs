using UnityEngine;

[System.Serializable]
public class EnemyCharacter : IDamageable
{
    public BattleSlotUI SlotUI { get; set; }
    public EnemyData enemyData;
    public int position;

    [Header("Runtime Variables")]
    public Health health = new Health();
    public Shield shield = new Shield();

    public int currentArmor;
    public int stunTurns;

    public string EnemyName => enemyData != null ? enemyData.enemyName : string.Empty;
    public Sprite Sprite => enemyData != null ? enemyData.enemySprite : null;
    public int Damage => enemyData != null ? enemyData.damage : 0;
    public int Range => enemyData != null ? enemyData.range : 0;
    public EnemyAbilityBase Ability => enemyData != null ? enemyData.enemyAbility : null;


    public EnemyCharacter(EnemyData data, int position)
    {
        enemyData = data;
        this.position = position;

        currentArmor = data != null ? data.armor : 0;
        stunTurns = 0;

        Initialize();
    }

    public void Initialize()
    {
        int maxHealthValue = enemyData != null ? enemyData.maxHealth : 0;
        int maxShieldValue = enemyData != null ? enemyData.maxShield : 0;

        health.Initialize(Mathf.Max(1, maxHealthValue));
        shield.Initialize(Mathf.Max(0, maxShieldValue));

        currentArmor = enemyData != null ? enemyData.armor : 0;
        stunTurns = 0;
    }

    public void TakeDamage(int damage)
    {
        ApplyDamage(damage);
    }

    public int ApplyDamage(int damage)
    {
        if (health.isDead)
        {
            BattleDebugLogger.LogEnemyDamageIgnored(EnemyName, damage, "AlreadyDead");
            return 0;
        }

        if (damage <= 0)
        {
            BattleDebugLogger.LogEnemyDamageIgnored(EnemyName, damage, "NonPositiveDamage");
            return 0;
        }

        int rawDamage = damage;
        int beforeShield = shield.currentShield;
        int beforeHealth = health.currentHealth;


        int damageReductionFromArmor = currentArmor * 5;
        int afterArmor = Mathf.Max(damage - damageReductionFromArmor, 0);

        if (afterArmor <= 0)
        {
            BattleDebugLogger.LogEnemyDamage(
                EnemyName,
                rawDamage,
                currentArmor,
                damageReductionFromArmor,
                afterArmor,
                beforeShield,
                shield.currentShield,
                beforeHealth,
                health.currentHealth,
                0
            );

            return 0;
        }
        
        int remainingDamage = afterArmor;

        if (shield.currentShield > 0)
            remainingDamage = shield.Absorb(remainingDamage);

        if (remainingDamage > 0)
            health.TakeDamage(remainingDamage);

        int afterShield = shield.currentShield;
        int afterHealth = health.currentHealth;

        int shieldDamage = beforeShield - afterShield;
        int healthDamage = beforeHealth - afterHealth;
        int appliedDamage = shieldDamage + healthDamage;

        BattleDebugLogger.LogEnemyDamage(
            EnemyName,
            rawDamage,
            currentArmor,
            damageReductionFromArmor,
            afterArmor,
            beforeShield,
            afterShield,
            beforeHealth,
            afterHealth,
            appliedDamage
        );

        return appliedDamage;
    }

    public void Heal(int amount)
    {
        if (health == null)
            return;

        health.Heal(amount);
    }

    // Eski ability kodları bozulmasın diye bunu koruyoruz
    public void AddShield(int amount)
    {
        RestoreShield(amount);
    }

    public void RestoreShield(int amount)
    {
        if (health.isDead || amount <= 0 || shield == null)
            return;

        shield.Restore(amount);
    }

    public void SetShieldDirect(int amount)
    {
        if (shield == null)
            return;

        shield.SetCurrentShield(amount);
    }

    public void BreakArmor(int amount)
    {
        if (amount <= 0)
            return;

        currentArmor = Mathf.Max(0, currentArmor - amount);
    }

    public void ApplyStun(int turns)
    {
        if (turns <= 0 || health.isDead)
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

    public void RefreshFromData()
    {
        int newMaxHealth = enemyData != null ? enemyData.maxHealth : 1;
        int newMaxShield = enemyData != null ? enemyData.maxShield : 0;

        if (health != null)
        {
            if (health.maxHealth <= 0)
                health.Initialize(Mathf.Max(1, newMaxHealth));
            else
                health.SetMaxHealth(newMaxHealth);
        }

        if (shield != null)
        {
            if (shield.maxShield <= 0 && shield.currentShield <= 0)
                shield.Initialize(Mathf.Max(0, newMaxShield));
            else
                shield.SetMaxShield(newMaxShield);
        }

        currentArmor = enemyData != null ? enemyData.armor : 0;
    }
}