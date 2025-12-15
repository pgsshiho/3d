using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;


public class AddGun : MonoBehaviour
{
    [Header("Settings")]
    public GameObject gun;          // 줍는 총 오브젝트
    public TextMeshProUGUI gungettext; // 실제 텍스트 컴포넌트

    [Header("Animation Settings")]
    public float floatSpeed = 0.5f; // 텍스트가 올라가는 속도
    public float duration = 1.5f;   // 텍스트가 유지되는 시간

    private bool hasGivenGun = false;
    private bool playerInRange = false;
    private PlayerMove player;
    private SpriteRenderer sr;

    public void Start()
    {
        gungettext.gameObject.SetActive(false);
        sr = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerMove>();
            playerInRange = true;
            // Debug.Log("플레이어 근처에 있음");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
            // Debug.Log("플레이어가 범위를 벗어남");
        }
    }

    private void Update()
    {
        if (playerInRange && !hasGivenGun && Input.GetKeyDown(KeyCode.F))
        {
            if (player != null)
            {
                if (gun.TryGetComponent(out Gun g))
                {
                    player.ObtainGun(g);
                    hasGivenGun = true;
                    gun.SetActive(false);

                    Debug.Log($"플레이어가 {gun.name} 총을 획득했습니다!");

                    if (gungettext != null)
                    {
                        StartCoroutine(ShowFloatingText(gun.name));
                    }
                }
            }
        }
    }

    // 텍스트가 위로 올라가며 사라지는 효과를 주는 코루틴
    private IEnumerator ShowFloatingText(string gunName)
    {
        sr.enabled = false;
        gungettext.text = $"{gunName} 획득";
        gungettext.gameObject.SetActive(true);
        Color originalColor = gungettext.color;
        originalColor.a = 1f; // 알파값 1(불투명)로 시작
        gungettext.color = originalColor;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            gungettext.transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

            float alpha = Mathf.Lerp(1f, 0f, timer / duration);
            gungettext.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }
        gungettext.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
