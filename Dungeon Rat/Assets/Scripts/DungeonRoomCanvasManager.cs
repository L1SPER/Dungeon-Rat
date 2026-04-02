using UnityEngine;

public class DungeonRoomCanvasManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject battleRoomCanvas;
    [SerializeField] private GameObject characterPanelCanvas;
    [SerializeField] private GameObject afterRoomCanvas;

    public void ShowBattleUI()
    {
        if (battleRoomCanvas != null) battleRoomCanvas.SetActive(true);
        if (characterPanelCanvas != null) characterPanelCanvas.SetActive(true);
        if (afterRoomCanvas != null) afterRoomCanvas.SetActive(false);
    }

    public void ShowAfterRoomUI()
    {
        if (battleRoomCanvas != null) battleRoomCanvas.SetActive(false);
        if (characterPanelCanvas != null) characterPanelCanvas.SetActive(false);
        if (afterRoomCanvas != null) afterRoomCanvas.SetActive(true);
    }

    public void HideAll()
    {
        if (battleRoomCanvas != null) battleRoomCanvas.SetActive(false);
        if (characterPanelCanvas != null) characterPanelCanvas.SetActive(false);
        if (afterRoomCanvas != null) afterRoomCanvas.SetActive(false);
    }
}