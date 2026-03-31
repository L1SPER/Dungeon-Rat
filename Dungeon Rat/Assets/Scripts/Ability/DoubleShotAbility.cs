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

        int damage = Mathf.RoundToInt(battleTurnManager.GetWeaponDamage(user) * damageMultiplier);

        int firstDealtDamage = enemyTarget.ApplyDamage(damage);
        Debug.Log($"{user.name} {abilityName} ile {enemyTarget.EnemyName} hedefini vurdu. Toplam hasar: {firstDealtDamage}");

        EnemyCharacter secondTarget = battleTurnManager.GetEnemyBehind(enemyTarget);

        if (secondTarget != null && !secondTarget.health.isDead)
        {
            int secondDealtDamage = secondTarget.ApplyDamage(damage);
            Debug.Log($"{user.name} {abilityName} ile arkasındaki hedef {secondTarget.EnemyName} vurdu. Toplam hasar: {secondDealtDamage}");
        }
        return true;
    }
}