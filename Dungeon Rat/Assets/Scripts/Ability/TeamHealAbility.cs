using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamHealAbility", menuName = "Abilities/Team Heal")]
public class TeamHealAbility : AbilityBase
{
    [Header("Heal")]
    public int healAmount = 10;

    private void OnEnable()
    {
        requiresTarget = false;
        targetSide = AbilityTargetSide.None;
        apCost = 1;
        manaCost = 1;
    }

    public override bool Use(Character user, EnemyCharacter enemyTarget, Character allyTarget, BattleTurnManager battleTurnManager)
    {
        if (user == null || battleTurnManager == null)
            return false;

        List<Character> allies = battleTurnManager.GetLivingAllies();

        for (int i = 0; i < allies.Count; i++)
        {
            if (allies[i] == null)
                continue;

            allies[i].Heal(healAmount);
        }

        Debug.Log($"{user.name} {abilityName} kullandı. Tüm takım {healAmount} iyileşti.");
        return true;
    }
}