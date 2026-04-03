using UnityEngine;

public class DungeonRoomCanvasManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject battleRoomCanvas;
    [SerializeField] private GameObject characterPanelCanvas;
    [SerializeField] private GameObject afterRoomCanvas;
    [SerializeField] private GameObject restRoomCanvas;


    public void ShowBattleUI()
    {
        if (battleRoomCanvas != null) battleRoomCanvas.SetActive(true);
        if (characterPanelCanvas != null) characterPanelCanvas.SetActive(true);
        if (afterRoomCanvas != null) afterRoomCanvas.SetActive(false);
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
    }

    public void HideAll()
    {
        if (battleRoomCanvas != null) battleRoomCanvas.SetActive(false);
        if (characterPanelCanvas != null) characterPanelCanvas.SetActive(false);
        if (afterRoomCanvas != null) afterRoomCanvas.SetActive(false);
    }
    public void ShowRestRoom()
    {
        if (battleRoomCanvas != null) battleRoomCanvas.SetActive(true);
        if (characterPanelCanvas != null) characterPanelCanvas.SetActive(false);
        if (afterRoomCanvas != null) afterRoomCanvas.SetActive(false);
        if (restRoomCanvas != null) restRoomCanvas.SetActive(true);
    }
}