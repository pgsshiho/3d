using UnityEngine;
using System.Collections; // ★ 코루틴 사용

public class MovingSpike : MonoBehaviour
{
    [Header("설정")]
    public float speed = 10f;   // 밀려나는 속도
    public float moveX;
    public float moveY;
    public float returnSpeed = 3f; // ★ 천천히 돌아가는 속도

    [Header("상태")]
    public bool moved = false;  // 이동했는지 체크

    private Rigidbody2D rb;
    private Vector2 originalPos; // 처음 위치 저장
    private bool isReturning = false; // ★ 복귀 중인 상태 방지

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalPos = transform.position; // 시작 위치 기억
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MoveSpikes();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Ground"))
        {
            if (!isReturning) // ★ 중복 호출 방지
                StartCoroutine(ReturnToOriginSmooth());
        }
    }

    public void MoveSpikes()
    {
        if (moved == false)
        {
            Vector2 dir = new Vector2(moveX, moveY).normalized;
            rb.AddForce(dir * speed, ForceMode2D.Impulse);
            moved = true;
        }
    }

    private IEnumerator ReturnToOriginSmooth()
    {
        isReturning = true;

        // 1) 속도 제거
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic; // ★ 물리 영향 잠시 끔

        // 2) 부드러운 이동
        while (Vector2.Distance(transform.position, originalPos) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, originalPos, returnSpeed * Time.deltaTime);
            yield return null;
        }

        // 3) 정확한 위치 고정 & 상태 리셋
        transform.position = originalPos;
        rb.bodyType = RigidbodyType2D.Dynamic; // ★ 물리 다시 켬
        moved = false;
        isReturning = false;

        Debug.Log("천천히 원래 위치 복귀 완료");
    }
}
