using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class Gun : MonoBehaviour
{
    public float recoilForce = 15f;
    public int maxAmmo = 5;
    public float lineLength = 10f;

    // ❗️ [추가] 연사 속도 (0.2초에 1발 = 초당 5발)
    public float fireRate = 0.2f;

    // ❗️ [추가] 다음 발사 가능 시간
    private float nextFireTime = 0f;
    public Image GI;
    [HideInInspector]
    public int currentAmmo;

    private Rigidbody2D playerRb;
    private Transform playerTransform;
    private Camera mainCamera;

    private LineRenderer lineRenderer;
    private SpriteRenderer gunSprite;

    private Vector2 aimDirection;
    private Vector2 actualRecoilDirection;
    public TextMeshProUGUI ammo;
    void Start()
    {
        playerTransform = GetComponentInParent<Transform>();
        playerRb = GetComponentInParent<Rigidbody2D>();
        mainCamera = Camera.main;
        currentAmmo = maxAmmo;

        gunSprite = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        Vector2 mousePosition = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);

        aimDirection = (mousePosition - (Vector2)transform.position).normalized;
        actualRecoilDirection = -aimDirection;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (angle > 90f || angle < -90f)
            gunSprite.flipY = true;
        else
            gunSprite.flipY = false;

        lineRenderer.SetPosition(0, transform.position);
        Vector2 lineEndPoint = (Vector2)transform.position + (aimDirection * lineLength);
        lineRenderer.SetPosition(1, lineEndPoint);
        ammo.text = $"{currentAmmo}/{maxAmmo}";
    }

    // ❗️ [수정] Fire() 함수에 쿨다운 로직 추가
    public void Fire()
    {
        // 1. 쿨다운이 아직 안 됐거나, 총알이 없으면 발사 불가
        if (Time.time < nextFireTime || currentAmmo <= 0)
        {
            // (쿨다운은 됐는데 총알이 0일 때만 로그 띄우기)
            if (Time.time >= nextFireTime && currentAmmo <= 0)
            {
                Debug.Log(gameObject.name + " 탄약 없음!");
            }
            return; // 발사 X
        }

        // 2. 쿨다운 시간을 다음 시간으로 설정
        nextFireTime = Time.time + fireRate;

        // 3. (원래 로직) 발사
        currentAmmo--;
        playerRb.AddForce(actualRecoilDirection * recoilForce, ForceMode2D.Impulse);
        Debug.Log(gameObject.name + " 발사! 남은 탄약: " + currentAmmo);
    }

    public void CheckReload(bool playerIsGrounded)
    {
        bool isGrounded = playerIsGrounded;

        if (isGrounded && currentAmmo < maxAmmo)
        {
            currentAmmo = maxAmmo;
            Debug.Log(gameObject.name + " (바닥 감지) 재장전 완료!");
        }
    }
}