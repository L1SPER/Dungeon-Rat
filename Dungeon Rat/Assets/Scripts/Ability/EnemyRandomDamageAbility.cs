using UnityEngine;

[CreateAssetMenu(fileName = "EnemyRandomDamageAbility", menuName = "Enemy Abilities/Random Damage")]
public class EnemyRandomDamageAbility : EnemyAbilityBase
{
    [Header("Damage")]
    public float damageMultiplier = 1f;
    public int flatBonusDamage = 0;

    public override void Execute(EnemyCharacter user, BattleTurnManager battleTurnManager)
    {
        if (user == null || battleTurnManager == null || user.health.isDead)
            return;

        Character target = battleTurnManager.GetRandomLivingAllyInRange(user);
        if (target == null)
        {
            Debug.Log($"{user.EnemyName} rastgele hedef bulamadı.");
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