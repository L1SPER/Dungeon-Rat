using Unity.VisualScripting;
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
        this.isDead = false;
        this.isInvulnerable=false;
    }
    public void SetMaxHealth(int _maxHealth)
    {
        if(_maxHealth < 0)
        {
            Debug.LogWarning("Max health cannot be negative.");
            return;
        }
        maxHealth = Mathf.Max(1, _maxHealth);

        if(currentHealth> maxHealth)
        {
            currentHealth = maxHealth;
        }
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
        if(isDead || amount <= 0)
            return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}
