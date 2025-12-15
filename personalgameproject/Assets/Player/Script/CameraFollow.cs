using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // 플레이어
    public float smoothSpeed = 5f;  // 부드럽게 따라오는 속도
    public Vector2 minPos;          // 맵 왼쪽 아래
    public Vector2 maxPos;          // 맵 오른쪽 위

    void LateUpdate()
    {
        if (target == null) return;

        // 따라갈 목표 위치
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        // 맵 경계 안으로 Clamp
        float clampedX = Mathf.Clamp(desiredPosition.x, minPos.x, maxPos.x);
        float clampedY = Mathf.Clamp(desiredPosition.y, minPos.y, maxPos.y);

        Vector3 clampedPosition = new Vector3(clampedX, clampedY, desiredPosition.z);

        // 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, clampedPosition, Time.deltaTime * smoothSpeed);
    }
}
