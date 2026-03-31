using UnityEngine;

[CreateAssetMenu(fileName = "EnemyNearestDamageAbility", menuName = "Enemy Abilities/Nearest Damage")]
public class EnemyNearestDamageAbility : EnemyAbilityBase
{
    [Header("Damage")]
    public float damageMultiplier = 1f;
    public int flatBonusDamage = 0;

    public override void Execute(EnemyCharacter user, BattleTurnManager battleTurnManager)
    {
        if (user == null || battleTurnManager == null || user.health.isDead)
            return;

        Character target = battleTurnManager.GetNearestLivingAllyInRange(user);
        if (target == null)
        {
            Debug.Log($"{user.EnemyName} hedef bulamadı.");
            return;
        }

        int baseDamage = user.Damage;
        int calculatedDamage = Mathf.RoundToInt(baseDamage * damageMultiplier) + flatBonusDamage;
        int appliedDamage = target.ApplyDamage(calculatedDamage);

        BattleDebugLogger.LogEnemyAction(
            user.EnemyName,
            abilityName,
            target.name,
            baseDamage,
            damageMultiplier,
            flatBonusDamage,
            calculatedDamage,
            appliedDamage
        );
    }
}