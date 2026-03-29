using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBase : ScriptableObject
{
    [Header("Ability Info")]
    public string abilityName;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Costs")]
    public int apCost = 1;
    public int manaCost = 0;

    [Header("Cooldown")]
    public int cooldownTurns = 0;

    [Header("Targeting")]
    public bool requiresTarget = true;
    public AbilityTargetSide targetSide = AbilityTargetSide.Enemy;

    public virtual bool CanUse(Character user, BattleTurnManager battleTurnManager)
    {
        if (user == null || battleTurnManager == null)
            return false;

        return true;
    }

    public virtual List<EnemyCharacter> GetValidEnemyTargets(Character user, BattleTurnManager battleTurnManager)
    {
        return new List<EnemyCharacter>();
    }

    public virtual List<Character> GetValidAllyTargets(Character user, BattleTurnManager battleTurnManager)
    {
        return new List<Character>();
    }

    public abstract bool Use(Character user, EnemyCharacter enemyTarget, Character allyTarget, BattleTurnManager battleTurnManager);
}