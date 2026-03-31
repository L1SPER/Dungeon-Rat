using UnityEngine;

public abstract class EnemyAbilityBase : ScriptableObject
{
    [Header("Info")]
    public string abilityName;
    [TextArea] public string description;

    public virtual bool PerformBasicAttackAfterUse => false;

    public abstract void Execute(EnemyCharacter user, BattleTurnManager battleTurnManager);
}