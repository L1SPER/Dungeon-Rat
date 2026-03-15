using UnityEngine;
using UnityEngine.EventSystems;

public class PartyBackgroundClickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PartyPreviewUI partyPreviewUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        partyPreviewUI.ClearSelection();
    }
}