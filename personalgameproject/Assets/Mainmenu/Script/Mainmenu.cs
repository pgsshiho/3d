using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{
    void Update()
    {
        if (Save.Instance == null) return; // 안전

        if (Input.GetMouseButtonDown(0))
        {
            if (Save.Instance.data.isFirst)
            {
                // 첫 실행이면 튜토리얼로
                SceneManager.LoadScene("Tutorial");
            }
            else
            {
                // 이어하기
                OnClickContinue();
            }
        }
    }

    public void OnClickContinue()
    {
        bool success = Save.Instance.LoadGame();

        if (success)
        {
            SceneManager.LoadScene(Save.Instance.data.sceneName);
        }
        else
        {
            Debug.Log("이어할 데이터가 없습니다. 처음부터 시작하세요.");
        }
    }
}
