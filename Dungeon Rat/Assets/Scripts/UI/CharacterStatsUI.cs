using TMPro;
using UnityEngine;

public class CharacterStatsUI : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text armorText;
    [SerializeField] private TMP_Text shieldText;

    [SerializeField] private TMP_Text strengthText;
    [SerializeField] private TMP_Text agilityText;
    [SerializeField] private TMP_Text intelligenceText;

    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text critChanceText;
    [SerializeField] private TMP_Text critDamageText;
    public void SetStats(Character character)
    {
        if (character == null)
        {
            SetEmptyStats();
            return;
        }
        healthText.text = character.finalStats.health.ToString();
        armorText.text = character.finalStats.armor.ToString();
        shieldText.text = character.finalStats.shield.ToString();

        strengthText.text = character.finalStats.strength.ToString();
        agilityText.text = character.finalStats.agility.ToString();
        intelligenceText.text = character.finalStats.intelligence.ToString();

        damageText.text = character.finalStats.minDamage.ToString() + " - " + character.finalStats.maxDamage.ToString();
        critChanceText.text = character.finalStats.critChance.ToString();
        critDamageText.text = character.finalStats.critDamage.ToString();
    }

    public void SetEmptyStats()
    {
        healthText.text = "-";
        armorText.text = "-";
        shieldText.text = "-";

        strengthText.text = "-";
        agilityText.text = "-";
        intelligenceText.text = "-";

        damageText.text = "-";
        critChanceText.text = "-";
        critDamageText.text = "-";
    }
}
