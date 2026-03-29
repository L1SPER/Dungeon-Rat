using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoubleShotAbility", menuName = "Abilities/Double Shot")]
public class DoubleShotAbility : AbilityBase
{
    [Header("Damage")]
    public float damageMultiplier = 1f;

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

        List<EnemyCharacter> targets = battleTurnManager.GetEnemiesInRange(user);
        if (targets.Count == 0)
            return false;

        int damage = Mathf.RoundToInt(battleTurnManager.GetWeaponDamage(user) * damageMultiplier);

        enemyTarget.TakeDamage(damage);
        Debug.Log($"{user.name} {abilityName} ile {enemyTarget.EnemyName} hedefini vurdu. Damage: {damage}");

        EnemyCharacter secondTarget = null;

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null || targets[i].isDead || targets[i] == enemyTarget)
                continue;

            secondTarget = targets[i];
            break;
        }

        if (secondTarget != null)
        {
            secondTarget.TakeDamage(damage);
            Debug.Log($"{user.name} {abilityName} ile ikinci hedef {secondTarget.EnemyName} vurdu. Damage: {damage}");
        }

        return true;
    }
}