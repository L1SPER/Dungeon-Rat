using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleSlotUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text shieldText;

    [Header("Sliders")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider shieldSlider;

    [Header("Optional UI Roots")]
    [SerializeField] private GameObject healthUI;
    [SerializeField] private GameObject shieldUI;

    [Header("Visuals")]
    [SerializeField] private Image unitImage;
    [SerializeField] private Button slotButton;
    [SerializeField] private GameObject selectedVisual;
    [SerializeField] private GameObject selectableVisual;

    private Character currentCharacter;
    private EnemyCharacter currentEnemy;

    private Action<BattleSlotUI> onSlotClicked;
    private bool isSelectable;

    public Character CurrentCharacter => currentCharacter;
    public EnemyCharacter CurrentEnemy => currentEnemy;
    public bool HasEnemy => currentEnemy != null && !currentEnemy.health.isDead;
    public bool HasCharacter => currentCharacter != null && currentCharacter.health != null && !currentCharacter.health.isDead;

    private void Awake()
    {
        if (slotButton != null)
            slotButton.onClick.AddListener(OnSlotClicked);
    }

    public void SetCharacter(Character character)
    {
        currentCharacter = character;
        currentEnemy = null;

        if (character == null || character.health == null || character.health.isDead || character.health.currentHealth <= 0)
        {
            ClearSlot();
            return;
        }

        if (nameText != null)
            nameText.text = character.name;

        if (unitImage != null)
        {
            unitImage.sprite = character.characterSprite;
            unitImage.enabled = character.characterSprite != null;
        }

        RefreshStatsUI();

        SetSelectable(false);
        SetSelected(false);
    }

    public void SetEnemy(EnemyCharacter enemy)
    {
        currentCharacter = null;
        currentEnemy = enemy;

        if (enemy == null || enemy.enemyData == null || enemy.health.isDead || enemy.health.currentHealth <= 0)
        {
            ClearSlot();
            return;
        }

        if (nameText != null)
            nameText.text = enemy.EnemyName;

        if (unitImage != null)
        {
            unitImage.sprite = enemy.Sprite;
            unitImage.enabled = enemy.Sprite != null;
        }

        RefreshStatsUI();

        SetSelectable(false);
        SetSelected(false);
    }

    public void RefreshStatsUI()
    {
        if (currentCharacter != null)
        {
            RefreshCharacterStatsUI();
            return;
        }

        if (currentEnemy != null)
        {
            RefreshEnemyStatsUI();
            return;
        }

        ClearStatsUI();
    }

    private void RefreshCharacterStatsUI()
    {
        if (currentCharacter == null || currentCharacter.health == null)
        {
            ClearSlot();
            return;
        }

        if (currentCharacter.health.isDead || currentCharacter.health.currentHealth <= 0)
        {
            ClearSlot();
            return;
        }

        int currentHp = currentCharacter.health.currentHealth;
        int maxHp = currentCharacter.health.maxHealth;

        int currentShield = currentCharacter.shield != null ? currentCharacter.shield.currentShield : 0;
        int maxShield = currentCharacter.shield != null ? currentCharacter.shield.maxShield : 0;

        SetHealthUI(currentHp, maxHp);
        SetShieldUI(currentShield, maxShield);
    }

    private void RefreshEnemyStatsUI()
    {
        if (currentEnemy == null)
        {
            ClearSlot();
            return;
        }

        if (currentEnemy.health.isDead || currentEnemy.health.currentHealth <= 0)
        {
            ClearSlot();
            return;
        }

        int currentHp = currentEnemy.health.currentHealth;
        int maxHp = currentEnemy.health.maxHealth;

        int currentShield = currentEnemy.shield.currentShield;
        int maxShield = currentEnemy.shield.maxShield;

        SetHealthUI(currentHp, maxHp);
        SetShieldUI(currentShield, maxShield);
    }

    private void SetHealthUI(int current, int max)
    {
        max = Mathf.Max(1, max);
        current = Mathf.Clamp(current, 0, max);

        if (healthText != null)
            healthText.text = $"{current}/{max}";

        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }

        if (healthUI != null)
            healthUI.SetActive(true);
    }

    private void SetShieldUI(int current, int max)
    {
        max = Mathf.Max(0, max);
        current = Mathf.Clamp(current, 0, max);

        bool showShield = max > 0 || current > 0;

        if (shieldUI != null)
            shieldUI.SetActive(showShield);

        if (shieldText != null)
            shieldText.text = showShield ? $"{current}/{max}" : "";

        if (shieldSlider != null)
        {
            shieldSlider.minValue = 0;
            shieldSlider.maxValue = Mathf.Max(1, max);
            shieldSlider.value = current;
            shieldSlider.gameObject.SetActive(showShield);
        }
    }

    private void ClearStatsUI()
    {
        if (healthText != null)
            healthText.text = "";

        if (shieldText != null)
            shieldText.text = "";

        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = 1;
            healthSlider.value = 0;
        }

        if (shieldSlider != null)
        {
            shieldSlider.minValue = 0;
            shieldSlider.maxValue = 1;
            shieldSlider.value = 0;
            shieldSlider.gameObject.SetActive(false);
        }

        if (healthUI != null)
            healthUI.SetActive(false);

        if (shieldUI != null)
            shieldUI.SetActive(false);
    }

    public void ClearSlot()
    {
        currentCharacter = null;
        currentEnemy = null;

        if (nameText != null)
            nameText.text = "";

        if (unitImage != null)
        {
            unitImage.sprite = null;
            unitImage.enabled = false;
        }

        ClearStatsUI();

        SetSelectable(false);
        SetSelected(false);
    }

    public void ConfigureClick(Action<BattleSlotUI> clickCallback)
    {
        onSlotClicked = clickCallback;
    }

    public void SetSelectable(bool value)
    {
        isSelectable = value;

        if (slotButton != null)
            slotButton.interactable = value;

        if (selectableVisual != null)
            selectableVisual.SetActive(value);
    }

    public void SetSelected(bool value)
    {
        if (selectedVisual != null)
            selectedVisual.SetActive(value);
    }

    private void OnSlotClicked()
    {
        if (!isSelectable)
            return;

        onSlotClicked?.Invoke(this);
    }
}