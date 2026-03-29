using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponBasicAttackAbility", menuName = "Abilities/Weapon Basic Attack")]
public class WeaponBasicAttackAbility : AbilityBase
{
    [Header("Damage")]
    public int offClassDamage = 10;
    public float damageMultiplier = 1f;

    private void OnEnable()
    {
        requiresTarget = true;
        targetSide = AbilityTargetSide.Enemy;
        apCost = 1;
        manaCost = 0;
        cooldownTurns = 0;
    }

    public override List<EnemyCharacter> GetValidEnemyTargets(Character user, BattleTurnManager battleTurnManager)
    {
        return battleTurnManager.GetEnemiesInRange(user);
    }

    public override bool Use(Character user, EnemyCharacter enemyTarget, Character allyTarget, BattleTurnManager battleTurnManager)
    {
        if (user == null || enemyTarget == null || battleTurnManager == null)
            return false;

        Weapon weapon = user.GetEquippedWeapon();
        if (weapon == null)
            return false;

        int damage = battleTurnManager.GetBasicAttackDamage(user, weapon, offClassDamage);
        damage = Mathf.RoundToInt(damage * damageMultiplier);

        enemyTarget.TakeDamage(damage);

        Debug.Log($"{user.name} {abilityName} kullandı. {enemyTarget.EnemyName} {damage} hasar aldı.");
        return true;
    }
}