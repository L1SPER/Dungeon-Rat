using UnityEngine;

public class DungeonEntranceUI : MonoBehaviour
{
    public void GoBackTown()
    {
        GameSceneManager.Instance.LoadScene("Town");
    }
    public void EnterDungeon()
    {
        GameSceneManager.Instance.LoadScene("Dungeon");
    }
}
