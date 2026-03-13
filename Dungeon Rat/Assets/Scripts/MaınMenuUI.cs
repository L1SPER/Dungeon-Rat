using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        GameSceneManager.Instance.LoadScene("Town");
    }

    public void OpenSettingsPanel()
    {
    }

    public void CloseSettingsPanel()
    {
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
