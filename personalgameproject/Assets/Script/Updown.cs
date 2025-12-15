using UnityEngine;

public class Updown : MonoBehaviour
{
    public float amplitude = 0.2f; // How high/low it moves
    public float frequency = 1f;   // How fast it moves

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

        // Apply the position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}