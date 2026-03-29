using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SplashBehindAbility", menuName = "Abilities/Splash Behind")]
public class SplashBehindAbility : AbilityBase
{
    [Header("Damage")]
    public float mainDamageMultiplier = 1f;
    [Range(0f, 1f)] public float behindDamagePercent = 0.5f;
    public int armorBreakAmount = 0;

    private void OnEnable()
    {
        requiresTarget = true;
        targetSide = AbilityTargetSide.Enemy;
        apCost = 1;
        manaCost = 0;
    }

    public override List<EnemyCharacter> GetValidEnemyTargets(Character user, BattleTurnManager battleTurnManager)
    {
        return battleTurnManager.GetEnemiesInRange(user);
    }

    public override bool Use(Character user, EnemyCharacter enemyTarget, Character allyTarget, BattleTurnManager battleTurnManager)
    {
        if (user == null || enemyTarget == null || battleTurnManager == null)
            return false;

        int baseDamage = battleTurnManager.GetWeaponDamage(user);
        int mainDamage = Mathf.RoundToInt(baseDamage * mainDamageMultiplier);

        enemyTarget.TakeDamage(mainDamage);

        if (armorBreakAmount > 0)
            enemyTarget.BreakArmor(armorBreakAmount);

        EnemyCharacter behindEnemy = battleTurnManager.GetEnemyBehind(enemyTarget);

        if (behindEnemy != null)
        {
            int splashDamage = Mathf.RoundToInt(mainDamage * behindDamagePercent);
            behindEnemy.TakeDamage(splashDamage);
            Debug.Log($"{user.name} {abilityName} kullandı. {enemyTarget.EnemyName} {mainDamage}, arkasındaki {behindEnemy.EnemyName} {splashDamage} hasar aldı.");
        }
        else
        {
            Debug.Log($"{user.name} {abilityName} kullandı. {enemyTarget.EnemyName} {mainDamage} hasar aldı.");
        }

        return true;
    }
}