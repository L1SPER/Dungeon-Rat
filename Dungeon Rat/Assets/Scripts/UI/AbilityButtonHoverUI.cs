using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityButtonHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private AbilityBase ability;
    private BattleAbilityTooltipUI tooltipUI;

    public void Setup(AbilityBase newAbility, BattleAbilityTooltipUI newTooltipUI)
    {
        ability = newAbility;
        tooltipUI = newTooltipUI;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipUI == null || ability == null)
            return;

        tooltipUI.Show(ability);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipUI == null)
            return;

        tooltipUI.Hide();
    }
}