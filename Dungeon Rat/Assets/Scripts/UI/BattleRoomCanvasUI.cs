using System.Collections.Generic;
using UnityEngine;

public class BattleRoomCanvasUI : MonoBehaviour
{
    [Header("Player Slots")]
    [SerializeField] private BattleSlotUI[] characterSlots;

    [Header("Enemy Slots")]
    [SerializeField] private BattleSlotUI[] enemySlots;

    private PartyManager partyManager;
    private EnemyPartyManager enemyPartyManager;

    public BattleSlotUI[] CharacterSlots => characterSlots;
    public BattleSlotUI[] EnemySlots => enemySlots;

    private void Awake()
    {
        partyManager = FindFirstObjectByType<PartyManager>();
        enemyPartyManager = FindFirstObjectByType<EnemyPartyManager>();
    }

    private void OnEnable()
    {
        if (partyManager == null)
            partyManager = FindFirstObjectByType<PartyManager>();

        if (enemyPartyManager == null)
            enemyPartyManager = FindFirstObjectByType<EnemyPartyManager>();

        if (partyManager != null)
            partyManager.OnPartyChanged += RefreshPlayerParty;

        if (enemyPartyManager != null)
            enemyPartyManager.OnEnemyPartyChanged += RefreshEnemyPartyUI;
    }

    private void OnDisable()
    {
        if (partyManager != null)
            partyManager.OnPartyChanged -= RefreshPlayerParty;

        if (enemyPartyManager != null)
            enemyPartyManager.OnEnemyPartyChanged -= RefreshEnemyPartyUI;
    }

    public void InitializeUI()
    {
        if (enemyPartyManager == null)
            enemyPartyManager = FindFirstObjectByType<EnemyPartyManager>();

        if (enemyPartyManager != null && enemyPartyManager.IsPartyEmpty() && DungeonManager.Instance != null)
        {
            List<EnemyCharacter> createdEnemies = DungeonManager.Instance.CreateCurrentEnemyParty();
            enemyPartyManager.SetEnemyParty(createdEnemies);
        }

        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshPlayerParty();
        RefreshEnemyPartyUI();
    }

    public void RefreshPlayerParty()
    {
        if (partyManager == null)
        {
            Debug.LogWarning("PartyManager bulunamadı.");
            return;
        }

        Character[] partyMembers = partyManager.GetPartyMembers();

        for (int i = 0; i < characterSlots.Length; i++)
        {
            if (characterSlots[i] == null)
                continue;

            if (partyMembers != null &&
                i < partyMembers.Length &&
                partyMembers[i] != null &&
                partyMembers[i].health != null &&
                !partyMembers[i].health.isDead)
            {
                characterSlots[i].SetCharacter(partyMembers[i]);
            }
            else
            {
                characterSlots[i].ClearSlot();
            }
        }
    }

    public void RefreshEnemyPartyUI()
    {
        ClearEnemySlots();

        if (enemyPartyManager == null)
        {
            Debug.LogWarning("EnemyPartyManager bulunamadı.");
            return;
        }

        EnemyCharacter[] enemies = enemyPartyManager.GetPartyMembers();

        if (enemies == null)
            return;

        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyCharacter enemy = enemies[i];

            if (enemy == null || enemy.health == null || enemy.health.isDead)
                continue;

            int slotIndex = enemy.position - 1;

            if (slotIndex >= 0 && slotIndex < enemySlots.Length && enemySlots[slotIndex] != null)
                enemySlots[slotIndex].SetEnemy(enemy);
        }
    }

    public void ClearAllSelections()
    {
        for (int i = 0; i < enemySlots.Length; i++)
        {
            if (enemySlots[i] == null)
                continue;

            enemySlots[i].SetSelectable(false);
            enemySlots[i].SetSelected(false);
        }

        for (int i = 0; i < characterSlots.Length; i++)
        {
            if (characterSlots[i] == null)
                continue;

            characterSlots[i].SetSelectable(false);
            characterSlots[i].SetSelected(false);
        }
    }

    public void EnableEnemySelections(List<EnemyCharacter> validTargets)
    {
        ClearAllSelections();

        for (int i = 0; i < enemySlots.Length; i++)
        {
            if (enemySlots[i] == null || enemySlots[i].CurrentEnemy == null)
                continue;

            bool canSelect = validTargets != null && validTargets.Contains(enemySlots[i].CurrentEnemy);
            enemySlots[i].SetSelectable(canSelect);
        }
    }

    public void EnableAllySelections(List<Character> validTargets)
    {
        ClearAllSelections();

        for (int i = 0; i < characterSlots.Length; i++)
        {
            if (characterSlots[i] == null || characterSlots[i].CurrentCharacter == null)
                continue;

            bool canSelect = validTargets != null && validTargets.Contains(characterSlots[i].CurrentCharacter);
            characterSlots[i].SetSelectable(canSelect);
        }
    }

    private void ClearEnemySlots()
    {
        for (int i = 0; i < enemySlots.Length; i++)
        {
            if (enemySlots[i] != null)
                enemySlots[i].ClearSlot();
        }
    }
}