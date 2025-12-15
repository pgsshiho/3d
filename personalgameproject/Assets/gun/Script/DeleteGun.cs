using UnityEngine;
using TMPro;
using System.Collections;
public class DeleteGun : MonoBehaviour
{
    [Header("Settings")]
    public GameObject gun;    
    public TextMeshProUGUI gungettext; 

    [Header("Animation Settings")]
    public float floatSpeed = 0.5f; // 텍스트가 올라가는 속도
    public float duration = 1.5f;   // 텍스트가 유지되는 시간

    private bool hasGivenGun = false;
    private bool playerInRange = false;
    private PlayerMove player;
    public void Start()
    {
        gungettext.gameObject.SetActive(false);
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
        // 플레이어가 범위 안에 있고 F키 누르면 삭제
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (player != null)
            {
                Gun g = gun.GetComponent<Gun>();
                if (g != null)
                {
                    player.Delgun(g);     // 리스트에서 삭제
                    gun.SetActive(false); // 오브젝트 비활성화

                    Debug.Log($"총 삭제 완료");

                    if (gungettext != null)
                    {
                        StartCoroutine(ShowFloatingText(gun.name));
                    }
                }
            }
        }
    }

    // 텍스트 효과
    private IEnumerator ShowFloatingText(string gunName)
    {
        gungettext.text = $"{gunName} 삭제됨";
        gungettext.gameObject.SetActive(true);

        Color originalColor = gungettext.color;
        originalColor.a = 1f;
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
    }
}
