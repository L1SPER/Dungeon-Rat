using UnityEngine;

public static class BattleDebugLogger
{
    public static bool Enabled = true;

    public static void LogCharacterDamage(
        string targetName,
        int rawDamage,
        string firstHitReductionInfo,
        int afterFirstHitReduction,
        int armor,
        int damageReductionFromArmor,
        int afterArmor,
        int shieldBefore,
        int shieldAfter,
        int healthBefore,
        int healthAfter,
        int appliedDamage)
    {
        if (!Enabled) return;

        Debug.Log(
            $"[CharacterDamage] {targetName} hasar aldı | " +
            $"Raw:{rawDamage} " +
            $"FirstHitReduction:{firstHitReductionInfo} " +
            $"AfterFirstHitReduction:{afterFirstHitReduction} " +
            $"Armor:{armor}, damageReductionFromArmor:{damageReductionFromArmor}, AfterArmor:{afterArmor} " +
            $"Shield:{shieldBefore}->{shieldAfter} (-{shieldBefore - shieldAfter}) " +
            $"Health:{healthBefore}->{healthAfter} (-{healthBefore - healthAfter}) " +
            $"Applied:{appliedDamage}"
        );
    }

    public static void LogCharacterDamageIgnored(string targetName, int rawDamage, string reason)
    {
        if (!Enabled) return;

        Debug.Log(
            $"[CharacterDamageIgnored] {targetName} | Raw:{rawDamage} Reason:{reason}"
        );
    }

    public static void LogEnemyDamage(
        string targetName,
        int rawDamage,
        int armor,
        int damageReductionFromArmor,
        int afterArmor,
        int shieldBefore,
        int shieldAfter,
        int healthBefore,
        int healthAfter,
        int appliedDamage)
    {
        if (!Enabled) return;

        Debug.Log(
            $"[EnemyDamage] {targetName} hasar aldı | " +
            $"Raw:{rawDamage} " +
            $"Armor:{armor} AfterArmor:{afterArmor} " +
            $"Shield:{shieldBefore}->{shieldAfter} (-{shieldBefore - shieldAfter}) " +
            $"Health:{healthBefore}->{healthAfter} (-{healthBefore - healthAfter}) " +
            $"Applied:{appliedDamage}"
        );
    }

    public static void LogEnemyDamageIgnored(string targetName, int rawDamage, string reason)
    {
        if (!Enabled) return;

        Debug.Log(
            $"[EnemyDamageIgnored] {targetName} | Raw:{rawDamage} Reason:{reason}"
        );
    }

    public static void LogEnemyAction(
        string enemyName,
        string actionName,
        string targetName,
        int baseDamage,
        float multiplier,
        int flatBonus,
        int calculatedDamage,
        int appliedDamage)
    {
        if (!Enabled) return;

        Debug.Log(
            $"[EnemyAction] {enemyName} {actionName} kullandı -> " +
            $"Hedef:{targetName} BaseDamage:{baseDamage} Mult:{multiplier} " +
            $"FlatBonus:{flatBonus} Calculated:{calculatedDamage} Applied:{appliedDamage}"
        );
    }

    public static void LogEnemyBasicAttack(
        string enemyName,
        string targetName,
        int baseDamage,
        int appliedDamage)
    {
        if (!Enabled) return;

        Debug.Log(
            $"[EnemyBasicAttack] {enemyName} normal saldırı yaptı -> " +
            $"Hedef:{targetName} BaseDamage:{baseDamage} Applied:{appliedDamage}"
        );
    }

    public static void LogPlayerAction(
        string userName,
        string actionName,
        string targetName,
        int baseDamage,
        float multiplier,
        int flatBonus,
        int calculatedDamage,
        int appliedDamage)
    {
        if (!Enabled) return;

        Debug.Log(
            $"[PlayerAction] {userName} {actionName} kullandı -> " +
            $"Hedef:{targetName} BaseDamage:{baseDamage} Mult:{multiplier} " +
            $"FlatBonus:{flatBonus} Calculated:{calculatedDamage} Applied:{appliedDamage}"
        );
    }
}