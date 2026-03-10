using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance;

    private void Awake()
    {
        if(Instance== null&& Instance!= this)
        {
            Destroy(gameObject);
            return;
            
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

}
