using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// using DG.Tweening; // 삭제: DOTween 제거

public class SceneChanger : MonoBehaviour
{
    private static Image image;
    public static bool isFading = false;

    private void Awake()
    {
        image = GetComponent<Image>();
        StartCoroutine(FadeProcess(Color.black, Color.clear, 0.7f, () =>
        {
            gameObject.SetActive(false);
        }));

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static void BG(string sceneName)
    {
        if (isFading) return; // 이미 페이딩 중이면 중복 실행 방지

        image.gameObject.SetActive(true);
    
        MonoBehaviour instance = image.GetComponent<MonoBehaviour>();

        instance.StartCoroutine(FadeProcess(Color.clear, Color.black, 0.7f, () =>
        {

            SceneManager.LoadScene(sceneName);
        }));
    }

    // DOTween의 DOColor 대신 사용할 코루틴 함수
    private static IEnumerator FadeProcess(Color startColor, Color endColor, float duration, System.Action onComplete = null)
    {
        isFading = true;
        float time = 0f;
        image.color = startColor;

        while (time < duration)
        {
            time += Time.deltaTime;
            // Lerp를 사용하여 색상 서서히 변경
            image.color = Color.Lerp(startColor, endColor, time / duration);
            yield return null;
        }

        image.color = endColor;
        isFading = false;

        // 완료 후 실행할 행동이 있다면 실행
        onComplete?.Invoke();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Mainmenu" || scene.name == "DeathScene" || scene.name == "EndingScene") return;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}