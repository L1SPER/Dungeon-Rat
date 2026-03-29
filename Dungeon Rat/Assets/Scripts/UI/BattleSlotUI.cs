using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleSlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text shieldText;

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
    public bool HasEnemy => currentEnemy != null && !currentEnemy.isDead;
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

        if (character == null)
        {
            ClearSlot();
            return;
        }

        nameText.text = character.name;
        unitImage.sprite = character.characterSprite;
        unitImage.enabled = character.characterSprite != null;

        RefreshStatsUI();

        SetSelectable(false);
        SetSelected(false);
    }

    public void SetEnemy(EnemyCharacter enemy)
    {
        currentCharacter = null;
        currentEnemy = enemy;

        if (enemy == null || enemy.enemyData == null || enemy.isDead)
        {
            ClearSlot();
            return;
        }

        nameText.text = enemy.EnemyName;
        unitImage.sprite = enemy.Sprite;
        unitImage.enabled = enemy.Sprite != null;

        RefreshStatsUI();

        SetSelectable(false);
        SetSelected(false);
    }

    public void RefreshStatsUI()
    {
        if (currentCharacter != null)
        {
            if (healthText != null)
                healthText.text = $"{currentCharacter.health.currentHealth}/{currentCharacter.health.maxHealth}";

            if (shieldText != null)
                shieldText.text = $"Shield: {currentCharacter.currentShield}";
        }
        else if (currentEnemy != null)
        {
            if (healthText != null)
                healthText.text = $"{currentEnemy.currentHealth}/{currentEnemy.MaxHealth}";

            if (shieldText != null)
                shieldText.text = $"Shield: {currentEnemy.currentShield}";
        }
        else
        {
            if (healthText != null)
                healthText.text = "";

            if (shieldText != null)
                shieldText.text = "";
        }
    }

    public void ClearSlot()
    {
        currentCharacter = null;
        currentEnemy = null;

        nameText.text = "";

        if (healthText != null)
            healthText.text = "";

        if (shieldText != null)
            shieldText.text = "";

        unitImage.sprite = null;
        unitImage.enabled = false;

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