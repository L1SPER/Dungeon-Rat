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

        bool isCritical;
        int weaponDamage = battleTurnManager.GetWeaponDamage(user, out isCritical);

        int damage = Mathf.RoundToInt(weaponDamage * damageMultiplier);

        Color damageColor = isCritical
        ? new Color(1f, 0.55f, 0f)
        : Color.yellow;


        int firstDealtDamage = enemyTarget.ApplyDamage(damage);
        Debug.Log($"{user.name} {abilityName} ile {enemyTarget.EnemyName} hedefini vurdu. Toplam hasar: {firstDealtDamage}");
        if (enemyTarget.SlotUI != null)
        {
            FloatingCombatTextManager.Instance?.ShowDamage(
                firstDealtDamage,
                enemyTarget.SlotUI.DamageTextAnchor,
                damageColor
            );
        }

        EnemyCharacter secondTarget = battleTurnManager.GetEnemyBehind(enemyTarget);
        int secondDealtDamage = 0;
        if (secondTarget != null && !secondTarget.health.isDead&& secondTarget.SlotUI != null)
        {
            secondDealtDamage = secondTarget.ApplyDamage(damage);
            FloatingCombatTextManager.Instance?.ShowDamage(
                secondDealtDamage,
                secondTarget.SlotUI.DamageTextAnchor,
                damageColor
            );
            Debug.Log($"{user.name} {abilityName} ile arkasındaki hedef {secondTarget.EnemyName} vurdu. Toplam hasar: {secondDealtDamage}");
        }
        return true;
    }
}