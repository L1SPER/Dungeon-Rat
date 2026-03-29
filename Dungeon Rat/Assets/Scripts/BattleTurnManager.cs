using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("UI References")]
    [SerializeField] private TMP_Text characterNameInThatTurnText;
    [SerializeField] private TMP_Text apText;
    [SerializeField] private TMP_Text manaText;

    [SerializeField] private Button basicAttackButton;
    [SerializeField] private Button ability1Button;
    [SerializeField] private Button ability2Button;
    [SerializeField] private Button nextTurnButton;

    [SerializeField] private TMP_Text basicAttackButtonText;
    [SerializeField] private TMP_Text ability1ButtonText;
    [SerializeField] private TMP_Text ability2ButtonText;

    [Header("Battle UI")]
    [SerializeField] private BattleRoomCanvasUI battleRoomCanvasUI;

    [Header("Settings")]
    [SerializeField] private float enemyTurnDelay = 1f;

    private PartyManager partyManager;
    private CharacterBattleState[] characterStates;

    private BattlePhase currentPhase;
    private int currentCharacterIndex = -1;
    private int currentAP = 0;

    private bool isSelectingTarget = false;
    private AbilityBase pendingAbility = null;

    private void Awake()
    {
        partyManager = FindFirstObjectByType<PartyManager>();

        if (battleRoomCanvasUI == null)
            battleRoomCanvasUI = FindFirstObjectByType<BattleRoomCanvasUI>();

        if (basicAttackButton != null)
            basicAttackButton.onClick.AddListener(OnBasicAttackClicked);

        if (ability1Button != null)
            ability1Button.onClick.AddListener(OnAbility1Clicked);

        if (ability2Button != null)
            ability2Button.onClick.AddListener(OnAbility2Clicked);

        if (nextTurnButton != null)
            nextTurnButton.onClick.AddListener(OnNextTurnClicked);
    }

    private IEnumerator Start()
    {
        yield return null;

        if (battleRoomCanvasUI != null)
            battleRoomCanvasUI.InitializeUI();

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

        InitializeBattle();
    }

    public void InitializeBattle()
    {
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

        characterStates = new CharacterBattleState[partyMembers.Length];

        for (int i = 0; i < partyMembers.Length; i++)
        {
            if (partyMembers[i] != null)
                characterStates[i] = new CharacterBattleState(partyMembers[i]);
        }

        StartPlayerPhase();
    }

    private void StartPlayerPhase()
    {
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
        currentPhase = BattlePhase.EnemyTurn;
        currentCharacterIndex = -1;
        currentAP = 0;
        CancelCurrentSelection(false);

        if (characterNameInThatTurnText != null)
            characterNameInThatTurnText.text = "Enemy Turn";

        if (apText != null)
            apText.text = "0";

        if (manaText != null)
            manaText.text = "0";

        RefreshButtons();
        RefreshAbilityButtonTexts();
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        Debug.Log("Enemy turn başladı.");

        //List<EnemyCharacter> livingEnemies = GetLivingEnemies();

        //for (int i = 0; i < livingEnemies.Count; i++)
        //{
        //    EnemyCharacter enemy = livingEnemies[i];
        //    if (enemy == null || enemy.isDead)
        //        continue;

        //    if (enemy.ConsumeOneStunTurn())
        //    {
        //        Debug.Log($"{enemy.EnemyName} stun olduğu için turunu pas geçti.");
        //        continue;
        //    }

        //    Character target = GetNearestLivingAlly();
        //    if (target == null)
        //        break;

        //    target.TakeDamage(enemy.Damage);
        //    Debug.Log($"{enemy.EnemyName} -> {target.name} saldırdı. Damage: {enemy.Damage}");

        //    yield return new WaitForSeconds(enemyTurnDelay);
        //}

        //if (AllPlayersDead())
        //{
        //    Debug.Log("Tüm oyuncular öldü. Battle kaybedildi.");
        //    yield break;
        //}
        yield return null;
        StartPlayerPhase();
    }

    private void StartNextAvailableCharacterTurn()
    {
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
        Character currentCharacter = GetCurrentCharacter();
        CharacterBattleState currentState = GetCurrentState();

        if (currentPhase != BattlePhase.PlayerTurn || currentCharacter == null || currentState == null)
        {
            if (characterNameInThatTurnText != null)
                characterNameInThatTurnText.text = currentPhase == BattlePhase.EnemyTurn ? "Enemy Turn" : "";

            if (apText != null)
                apText.text = "0";

            if (manaText != null)
                manaText.text = "0";

            RefreshButtons();
            RefreshAbilityButtonTexts();
            return;
        }

        if (characterNameInThatTurnText != null)
            characterNameInThatTurnText.text = currentCharacter.name;

        if (apText != null)
            apText.text = currentAP.ToString();

        if (manaText != null)
            manaText.text = Mathf.Max(0, currentState.mana).ToString();

        if (battleRoomCanvasUI != null)
            battleRoomCanvasUI.RefreshAll();

        RefreshButtons();
        RefreshAbilityButtonTexts();

        if (AllEnemiesDead())
            Debug.Log("Tüm düşmanlar öldü. Battle kazanıldı.");
    }

    private void RefreshButtons()
    {
        bool isPlayerTurn = currentPhase == BattlePhase.PlayerTurn;
        Character currentCharacter = GetCurrentCharacter();
        Weapon weapon = currentCharacter != null ? currentCharacter.GetEquippedWeapon() : null;

        bool canUseBasicAttack = false;
        bool canUseAbility1 = false;
        bool canUseAbility2 = false;

        if (isPlayerTurn && currentCharacter != null && weapon != null && currentAP > 0)
        {
            canUseBasicAttack = weapon.basicAttackAbility != null && !currentCharacter.IsAbilityOnCooldown(weapon.basicAttackAbility);

            canUseAbility1 = weapon.ability1 != null
                             && HasCompatibleWeapon(currentCharacter)
                             && !currentCharacter.IsAbilityOnCooldown(weapon.ability1);

            canUseAbility2 = weapon.ability2 != null
                             && HasCompatibleWeapon(currentCharacter)
                             && !currentCharacter.IsAbilityOnCooldown(weapon.ability2);
        }

        if (basicAttackButton != null)
            basicAttackButton.interactable = canUseBasicAttack;

        if (ability1Button != null)
            ability1Button.interactable = canUseAbility1;

        if (ability2Button != null)
            ability2Button.interactable = canUseAbility2;

        if (nextTurnButton != null)
            nextTurnButton.interactable = isPlayerTurn;
    }

    private void RefreshAbilityButtonTexts()
    {
        Character currentCharacter = GetCurrentCharacter();
        Weapon weapon = currentCharacter != null ? currentCharacter.GetEquippedWeapon() : null;

        SetAbilityButtonText(basicAttackButtonText, currentCharacter, weapon != null ? weapon.basicAttackAbility : null, "Basic Attack");
        SetAbilityButtonText(ability1ButtonText, currentCharacter, weapon != null ? weapon.ability1 : null, "Ability 1");
        SetAbilityButtonText(ability2ButtonText, currentCharacter, weapon != null ? weapon.ability2 : null, "Ability 2");
    }

    private void SetAbilityButtonText(TMP_Text textComponent, Character character, AbilityBase ability, string fallback)
    {
        if (textComponent == null)
            return;

        if (ability == null)
        {
            textComponent.text = fallback;
            return;
        }

        string text = ability.abilityName;

        if (character != null && character.IsAbilityOnCooldown(ability))
        {
            int remaining = character.GetAbilityCooldownRemaining(ability);
            textComponent.text = $"{ability.abilityName} ({remaining})";
        }
        else
        {
            textComponent.text = fallback;
        }
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
    }

    private void OnSlotClicked(BattleSlotUI clickedSlot)
    {
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

        UpdateTurnUI();

        if (AllEnemiesDead())
        {
            Debug.Log("Tüm düşmanlar öldü. Battle kazanıldı.");
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
        if (currentPhase != BattlePhase.PlayerTurn)
            return;

        CancelCurrentSelection(false);
        EndCurrentCharacterTurn();
    }

    private void EndCurrentCharacterTurn()
    {
        CharacterBattleState currentState = GetCurrentState();

        if (currentState != null)
            currentState.turnFinished = true;

        StartNextAvailableCharacterTurn();
    }

    public List<EnemyCharacter> GetLivingEnemies()
    {
        List<EnemyCharacter> result = new List<EnemyCharacter>();

        if (battleRoomCanvasUI == null || battleRoomCanvasUI.CurrentEnemies == null)
            return result;

        for (int i = 0; i < battleRoomCanvasUI.CurrentEnemies.Count; i++)
        {
            EnemyCharacter enemy = battleRoomCanvasUI.CurrentEnemies[i];
            if (enemy == null || enemy.isDead)
                continue;

            result.Add(enemy);
        }

        return result;
    }

    public List<Character> GetLivingAllies()
    {
        List<Character> result = new List<Character>();

        if (partyManager == null)
            return result;

        Character[] party = partyManager.GetPartyMembers();
        if (party == null)
            return result;

        for (int i = 0; i < party.Length; i++)
        {
            if (party[i] == null || party[i].health == null || party[i].health.isDead)
                continue;

            result.Add(party[i]);
        }

        return result;
    }

    public List<EnemyCharacter> GetEnemiesInRange(Character attacker)
    {
        List<EnemyCharacter> result = new List<EnemyCharacter>();

        if (attacker == null)
            return result;

        Weapon weapon = attacker.GetEquippedWeapon();
        if (weapon == null)
            return result;

        List<EnemyCharacter> livingEnemies = GetLivingEnemies();
        int range = Mathf.Clamp(weapon.range, 1, 99);

        for (int i = 0; i < livingEnemies.Count; i++)
        {
            if (i >= range)
                break;

            result.Add(livingEnemies[i]);
        }

        return result;
    }

    public EnemyCharacter GetEnemyBehind(EnemyCharacter targetEnemy)
    {
        if (targetEnemy == null)
            return null;

        List<EnemyCharacter> livingEnemies = GetLivingEnemies();

        for (int i = 0; i < livingEnemies.Count; i++)
        {
            if (livingEnemies[i] == targetEnemy)
            {
                if (i + 1 < livingEnemies.Count)
                    return livingEnemies[i + 1];

                return null;
            }
        }

        return null;
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

    private Character GetNearestLivingAlly()
    {
        List<Character> allies = GetLivingAllies();
        if (allies.Count == 0)
            return null;

        return allies[0];
    }

    private bool AllEnemiesDead()
    {
        List<EnemyCharacter> enemies = GetLivingEnemies();
        return enemies.Count == 0;
    }

    private bool AllPlayersDead()
    {
        List<Character> allies = GetLivingAllies();
        return allies.Count == 0;
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
}