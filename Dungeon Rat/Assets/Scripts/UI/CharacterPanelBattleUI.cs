using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanelBattleUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text characterNameInThatTurnText;
    [SerializeField] private TMP_Text apText;
    [SerializeField] private TMP_Text manaText;

    [Header("Buttons")]
    [SerializeField] private Button basicAttackButton;
    [SerializeField] private Button ability1Button;
    [SerializeField] private Button ability2Button;
    [SerializeField] private Button nextTurnButton;

    [Header("Button Texts")]
    [SerializeField] private TMP_Text basicAttackButtonText;
    [SerializeField] private TMP_Text ability1ButtonText;
    [SerializeField] private TMP_Text ability2ButtonText;

    [Header("Tooltip")]
    [SerializeField] private BattleAbilityTooltipUI abilityTooltipUI;

    private BattleTurnManager battleTurnManager;

    public void Initialize(BattleTurnManager manager)
    {
        battleTurnManager = manager;
        RegisterButtonEvents();
        RefreshUI(manager);
    }

    private void Awake()
    {
        RegisterButtonEvents();
    }

    private void RegisterButtonEvents()
    {
        if (basicAttackButton != null)
        {
            basicAttackButton.onClick.RemoveAllListeners();
            basicAttackButton.onClick.AddListener(OnBasicAttackClicked);
        }

        if (ability1Button != null)
        {
            ability1Button.onClick.RemoveAllListeners();
            ability1Button.onClick.AddListener(OnAbility1Clicked);
        }

        if (ability2Button != null)
        {
            ability2Button.onClick.RemoveAllListeners();
            ability2Button.onClick.AddListener(OnAbility2Clicked);
        }

        if (nextTurnButton != null)
        {
            nextTurnButton.onClick.RemoveAllListeners();
            nextTurnButton.onClick.AddListener(OnNextTurnClicked);
        }
    }

    public void RefreshUI(BattleTurnManager manager)
    {
        if (manager == null)
            return;

        battleTurnManager = manager;

        if (characterNameInThatTurnText != null)
            characterNameInThatTurnText.text = battleTurnManager.GetTurnNameForUI();

        if (apText != null)
            apText.text = battleTurnManager.GetDisplayedAP().ToString();

        if (manaText != null)
            manaText.text = battleTurnManager.GetDisplayedMana().ToString();

        AbilityBase basicAttackAbility = battleTurnManager.GetCurrentBasicAttackAbility();
        AbilityBase ability1 = battleTurnManager.GetCurrentAbility1();
        AbilityBase ability2 = battleTurnManager.GetCurrentAbility2();

        if (basicAttackButtonText != null)
            basicAttackButtonText.text = battleTurnManager.GetAbilityButtonText(basicAttackAbility, "Basic Attack");

        if (ability1ButtonText != null)
            ability1ButtonText.text = battleTurnManager.GetAbilityButtonText(ability1, "Ability 1");

        if (ability2ButtonText != null)
            ability2ButtonText.text = battleTurnManager.GetAbilityButtonText(ability2, "Ability 2");

        if (basicAttackButton != null)
            basicAttackButton.interactable = battleTurnManager.CanUseCurrentBasicAttack();

        if (ability1Button != null)
            ability1Button.interactable = battleTurnManager.CanUseCurrentAbility1();

        if (ability2Button != null)
            ability2Button.interactable = battleTurnManager.CanUseCurrentAbility2();

        if (nextTurnButton != null)
            nextTurnButton.interactable = battleTurnManager.CanUseNextTurnButton();

        SetupAbilityHover(basicAttackButton, basicAttackAbility);
        SetupAbilityHover(ability1Button, ability1);
        SetupAbilityHover(ability2Button, ability2);
    }

    private void SetupAbilityHover(Button button, AbilityBase ability)
    {
        if (button == null)
            return;

        AbilityButtonHoverUI hover = button.GetComponent<AbilityButtonHoverUI>();

        if (hover == null)
            hover = button.gameObject.AddComponent<AbilityButtonHoverUI>();

        hover.Setup(ability, abilityTooltipUI);
    }

    private void OnBasicAttackClicked()
    {
        if (battleTurnManager != null)
            battleTurnManager.OnBasicAttackClicked();
    }

    private void OnAbility1Clicked()
    {
        if (battleTurnManager != null)
            battleTurnManager.OnAbility1Clicked();
    }

    private void OnAbility2Clicked()
    {
        if (battleTurnManager != null)
            battleTurnManager.OnAbility2Clicked();
    }

    private void OnNextTurnClicked()
    {
        if (battleTurnManager != null)
            battleTurnManager.OnNextTurnClicked();
    }
}