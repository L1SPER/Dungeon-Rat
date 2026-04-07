using System;
using UnityEngine;

public class DungeonRoomCanvasManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject battleRoomCanvas;
    [SerializeField] private GameObject characterPanelCanvas;
    [SerializeField] private GameObject afterRoomCanvas;
    [SerializeField] private GameObject restRoomCanvas;
    [SerializeField] private GameObject partyPreviewCanvas;

    public void ShowBattleUI()
    {
        if (battleRoomCanvas != null) battleRoomCanvas.SetActive(true);
        if (characterPanelCanvas != null) characterPanelCanvas.SetActive(true);
        if (afterRoomCanvas != null) afterRoomCanvas.SetActive(false);
        if (restRoomCanvas != null) restRoomCanvas.SetActive(false);
        if(partyPreviewCanvas != null) partyPreviewCanvas.SetActive(false);
    }

    public void ShowAfterRoomUI()
    {
        Debug.Log("ShowAfterRoomUI çağrıldı");

        if (battleRoomCanvas != null)
            Debug.Log("battleRoomCanvas: " + battleRoomCanvas.name);

        if (characterPanelCanvas != null)
            Debug.Log("characterPanelCanvas: " + characterPanelCanvas.name);

        if (afterRoomCanvas != null)
            Debug.Log("afterRoomCanvas: " + afterRoomCanvas.name);

        if (battleRoomCanvas != null) battleRoomCanvas.SetActive(false);
        if (characterPanelCanvas != null) characterPanelCanvas.SetActive(false);
        if (afterRoomCanvas != null) afterRoomCanvas.SetActive(true);
        if(restRoomCanvas != null) restRoomCanvas.SetActive(false);
        if (partyPreviewCanvas != null) partyPreviewCanvas.SetActive(false);

        AfterRoomCanvasUI afterUI = afterRoomCanvas.GetComponent<AfterRoomCanvasUI>();
        if (afterUI != null)
            afterUI.RefreshUI();
    }

    public void HideAll()
    {
        if (battleRoomCanvas != null) battleRoomCanvas.SetActive(false);
        if (characterPanelCanvas != null) characterPanelCanvas.SetActive(false);
        if (afterRoomCanvas != null) afterRoomCanvas.SetActive(false);
        if(restRoomCanvas != null) restRoomCanvas.SetActive(false);
        if (partyPreviewCanvas != null) partyPreviewCanvas.SetActive(false);
    }
    public void ShowRestRoomUI()
    {
        if (battleRoomCanvas != null) battleRoomCanvas.SetActive(false);
        if (characterPanelCanvas != null) characterPanelCanvas.SetActive(false);
        if (afterRoomCanvas != null) afterRoomCanvas.SetActive(false);
        if (restRoomCanvas != null) restRoomCanvas.SetActive(true);
        if (partyPreviewCanvas != null) partyPreviewCanvas.SetActive(true);
    }
}