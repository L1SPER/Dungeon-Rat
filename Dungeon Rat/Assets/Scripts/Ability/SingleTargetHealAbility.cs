using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SingleTargetHealAbility", menuName = "Abilities/Single Target Heal")]
public class SingleTargetHealAbility : AbilityBase
{
    [Header("Heal")]
    public int healAmount = 15;

    private void OnEnable()
    {
        requiresTarget = true;
        targetSide = AbilityTargetSide.Ally;
        apCost = 1;
        manaCost = 1;
    }

    public override List<Character> GetValidAllyTargets(Character user, BattleTurnManager battleTurnManager)
    {
        return battleTurnManager.GetLivingAllies();
    }

    public override bool Use(Character user, EnemyCharacter enemyTarget, Character allyTarget, BattleTurnManager battleTurnManager)
    {
        if (user == null || allyTarget == null)
            return false;

        allyTarget.Heal(healAmount);
        Debug.Log($"{user.name} {abilityName} kullandı. {allyTarget.name} {healAmount} iyileşti.");
        return true;
    }
}