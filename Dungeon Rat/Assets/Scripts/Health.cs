using UnityEngine;

public class Health
{   
    public int maxHealth;
    public int currentHealth;
    public bool isInvulnerable;
    public bool isDead;

    public void Initialize(int maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
    }
    public void SetMaxHealth(int _maxHealth)
    {
        this.maxHealth = _maxHealth;
    }
    public void SetCurrentHealth(int _currentHealth)
    {
        this.currentHealth = _currentHealth;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
            isDead = true;
        }
    }
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
    public void SetInvulnerable(bool _isInvulnerable)
    {
        this.isInvulnerable = _isInvulnerable;
    }
    public void SetIsDead(bool _isDead)
    {
        this.isDead = _isDead;
    }
    public bool IsDead()
    {
        return isDead;
    }
}
