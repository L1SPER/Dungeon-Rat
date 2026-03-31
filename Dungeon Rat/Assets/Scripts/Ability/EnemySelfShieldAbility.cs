using UnityEngine;

[CreateAssetMenu(fileName = "EnemySelfShieldAbility", menuName = "Enemy Abilities/Self Shield")]
public class EnemySelfShieldAbility : EnemyAbilityBase
{
    [Header("Shield")]
    public int shieldAmount = 10;

    public override bool PerformBasicAttackAfterUse => true;

    public override void Execute(EnemyCharacter user, BattleTurnManager battleTurnManager)
    {
        if (user == null || user.health.isDead)
            return;

        user.AddShield(shieldAmount);
        Debug.Log($"{user.EnemyName} {abilityName} kullandı. {shieldAmount} shield kazandı.");
    }
}