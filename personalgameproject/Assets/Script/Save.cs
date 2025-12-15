using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // 파일에 실제로 기록될 변수들
    public bool isFirst = true;
    public string sceneName;
    public Vector3 playerPos;
    public int DeathCount = 0;

    // [중요] PlayerMove에서 접근할 변수 이름
    public List<string> collectedGunNames = new List<string>();
}

public class Save : MonoBehaviour
{
    public static Save Instance;

    // 실제 데이터가 담기는 객체
    public SaveData data = new SaveData();

    private string path;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 저장 경로 설정 (PC: AppData 폴더, 모바일: 앱 내부 저장소)
            path = Path.Combine(Application.persistentDataPath, "savefile.json");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // [기능 1] 현재 상태를 파일로 저장
    public void SaveGame(Transform playerTransform, string currentSceneName)
    {
        // 데이터 갱신
        data.playerPos = playerTransform.position;
        data.sceneName = currentSceneName;
        // collectedGunNames나 isFirst는 게임 플레이 중 실시간으로 data에 반영됨

        // JSON 변환 및 저장
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log($"게임 저장 완료! 위치: {data.playerPos}, 씬: {data.sceneName}");
        Debug.Log($"저장 경로: {path}");
    }

    // [기능 2] 파일에서 데이터 불러오기
    public bool LoadGame()
    {
        if (!File.Exists(path))
        {
            Debug.Log("저장된 파일이 없습니다.");
            return false;
        }

        string json = File.ReadAllText(path);
        data = JsonUtility.FromJson<SaveData>(json);

        Debug.Log("게임 불러오기 완료!");
        return true;
    }
}
//Save.Instance.SaveGame(player.transform, SceneManager.GetActiveScene().name);
/*public void OnClickContinue()
{
    // 1. 저장된 파일을 불러옴
    bool success = Save.Instance.LoadGame();

    if (success)
    {
        // 2. 불러오기 성공 시, 저장된 씬으로 이동
        // (씬이 로드되면 PlayerMove가 Save.Instance.data를 보고 위치를 잡음)
        SceneManager.LoadScene(Save.Instance.data.sceneName);
    }
    else
    {
        Debug.Log("이어할 데이터가 없습니다. 처음부터 시작하세요.");
    }
}*/