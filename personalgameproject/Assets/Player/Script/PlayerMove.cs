using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerMove : MonoBehaviour
{
    [Header("플레이어 이동")]
    public int speed = 5;
    public float jumpForce = 3f;
    public float airControlForce = 10f;
    public float maxMoveSpeed = 7f;

    private Rigidbody2D rb;
    private float horizontalInput;

    [Header("무기 관리")]
    public List<Gun> allGuns = new List<Gun>();
    public List<Gun> availableGuns = new List<Gun>();

    private Gun currentGun;
    private int currentGunIndex = -1;

    [Header("필수 연결 (플레이어)")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    private bool isGrounded = false;
    private bool isJumping = false;

    [Header("카메라 고정 관련")]
    public Transform cameraTransform;
    private Vector3 cameraDefaultLocalPos;
    private bool cameraLocked = false;

    [Header("데스 UI")]
    public GameObject Deathpanel;
    public int DeathCount;
    public TextMeshProUGUI deatht;
    public bool isdeath = false;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();

        if (cameraTransform != null)
        {
            cameraDefaultLocalPos = cameraTransform.localPosition;
        }
        else
        {
            Debug.LogWarning("Camera Transform이 연결되지 않았습니다.");
        }

        // 저장된 데이터 로드 (위치)
        if (Save.Instance != null && Save.Instance.data.sceneName == SceneManager.GetActiveScene().name)
        {
            if (Save.Instance.data.playerPos != Vector3.zero)
            {
                transform.position = Save.Instance.data.playerPos;
                Debug.Log("저장된 위치로 플레이어 이동 완료");
            }
            if (Save.Instance != null && !Save.Instance.data.isFirst)
                transform.position = Save.Instance.data.playerPos;
        }

        // 초기화: 모든 총 비활성화
        foreach (Gun gun in allGuns)
        {
            if (gun != null) gun.gameObject.SetActive(false);
        }

        // 첫 번째 총 장착
        if (availableGuns.Count > 0 && availableGuns[0] != null)
        {
            EquipGun(0);
        }

        // 데스 패널 초기화 (숨김)
        if (Deathpanel != null) Deathpanel.SetActive(false);
    }

    void Update()
    {
        // 1. 물리 상태 체크
        CheckGrounded();

        // 2. 입력 수신
        horizontalInput = Input.GetAxisRaw("Horizontal");

        HandleWeaponSwitching();
        HandleActionInput();

        // 3. 카메라 고정 로직
        if (cameraLocked && cameraTransform != null)
        {
            cameraTransform.localPosition = cameraDefaultLocalPos;
        }

        // [수정됨] 4. 사망 횟수 텍스트 갱신
        if (deatht != null)
        {
            deatht.text = $"Deaths: {DeathCount}";
        }
        if (isdeath == true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                bool success = Save.Instance.LoadGame();
                Time.timeScale = 1;
                if (success)
                {
                    SceneManager.LoadScene(Save.Instance.data.sceneName);
                }
                else
                {
                    Debug.Log("이어할 데이터가 없습니다. 처음부터 시작하세요.");
                }
            }
            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                SceneManager.LoadScene("Mainmenu");
                Time.timeScale = 1;
            }
        }

    }
    void FixedUpdate()
    {
        // Unity 6 (2023.3+) 이상: linearVelocity 사용
        // (하위 버전 사용 시 linearVelocity를 velocity로 변경하세요)
        Vector2 currentVelocity = rb.linearVelocity;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(horizontalInput * speed, currentVelocity.y);
        }
        else
        {
            rb.AddForce(new Vector2(horizontalInput * airControlForce, 0f));

            // 속도 제한 (Clamp)
            if (Mathf.Abs(rb.linearVelocity.x) > maxMoveSpeed)
            {
                float clampedX = Mathf.Sign(rb.linearVelocity.x) * maxMoveSpeed;
                rb.linearVelocity = new Vector2(clampedX, rb.linearVelocity.y);
            }
        }
    }

    void HandleWeaponSwitching()
    {
        if (availableGuns.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipGun(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2) && availableGuns.Count >= 2) EquipGun(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3) && availableGuns.Count >= 3) EquipGun(2);
    }

    void HandleActionInput()
    {
        // 점프
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // 발사
        if (Input.GetMouseButton(0) && currentGun != null && isJumping)
        {
            currentGun.Fire();
        }

        // 리로드 체크
        if (currentGun != null)
        {
            currentGun.CheckReload(isGrounded);
        }
    }

    void CheckGrounded()
    {
        if (groundCheck == null) return;

        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
        isJumping = !isGrounded;

        // 착지 순간
        if (!wasGrounded && isGrounded)
        {
            ReloadAllGuns();
        }
    }

    void EquipGun(int index)
    {
        if (index < 0 || index >= availableGuns.Count || index == currentGunIndex)
            return;

        if (currentGun != null)
        {
            currentGun.gameObject.SetActive(false);
            if (currentGun.GI != null) currentGun.GI.gameObject.SetActive(false);
        }
        currentGunIndex = index;
        currentGun = availableGuns[currentGunIndex];

        if (currentGun != null)
        {
            currentGun.gameObject.SetActive(true);
            if (currentGun.GI != null) currentGun.GI.gameObject.SetActive(true);
            Debug.Log($"무기 교체: {currentGun.gameObject.name}");
        }
    }

    void ReloadAllGuns()
    {
        foreach (Gun gun in allGuns)
        {
            if (gun != null)
                gun.CheckReload(true);
        }
        Debug.Log("모든 총 재장전 체크 완료 (착지)");
    }

    public void ObtainGun(Gun newGun)
    {
        if (newGun == null) return;

        if (!allGuns.Contains(newGun))
            allGuns.Add(newGun);

        if (!availableGuns.Contains(newGun))
        {
            availableGuns.Add(newGun);

            if (Save.Instance != null && !Save.Instance.data.collectedGunNames.Contains(newGun.gameObject.name))
            {
                Save.Instance.data.collectedGunNames.Add(newGun.gameObject.name);
            }
        }
    }

    public void Delgun(Gun targetGun)
    {
        if (targetGun == null) return;

        allGuns.Remove(targetGun);
        availableGuns.Remove(targetGun);

        Debug.Log($"총 삭제: {targetGun.gameObject.name}");

        if (currentGun == targetGun)
        {
            targetGun.gameObject.SetActive(false);
            currentGun = null;
            currentGunIndex = -1;
            Debug.Log("현재 무기를 잃어버렸습니다.");

            if (availableGuns.Count > 0)
            {
                EquipGun(0);
            }
        }
        else
        {
            if (currentGun != null)
            {
                currentGunIndex = availableGuns.IndexOf(currentGun);
            }
        }

        if (Save.Instance != null && Save.Instance.data.collectedGunNames.Contains(targetGun.gameObject.name))
        {
            Save.Instance.data.collectedGunNames.Remove(targetGun.gameObject.name);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            cameraLocked = true;
        }
        else if (collision.collider.CompareTag("Spike"))
        {
            Death(collision.transform.position);
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            cameraLocked = false;
        }
    }

    // [수정됨] 사망 처리 로직 완성
    void Death(Vector2 targetPos)
    {
        // 1. 사망 횟수 증가
        Save.Instance.data.DeathCount += 1;
        DeathCount = Save.Instance.data.DeathCount;
        Save.Instance.SaveGame(transform, SceneManager.GetActiveScene().name);
        Debug.Log($"죽음 횟수 : {DeathCount}");

        if (deatht != null)
        {
            deatht.text = $"Deaths: {DeathCount}";
        }

        // 4. 시간 정지
        Time.timeScale = 0;
        Debug.Log($"사망! 총 사망 횟수: {DeathCount}");
        isdeath = true;
        if (Deathpanel != null)
        {
            Deathpanel.SetActive(true);
        }
    }
}