using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTurnManager : MonoBehaviour
{
    private enum BattlePhase
    {
        PlayerTurn,
        EnemyTurn
    }

    [System.Serializable]
    private class CharacterBattleState
    {
        public Character character;
        public int mana;
        public bool turnFinished;
        public bool hasStartedTurnBefore;

        public CharacterBattleState(Character character)
        {
            this.character = character;
            mana = 1;
            turnFinished = false;
            hasStartedTurnBefore = false;
        }
    }

    [Header("References")]
    [SerializeField] private BattleRoomCanvasUI battleRoomCanvasUI;
    [SerializeField] private CharacterPanelBattleUI characterPanelUI;
    [SerializeField] private DungeonRoomCanvasManager canvasManager;

    [SerializeField] private LootInventoryObject lootInventoryObject;
    [SerializeField] private LootInventoryUI lootInventoryUI;
    [SerializeField] private ItemDatabaseObject itemDatabase;
    [SerializeField] private int rewardItemCount = 5;

    [Header("Settings")]
    [SerializeField] private float enemyTurnDelay = 1f;

    private PartyManager partyManager;
    private EnemyPartyManager enemyPartyManager;
    private CharacterBattleState[] characterStates;

    private BattlePhase currentPhase;
    private int currentCharacterIndex = -1;
    private int currentAP = 0;

    private bool isSelectingTarget = false;
    private AbilityBase pendingAbility = null;
    private bool battleEnded = false;

    private void Awake()
    {
        partyManager = FindFirstObjectByType<PartyManager>();
        enemyPartyManager = FindFirstObjectByType<EnemyPartyManager>();

        if (partyManager != null)
            partyManager.OnPartyChanged += HandlePartyChanged;

        if (battleRoomCanvasUI == null)
            battleRoomCanvasUI = FindFirstObjectByType<BattleRoomCanvasUI>();

        if (characterPanelUI == null)
            characterPanelUI = FindFirstObjectByType<CharacterPanelBattleUI>();

        if (canvasManager == null)
            canvasManager = FindFirstObjectByType<DungeonRoomCanvasManager>();
    }

    private void OnDestroy()
    {
        if (partyManager != null)
            partyManager.OnPartyChanged -= HandlePartyChanged;
    }

    private IEnumerator Start()
    {
        yield return null;

        if (battleRoomCanvasUI != null)
        {
            for (int i = 0; i < battleRoomCanvasUI.EnemySlots.Length; i++)
            {
                if (battleRoomCanvasUI.EnemySlots[i] != null)
                    battleRoomCanvasUI.EnemySlots[i].ConfigureClick(OnSlotClicked);
            }

            for (int i = 0; i < battleRoomCanvasUI.CharacterSlots.Length; i++)
            {
                if (battleRoomCanvasUI.CharacterSlots[i] != null)
                    battleRoomCanvasUI.CharacterSlots[i].ConfigureClick(OnSlotClicked);
            }
        }

        if (characterPanelUI != null)
            characterPanelUI.Initialize(this);
    }

    public void InitializeBattle()
    {
        if (enemyPartyManager == null)
            enemyPartyManager = FindFirstObjectByType<EnemyPartyManager>();

        if (battleRoomCanvasUI == null)
            battleRoomCanvasUI = FindFirstObjectByType<BattleRoomCanvasUI>();

        if (enemyPartyManager != null)
            enemyPartyManager.ClearEnemyParty();

        battleEnded = false;
        isSelectingTarget = false;
        pendingAbility = null;
        currentCharacterIndex = -1;
        currentAP = 0;

        if (partyManager == null)
        {
            Debug.LogWarning("BattleTurnManager -> PartyManager bulunamadı.");
            return;
        }

        Character[] partyMembers = partyManager.GetPartyMembers();

        if (partyMembers == null || partyMembers.Length == 0)
        {
            Debug.LogWarning("BattleTurnManager -> Party boş.");
            return;
        }

        BuildFreshCharacterStates();

        if (canvasManager != null)
            canvasManager.ShowBattleUI();

        if (battleRoomCanvasUI != null)
            battleRoomCanvasUI.InitializeUI();

        RefreshCharacterPanelUI();
        StartPlayerPhase();
    }
    private void BuildFreshCharacterStates()
    {
        if (partyManager == null)
            return;

        Character[] partyMembers = partyManager.GetPartyMembers();
        if (partyMembers == null)
        {
            characterStates = null;
            currentCharacterIndex = -1;
            return;
        }

        characterStates = new CharacterBattleState[partyMembers.Length];

        for (int i = 0; i < partyMembers.Length; i++)
        {
            Character member = partyMembers[i];

            if (member == null)
                continue;

            characterStates[i] = new CharacterBattleState(member);
        }

        currentCharacterIndex = -1;
    }
    private void WinBattle()
    {
        if (battleEnded)
            return;

        battleEnded = true;
        CancelCurrentSelection(false);
        StopAllCoroutines();

        Debug.Log("Tüm düşmanlar öldü. Battle kazanıldı.");

        GenerateBattleRewardLoot();

        if (canvasManager != null)
            canvasManager.ShowAfterRoomUI();
    }
    private void GenerateBattleRewardLoot()
    {
        if (lootInventoryObject == null || lootInventoryObject.inventory == null)
        {
            Debug.LogWarning("LootInventoryObject veya LootInventory bağlı değil.");
            return;
        }

        if (itemDatabase == null || itemDatabase.items == null || itemDatabase.items.Length == 0)
        {
            Debug.LogWarning("ItemDatabase bulunamadı veya boş.");
            return;
        }

        DungeonTier currentTier = DungeonTier.Common;

        if (DungeonManager.Instance != null)
            currentTier = DungeonManager.Instance.CurrentTier;

        int rewardCount = GetRandomRewardItemCountByTier(currentTier);
        lootInventoryObject.inventory.GenerateDungeonLoot(itemDatabase.items, currentTier, rewardCount);

        int generatedCount = 0;
        InventorySlot[] slots = lootInventoryObject.GetSlots;

        if (slots != null)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null && !slots[i].IsEmpty())
                    generatedCount++;
            }
        }

        Debug.Log($"Loot üretildi. Tier: {currentTier} | Dolu Slot Sayısı: {generatedCount}");
    }

    private void LoseBattle()
    {
        if (battleEnded)
            return;

        battleEnded = true;
        CancelCurrentSelection(false);
        StopAllCoroutines();

        Debug.Log("Tüm oyuncular öldü. Battle kaybedildi.");
        //RefreshCharacterPanelUI();

        if (DungeonManager.Instance != null && DungeonManager.Instance.HasActiveRun)
        {
            DungeonManager.Instance.MarkCharacterDeath();
            DungeonManager.Instance.EndDungeon();
        }

    }

    private void HandlePartyChanged()
    {
        if (battleEnded)
            return;

        Character previousActiveCharacter = GetCurrentCharacter();

        RebuildCharacterStates(previousActiveCharacter);
        CancelCurrentSelection(false);

        if (battleRoomCanvasUI != null)
            battleRoomCanvasUI.RefreshAll();

        if (currentPhase == BattlePhase.PlayerTurn)
        {
            if (partyManager != null && partyManager.AreAllDead())
            {
                LoseBattle();
                return;
            }

            Character currentCharacter = GetCurrentCharacter();
            CharacterBattleState currentState = GetCurrentState();

            if (currentCharacter == null || currentState == null || currentCharacter.health.isDead || currentState.turnFinished)
            {
                StartNextAvailableCharacterTurn();
                return;
            }
        }

        UpdateTurnUI();
    }

    private void RebuildCharacterStates(Character preferredCurrentCharacter)
    {
        if (partyManager == null)
            return;

        Character[] partyMembers = partyManager.GetPartyMembers();
        if (partyMembers == null)
        {
            characterStates = null;
            currentCharacterIndex = -1;
            return;
        }

        CharacterBattleState[] newStates = new CharacterBattleState[partyMembers.Length];

        for (int i = 0; i < partyMembers.Length; i++)
        {
            Character member = partyMembers[i];

            if (member == null)
                continue;

            CharacterBattleState existingState = FindStateByCharacter(member);
            newStates[i] = existingState ?? new CharacterBattleState(member);
        }

        characterStates = newStates;

        if (preferredCurrentCharacter != null)
            currentCharacterIndex = GetStateIndexByCharacter(preferredCurrentCharacter);
        else
            currentCharacterIndex = -1;
    }

    private CharacterBattleState FindStateByCharacter(Character character)
    {
        if (characterStates == null || character == null)
            return null;

        for (int i = 0; i < characterStates.Length; i++)
        {
            if (characterStates[i] != null && characterStates[i].character == character)
                return characterStates[i];
        }

        return null;
    }

    private int GetStateIndexByCharacter(Character character)
    {
        if (characterStates == null || character == null)
            return -1;

        for (int i = 0; i < characterStates.Length; i++)
        {
            if (characterStates[i] != null && characterStates[i].character == character)
                return i;
        }

        return -1;
    }

    private void StartPlayerPhase()
    {
        if (battleEnded)
            return;

        if (enemyPartyManager != null && enemyPartyManager.AreAllDead())
        {
            WinBattle();
            return;
        }

        currentPhase = BattlePhase.PlayerTurn;
        CancelCurrentSelection(false);

        for (int i = 0; i < characterStates.Length; i++)
        {
            if (characterStates[i] == null || characterStates[i].character == null)
                continue;

            if (characterStates[i].character.health != null && characterStates[i].character.health.isDead)
            {
                characterStates[i].turnFinished = true;
                continue;
            }

            characterStates[i].turnFinished = false;
        }

        StartNextAvailableCharacterTurn();
    }

    private void StartEnemyPhase()
    {
        if (battleEnded)
            return;

        if (enemyPartyManager != null && enemyPartyManager.AreAllDead())
        {
            WinBattle();
            return;
        }

        currentPhase = BattlePhase.EnemyTurn;
        currentCharacterIndex = -1;
        currentAP = 0;
        CancelCurrentSelection(false);

        RefreshCharacterPanelUI();
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        Debug.Log("<color=yellow>Enemy turn başladı.</color>");

        List<EnemyCharacter> livingEnemies = enemyPartyManager != null
            ? enemyPartyManager.GetAliveMembers()
            : new List<EnemyCharacter>();

        for (int i = 0; i < livingEnemies.Count; i++)
        {
            if (battleEnded)
                yield break;

            EnemyCharacter enemy = livingEnemies[i];

            if (enemy == null || enemy.health == null || enemy.health.isDead)
                continue;

            if (enemy.ConsumeOneStunTurn())
            {
                Debug.Log($"<color=red>{enemy.EnemyName} stun olduğu için turunu pas geçti.</color>");
                LogBattleState($"{enemy.EnemyName} turn skipped because of stun");
                yield return new WaitForSeconds(enemyTurnDelay);
                continue;
            }

            if (enemy.Ability == null)
            {
                Debug.LogWarning($"<color=red>{enemy.EnemyName} için ability atanmadı.</color>");
                ExecuteEnemyBasicAttack(enemy);
                LogBattleState($"<color=red>{enemy.EnemyName} used fallback basic attack</color>");
                yield return new WaitForSeconds(enemyTurnDelay);
                continue;
            }

            enemy.Ability.Execute(enemy, this);

            if (enemy.Ability.PerformBasicAttackAfterUse)
                ExecuteEnemyBasicAttack(enemy);

            if (battleRoomCanvasUI != null)
                battleRoomCanvasUI.RefreshAll();

            LogBattleState($"<color=red>{enemy.EnemyName} finished action</color>");

            if (partyManager != null && partyManager.AreAllDead())
            {
                LoseBattle();
                yield break;
            }

            if (enemyPartyManager != null && enemyPartyManager.AreAllDead())
            {
                WinBattle();
                yield break;
            }

            yield return new WaitForSeconds(enemyTurnDelay);
        }

        StartPlayerPhase();
    }

    private void ExecuteEnemyBasicAttack(EnemyCharacter enemy)
    {
        if (enemy == null || enemy.health == null || enemy.health.isDead)
            return;

        Character target = partyManager != null ? partyManager.GetFrontAliveMemberInRange(enemy.Range) : null;
        if (target == null)
        {
            Debug.Log($"<color=red>{enemy.EnemyName} normal saldırı için hedef bulamadı.</color>");
            return;
        }

        int baseDamage = enemy.Damage;
        int appliedDamage = target.ApplyDamage(baseDamage);

        BattleDebugLogger.LogEnemyBasicAttack(
            enemy.EnemyName,
            target.name,
            baseDamage,
            appliedDamage
        );
    }

    private void StartNextAvailableCharacterTurn()
    {
        if (battleEnded)
            return;

        if (enemyPartyManager != null && enemyPartyManager.AreAllDead())
        {
            WinBattle();
            return;
        }

        int nextIndex = FindNextAvailableCharacterIndex();

        if (nextIndex == -1)
        {
            StartEnemyPhase();
            return;
        }

        currentCharacterIndex = nextIndex;
        currentAP = 1;
        CancelCurrentSelection(false);

        CharacterBattleState currentState = GetCurrentState();
        Character currentCharacter = GetCurrentCharacter();

        if (currentState == null || currentCharacter == null)
            return;

        if (currentState.hasStartedTurnBefore)
            currentState.mana += 1;
        else
            currentState.hasStartedTurnBefore = true;

        if (currentState.mana < 0)
            currentState.mana = 0;

        currentCharacter.ReduceAbilityCooldowns();

        UpdateTurnUI();
    }

    private int FindNextAvailableCharacterIndex()
    {
        if (characterStates == null)
            return -1;

        for (int i = 0; i < characterStates.Length; i++)
        {
            CharacterBattleState state = characterStates[i];

            if (state == null || state.character == null)
                continue;

            if (state.turnFinished)
                continue;

            if (state.character.health != null && state.character.health.isDead)
                continue;

            if (state.character.ConsumeSkipNextTurn())
            {
                state.turnFinished = true;
                Debug.Log($"{state.character.name} bu turu pas geçti.");
                continue;
            }

            return i;
        }

        return -1;
    }

    private CharacterBattleState GetCurrentState()
    {
        if (characterStates == null)
            return null;

        if (currentCharacterIndex < 0 || currentCharacterIndex >= characterStates.Length)
            return null;

        return characterStates[currentCharacterIndex];
    }

    private Character GetCurrentCharacter()
    {
        CharacterBattleState state = GetCurrentState();
        return state != null ? state.character : null;
    }

    private void UpdateTurnUI()
    {
        if (battleEnded)
            return;

        if (enemyPartyManager != null && enemyPartyManager.AreAllDead())
        {
            WinBattle();
            return;
        }

        if (battleRoomCanvasUI != null)
            battleRoomCanvasUI.RefreshAll();

        RefreshCharacterPanelUI();
    }

    private void RefreshCharacterPanelUI()
    {
        if (characterPanelUI != null)
            characterPanelUI.RefreshUI(this);
    }

    private bool HasCompatibleWeapon(Character character)
    {
        if (character == null)
            return false;

        Weapon weapon = character.GetEquippedWeapon();
        if (weapon == null)
            return false;

        switch (character.classType)
        {
            case ClassType.Warrior:
                return weapon.WeaponType == WeaponType.Sword || weapon.WeaponType == WeaponType.Axe;

            case ClassType.Archer:
                return weapon.WeaponType == WeaponType.Bow || weapon.WeaponType == WeaponType.Crossbow;

            case ClassType.Mage:
                return weapon.WeaponType == WeaponType.Wand || weapon.WeaponType == WeaponType.Staff;
        }

        return false;
    }

    public void OnBasicAttackClicked()
    {
        Character currentCharacter = GetCurrentCharacter();
        Weapon weapon = currentCharacter != null ? currentCharacter.GetEquippedWeapon() : null;

        if (weapon == null || weapon.basicAttackAbility == null)
            return;

        ToggleAbilitySelection(weapon.basicAttackAbility);
    }

    public void OnAbility1Clicked()
    {
        Character currentCharacter = GetCurrentCharacter();
        Weapon weapon = currentCharacter != null ? currentCharacter.GetEquippedWeapon() : null;

        if (weapon == null || weapon.ability1 == null)
            return;

        ToggleAbilitySelection(weapon.ability1);
    }

    public void OnAbility2Clicked()
    {
        Character currentCharacter = GetCurrentCharacter();
        Weapon weapon = currentCharacter != null ? currentCharacter.GetEquippedWeapon() : null;

        if (weapon == null || weapon.ability2 == null)
            return;

        ToggleAbilitySelection(weapon.ability2);
    }

    private void ToggleAbilitySelection(AbilityBase ability)
    {
        if (battleEnded)
            return;

        if (currentPhase != BattlePhase.PlayerTurn)
            return;

        Character currentCharacter = GetCurrentCharacter();
        if (currentCharacter == null || ability == null || currentAP <= 0)
            return;

        if (pendingAbility == ability && isSelectingTarget)
        {
            CancelCurrentSelection(true);
            return;
        }

        TryUseAbility(ability);
    }

    private void TryUseAbility(AbilityBase ability)
    {
        Character currentCharacter = GetCurrentCharacter();
        CharacterBattleState currentState = GetCurrentState();

        if (currentCharacter == null || currentState == null || ability == null)
            return;

        if (!ability.CanUse(currentCharacter, this))
            return;

        if (currentCharacter.IsAbilityOnCooldown(ability))
        {
            Debug.Log($"{ability.abilityName} cooldown'da. Kalan tur: {currentCharacter.GetAbilityCooldownRemaining(ability)}");
            return;
        }

        if (currentAP < ability.apCost)
            return;

        if (currentState.mana < ability.manaCost)
            return;

        if (!ability.requiresTarget)
        {
            bool success = ability.Use(currentCharacter, null, null, this);

            if (!success)
                return;

            SpendAbilityCost(ability);
            return;
        }

        pendingAbility = ability;
        isSelectingTarget = true;

        if (battleRoomCanvasUI == null)
            return;

        if (ability.targetSide == AbilityTargetSide.Enemy)
        {
            List<EnemyCharacter> validEnemyTargets = ability.GetValidEnemyTargets(currentCharacter, this);
            battleRoomCanvasUI.EnableEnemySelections(validEnemyTargets);
        }
        else if (ability.targetSide == AbilityTargetSide.Ally)
        {
            List<Character> validAllyTargets = ability.GetValidAllyTargets(currentCharacter, this);
            battleRoomCanvasUI.EnableAllySelections(validAllyTargets);
        }

        RefreshCharacterPanelUI();
    }

    private void OnSlotClicked(BattleSlotUI clickedSlot)
    {
        if (battleEnded)
            return;

        if (!isSelectingTarget || pendingAbility == null || clickedSlot == null)
            return;

        Character currentCharacter = GetCurrentCharacter();
        if (currentCharacter == null)
            return;

        bool success = false;

        if (pendingAbility.targetSide == AbilityTargetSide.Enemy && clickedSlot.CurrentEnemy != null)
            success = pendingAbility.Use(currentCharacter, clickedSlot.CurrentEnemy, null, this);
        else if (pendingAbility.targetSide == AbilityTargetSide.Ally && clickedSlot.CurrentCharacter != null)
            success = pendingAbility.Use(currentCharacter, null, clickedSlot.CurrentCharacter, this);

        if (!success)
            return;

        SpendAbilityCost(pendingAbility);
    }

    private void SpendAbilityCost(AbilityBase usedAbility)
    {
        if (battleEnded)
            return;

        CharacterBattleState currentState = GetCurrentState();
        Character currentCharacter = GetCurrentCharacter();

        if (currentState == null || usedAbility == null || currentCharacter == null)
            return;

        currentAP -= usedAbility.apCost;
        if (currentAP < 0)
            currentAP = 0;

        currentState.mana -= usedAbility.manaCost;
        if (currentState.mana < 0)
            currentState.mana = 0;

        if (usedAbility.cooldownTurns > 0)
            currentCharacter.StartAbilityCooldown(usedAbility);

        CancelCurrentSelection(false);

        if (battleRoomCanvasUI != null)
            battleRoomCanvasUI.RefreshAll();

        LogBattleState($"{currentCharacter.name} used {usedAbility.abilityName}");

        UpdateTurnUI();

        if (battleEnded)
            return;

        if (enemyPartyManager != null && enemyPartyManager.AreAllDead())
        {
            WinBattle();
            return;
        }

        if (currentAP == 0)
            EndCurrentCharacterTurn();
    }

    private void CancelCurrentSelection(bool refreshUI)
    {
        isSelectingTarget = false;
        pendingAbility = null;

        if (battleRoomCanvasUI != null)
            battleRoomCanvasUI.ClearAllSelections();

        if (refreshUI)
            UpdateTurnUI();
    }

    public void OnNextTurnClicked()
    {
        if (battleEnded)
            return;

        if (currentPhase != BattlePhase.PlayerTurn)
            return;

        CancelCurrentSelection(false);
        EndCurrentCharacterTurn();
    }

    private void EndCurrentCharacterTurn()
    {
        if (battleEnded)
            return;

        CharacterBattleState currentState = GetCurrentState();
        Character currentCharacter = GetCurrentCharacter();

        if (currentState != null)
            currentState.turnFinished = true;

        if (currentCharacter != null)
            LogBattleState($"{currentCharacter.name} turn ended");

        StartNextAvailableCharacterTurn();
    }

    public List<EnemyCharacter> GetLivingEnemies()
    {
        return enemyPartyManager != null ? enemyPartyManager.GetAliveMembers() : new List<EnemyCharacter>();
    }

    public List<Character> GetLivingAllies()
    {
        return partyManager != null ? partyManager.GetAliveMembers() : new List<Character>();
    }

    public List<EnemyCharacter> GetEnemiesInRange(Character attacker)
    {
        List<EnemyCharacter> result = new List<EnemyCharacter>();

        if (attacker == null || enemyPartyManager == null)
            return result;

        Weapon weapon = attacker.GetEquippedWeapon();
        if (weapon == null)
            return result;

        return enemyPartyManager.GetAliveMembersInRange(weapon.range);
    }

    public List<Character> GetAlliesInEnemyRange(EnemyCharacter attacker)
    {
        if (attacker == null || partyManager == null)
            return new List<Character>();

        return partyManager.GetAliveMembersInRange(attacker.Range);
    }

    public Character GetNearestLivingAllyInRange(EnemyCharacter attacker)
    {
        if (attacker == null || partyManager == null)
            return null;

        return partyManager.GetFrontAliveMemberInRange(attacker.Range);
    }

    public Character GetRandomLivingAllyInRange(EnemyCharacter attacker)
    {
        if (attacker == null || partyManager == null)
            return null;

        return partyManager.GetRandomAliveMemberInRange(attacker.Range);
    }

    public EnemyCharacter GetLowestShieldLivingEnemyAlly(EnemyCharacter source)
    {
        return enemyPartyManager != null
            ? enemyPartyManager.GetLowestShieldAliveMemberExcept(source)
            : null;
    }

    public EnemyCharacter GetEnemyBehind(EnemyCharacter targetEnemy)
    {
        return enemyPartyManager != null
            ? enemyPartyManager.GetEnemyBehind(targetEnemy)
            : null;
    }

    public int GetWeaponDamage(Character attacker)
    {
        if (attacker == null)
            return 0;

        return Random.Range(attacker.finalStats.minDamage, attacker.finalStats.maxDamage + 1);
    }

    public int GetBasicAttackDamage(Character attacker, Weapon weapon, int offClassDamage)
    {
        if (attacker == null || weapon == null)
            return 0;

        if (HasCompatibleWeapon(attacker))
            return GetWeaponDamage(attacker);

        return offClassDamage;
    }

    public int GetCurrentMana()
    {
        CharacterBattleState currentState = GetCurrentState();
        return currentState != null ? currentState.mana : 0;
    }

    public int GetCurrentAP()
    {
        return currentAP;
    }

    public Character GetActiveCharacter()
    {
        return GetCurrentCharacter();
    }

    public bool IsBattleEnded()
    {
        return battleEnded;
    }

    public bool IsPlayerTurn()
    {
        return currentPhase == BattlePhase.PlayerTurn && !battleEnded;
    }

    public string GetTurnNameForUI()
    {
        if (battleEnded)
            return "";

        Character currentCharacter = GetCurrentCharacter();

        if (currentPhase == BattlePhase.EnemyTurn)
            return "Enemy Turn";

        return currentCharacter != null ? currentCharacter.name : "";
    }

    public int GetDisplayedAP()
    {
        if (battleEnded)
            return 0;

        return currentPhase == BattlePhase.PlayerTurn ? currentAP : 0;
    }

    public int GetDisplayedMana()
    {
        if (battleEnded)
            return 0;

        CharacterBattleState currentState = GetCurrentState();
        if (currentPhase != BattlePhase.PlayerTurn || currentState == null)
            return 0;

        return Mathf.Max(0, currentState.mana);
    }

    public AbilityBase GetCurrentBasicAttackAbility()
    {
        Character currentCharacter = GetCurrentCharacter();
        Weapon weapon = currentCharacter != null ? currentCharacter.GetEquippedWeapon() : null;
        return weapon != null ? weapon.basicAttackAbility : null;
    }

    public AbilityBase GetCurrentAbility1()
    {
        Character currentCharacter = GetCurrentCharacter();
        Weapon weapon = currentCharacter != null ? currentCharacter.GetEquippedWeapon() : null;
        return weapon != null ? weapon.ability1 : null;
    }

    public AbilityBase GetCurrentAbility2()
    {
        Character currentCharacter = GetCurrentCharacter();
        Weapon weapon = currentCharacter != null ? currentCharacter.GetEquippedWeapon() : null;
        return weapon != null ? weapon.ability2 : null;
    }

    public string GetAbilityButtonText(AbilityBase ability, string fallback)
    {
        Character currentCharacter = GetCurrentCharacter();

        if (ability == null)
            return fallback;

        if (currentCharacter != null && currentCharacter.IsAbilityOnCooldown(ability))
        {
            int remaining = currentCharacter.GetAbilityCooldownRemaining(ability);
            return $"{ability.abilityName} ({remaining})";
        }

        return ability.abilityName;
    }

    public bool CanUseCurrentBasicAttack()
    {
        Character currentCharacter = GetCurrentCharacter();
        Weapon weapon = currentCharacter != null ? currentCharacter.GetEquippedWeapon() : null;

        return IsPlayerTurn()
               && currentCharacter != null
               && weapon != null
               && currentAP > 0
               && weapon.basicAttackAbility != null
               && !currentCharacter.IsAbilityOnCooldown(weapon.basicAttackAbility);
    }

    public bool CanUseCurrentAbility1()
    {
        Character currentCharacter = GetCurrentCharacter();
        Weapon weapon = currentCharacter != null ? currentCharacter.GetEquippedWeapon() : null;

        return IsPlayerTurn()
               && currentCharacter != null
               && weapon != null
               && currentAP > 0
               && weapon.ability1 != null
               && HasCompatibleWeapon(currentCharacter)
               && !currentCharacter.IsAbilityOnCooldown(weapon.ability1);
    }

    public bool CanUseCurrentAbility2()
    {
        Character currentCharacter = GetCurrentCharacter();
        Weapon weapon = currentCharacter != null ? currentCharacter.GetEquippedWeapon() : null;

        return IsPlayerTurn()
               && currentCharacter != null
               && weapon != null
               && currentAP > 0
               && weapon.ability2 != null
               && HasCompatibleWeapon(currentCharacter)
               && !currentCharacter.IsAbilityOnCooldown(weapon.ability2);
    }

    public bool CanUseNextTurnButton()
    {
        return IsPlayerTurn();
    }

    public AbilityBase GetPendingAbility()
    {
        return pendingAbility;
    }

    public bool IsSelectingTarget()
    {
        return isSelectingTarget;
    }

    private void LogBattleState(string context)
    {
        Debug.Log($"===== BATTLE STATE | {context} =====");

        LogAllCharactersState();
        LogAllEnemiesState();

        Debug.Log("====================================");
    }

    private void LogAllCharactersState()
    {
        List<Character> allies = partyManager != null
            ? partyManager.GetAllMembersForLog()
            : new List<Character>();

        Debug.Log("<color=lime>--- CHARACTERS ---</color>");

        if (allies.Count == 0)
        {
            Debug.Log("<color=lime>No characters found.</color>");
            return;
        }

        for (int i = 0; i < allies.Count; i++)
        {
            Character character = allies[i];

            if (character == null)
            {
                Debug.Log($"<color=lime>Character Slot {i}: NULL</color>");
                continue;
            }

            int currentHp = 0;
            int maxHp = 0;
            bool isDead = false;

            if (character.health != null)
            {
                currentHp = character.health.currentHealth;
                maxHp = character.health.maxHealth;
                isDead = character.health.isDead;
            }

            int currentShield = 0;
            int maxShield = 0;
            if (character.shield != null)
            {
                currentShield = character.shield.currentShield;
                maxShield = character.shield.maxShield;
            }

            Debug.Log(
                $"<color=lime>[{i}] {character.name} | HP: {currentHp}/{maxHp} | Shield: {currentShield}/{maxShield} | Dead: {isDead}</color>"
            );
        }
    }

    private void LogAllEnemiesState()
    {
        List<EnemyCharacter> enemies = enemyPartyManager != null
            ? enemyPartyManager.GetAllMembersForLog()
            : new List<EnemyCharacter>();

        Debug.Log("<color=red>--- ENEMIES ---</color>");

        if (enemies.Count == 0)
        {
            Debug.Log("<color=red>No enemies found.</color>");
            return;
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyCharacter enemy = enemies[i];

            if (enemy == null)
            {
                Debug.Log($"<color=red>Enemy Slot {i}: NULL</color>");
                continue;
            }

            int currentHp = 0;
            int maxHp = 0;
            bool isDead = false;

            if (enemy.health != null)
            {
                currentHp = enemy.health.currentHealth;
                maxHp = enemy.health.maxHealth;
                isDead = enemy.health.isDead;
            }

            int currentShield = 0;
            int maxShield = 0;

            if (enemy.shield != null)
            {
                currentShield = enemy.shield.currentShield;
                maxShield = enemy.shield.maxShield;
            }

            Debug.Log(
                $"<color=red>[{i}] {enemy.EnemyName} | HP: {currentHp}/{maxHp} | Shield: {currentShield}/{maxShield} | Dead: {isDead}</color>"
            );
        }
    }

    private int GetRandomRewardItemCountByTier(DungeonTier tier)
    {
        switch (tier)
        {
            case DungeonTier.Common:
                return Random.Range(1, 3); // 1-2
            case DungeonTier.Uncommon:
                return Random.Range(1, 4); // 1-3
            case DungeonTier.Rare:
                return Random.Range(2, 4); // 2-3
            case DungeonTier.Epic:
                return Random.Range(2, 5); // 2-4
            case DungeonTier.Legendary:
                return Random.Range(3, 6); // 3-5
            default:
                return Random.Range(1, 3); // 1-2
        }
    }
}