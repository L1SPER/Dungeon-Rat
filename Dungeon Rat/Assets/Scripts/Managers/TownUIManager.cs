using System;
using UnityEngine;
using UnityEngine.Rendering;

public class TownUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] buildings;
    [SerializeField] private GameObject townCanvas;
    [SerializeField] private GameObject townUICanvas;

    private void Start()
    {
        CloseAllPanels();
    }

    public void OpenPanel(int index)
    {
        CloseAllPanels();
        townCanvas.SetActive(false);
        townUICanvas.SetActive(true);
        buildings[index].SetActive(true);
    }

    public void ClosePanel(int index)
    {
        buildings[index].SetActive(false);
        townUICanvas.SetActive(false);
        townCanvas.SetActive(true);
    }

    public void CloseAllPanels()
    {
        foreach (GameObject building in buildings)
        {
            building.SetActive(false);
        }
        townUICanvas.SetActive(false);
        townCanvas.SetActive(true);
    }

    public void GoToEntrance()
    {
        GameSceneManager.Instance.LoadScene("Dungeon Entrance");
    }

    public void GoToMainMenu()
    {
        GameSceneManager.Instance.LoadScene("Main Menu");
    }
}
