using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int highestStageUnlocked = 1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockNextStage(int clearedStageID)
    {
        if (clearedStageID + 1 > highestStageUnlocked)
        {
            highestStageUnlocked = clearedStageID + 1;
        }
    }
}