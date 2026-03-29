using UnityEngine;

[CreateAssetMenu(fileName = "SelfGuardAbility", menuName = "Abilities/Self Guard")]
public class SelfGuardAbility : AbilityBase
{
    [Header("Guard")]
    [Range(0, 100)] public int reducePercent = 50;

    private void OnEnable()
    {
        requiresTarget = false;
        targetSide = AbilityTargetSide.None;
        apCost = 1;
        manaCost = 0;
    }

    public override bool Use(Character user, EnemyCharacter enemyTarget, Character allyTarget, BattleTurnManager battleTurnManager)
    {
        if (user == null)
            return false;

        user.SetFirstHitDamageReduction(reducePercent);
        Debug.Log($"{user.name} {abilityName} kullandı. İlk alacağı hasar %{reducePercent} azaltılacak.");
        return true;
    }
}