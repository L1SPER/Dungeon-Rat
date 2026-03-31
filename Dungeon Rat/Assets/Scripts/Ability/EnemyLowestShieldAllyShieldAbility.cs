using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAllyShieldAbility", menuName = "Enemy Abilities/Ally Shield")]
public class EnemyLowestShieldAllyShieldAbility : EnemyAbilityBase
{
    [Header("Shield")]
    public int shieldAmount = 10;

    public override bool PerformBasicAttackAfterUse => true;

    public override void Execute(EnemyCharacter user, BattleTurnManager battleTurnManager)
    {
        if (user == null || battleTurnManager == null || user.health.isDead)
            return;

        EnemyCharacter target = battleTurnManager.GetLowestShieldLivingEnemyAlly(user);

        if (target == null)
        {
            Debug.Log($"{user.EnemyName} shield verecek takım arkadaşı bulamadı.");
            return;
        }

        target.AddShield(shieldAmount);
        Debug.Log($"{user.EnemyName} {abilityName} kullandı. {target.EnemyName} {shieldAmount} shield kazandı.");
    }
}