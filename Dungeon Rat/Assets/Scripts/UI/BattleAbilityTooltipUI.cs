using TMPro;
using UnityEngine;

public class BattleAbilityTooltipUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    private void Awake()
    {
        Hide();
    }

    public void Show(AbilityBase ability)
    {
        if (ability == null)
        {
            Hide();
            return;
        }

        if (titleText != null)
            titleText.text = string.IsNullOrWhiteSpace(ability.abilityName) ? "Ability" : ability.abilityName;

        if (descriptionText != null)
            descriptionText.text = string.IsNullOrWhiteSpace(ability.description) ? "No description." : ability.description;

        if (root != null)
            root.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}