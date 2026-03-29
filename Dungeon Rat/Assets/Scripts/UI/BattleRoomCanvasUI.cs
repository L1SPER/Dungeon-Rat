using System.Collections.Generic;
using UnityEngine;

public class BattleRoomCanvasUI : MonoBehaviour
{
    [Header("Player Slots")]
    [SerializeField] private BattleSlotUI[] characterSlots;

    [Header("Enemy Slots")]
    [SerializeField] private BattleSlotUI[] enemySlots;

    private PartyManager partyManager;
    private List<EnemyCharacter> currentEnemies = new List<EnemyCharacter>();

    public BattleSlotUI[] CharacterSlots => characterSlots;
    public BattleSlotUI[] EnemySlots => enemySlots;
    public List<EnemyCharacter> CurrentEnemies => currentEnemies;

    private void Awake()
    {
        partyManager = FindFirstObjectByType<PartyManager>();
    }

    public void InitializeUI()
    {
        RefreshAll();

        if (currentEnemies.Count == 0 && DungeonManager.Instance != null)
            currentEnemies = DungeonManager.Instance.CreateCurrentEnemyParty();

        RefreshEnemyPartyUI();
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

            if (partyMembers != null && i < partyMembers.Length && partyMembers[i] != null && !partyMembers[i].health.isDead)
                characterSlots[i].SetCharacter(partyMembers[i]);
            else
                characterSlots[i].ClearSlot();
        }
    }

    public void RefreshEnemyPartyUI()
    {
        ClearEnemySlots();

        for (int i = 0; i < currentEnemies.Count; i++)
        {
            EnemyCharacter enemy = currentEnemies[i];
            if (enemy == null || enemy.isDead)
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

            bool canSelect = validTargets.Contains(enemySlots[i].CurrentEnemy);
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

            bool canSelect = validTargets.Contains(characterSlots[i].CurrentCharacter);
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