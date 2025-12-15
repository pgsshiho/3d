using UnityEngine;
using System.Collections;

public class Sign : MonoBehaviour
{
    public GameObject tutorial_panel;
    private CanvasGroup canvasGroup;
    private RectTransform panelRect;
    private bool isPlayerNear = false;

    [Header("효과 설정")]
    public float fadeDuration = 0.4f;   // 투명도 전환 속도
    public float scaleDuration = 0.3f;  // 크기 전환 속도
    public Vector3 shownScale = Vector3.one;      // 보여질 때 크기
    public Vector3 hiddenScale = new Vector3(0.8f, 0.8f, 1f); // 숨길 때 크기

    void Start()
    {
        // CanvasGroup 가져오기 또는 추가
        canvasGroup = tutorial_panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = tutorial_panel.AddComponent<CanvasGroup>();

        // RectTransform 가져오기
        panelRect = tutorial_panel.GetComponent<RectTransform>();

        // 초기 상태
        canvasGroup.alpha = 0f;
        panelRect.localScale = hiddenScale;
        tutorial_panel.SetActive(false);
    }

    void Update()
    {
        // 상태 변화 감지 후 애니메이션 실행
        if (isPlayerNear && !tutorial_panel.activeSelf)
        {
            tutorial_panel.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(ShowPanel());
        }
        else if (!isPlayerNear && tutorial_panel.activeSelf)
        {
            StopAllCoroutines();
            StartCoroutine(HidePanel());
        }
    }

    IEnumerator ShowPanel()
    {
        float time = 0f;
        float startAlpha = canvasGroup.alpha;
        Vector3 startScale = panelRect.localScale;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, t);
            panelRect.localScale = Vector3.Lerp(startScale, shownScale, t);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        panelRect.localScale = shownScale;
    }

    IEnumerator HidePanel()
    {
        float time = 0f;
        float startAlpha = canvasGroup.alpha;
        Vector3 startScale = panelRect.localScale;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            panelRect.localScale = Vector3.Lerp(startScale, hiddenScale, t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        panelRect.localScale = hiddenScale;
        tutorial_panel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = false;
    }
}
