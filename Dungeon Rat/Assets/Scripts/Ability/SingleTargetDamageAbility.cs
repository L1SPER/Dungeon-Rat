using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SingleTargetDamageAbility", menuName = "Abilities/Single Target Damage")]
public class SingleTargetDamageAbility : AbilityBase
{
    [Header("Damage")]
    public float damageMultiplier = 1f;
    public int flatBonusDamage = 0;

    [Header("Extra Effects")]
    public int armorBreakAmount = 0;
    public int stunTurns = 0;
    public bool applySkipNextTurnToUser = false;

    private void OnEnable()
    {
        requiresTarget = true;
        targetSide = AbilityTargetSide.Enemy;
        apCost = 1;
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
        int calculatedDamage = Mathf.RoundToInt(baseDamage * damageMultiplier) + flatBonusDamage;
        int appliedDamage = enemyTarget.ApplyDamage(calculatedDamage);

        if (armorBreakAmount > 0)
            enemyTarget.BreakArmor(armorBreakAmount);

        if (stunTurns > 0)
            enemyTarget.ApplyStun(stunTurns);

        if (applySkipNextTurnToUser)
            user.ApplySkipNextTurn();

        BattleDebugLogger.LogPlayerAction(
            user.name,
            abilityName,
            enemyTarget.EnemyName,
            baseDamage,
            damageMultiplier,
            flatBonusDamage,
            calculatedDamage,
            appliedDamage
        );

        return true;
    }
}