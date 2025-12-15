using UnityEngine;
using UnityEngine.SceneManagement;

public class gostageview : MonoBehaviour
{
    SceneChanger sc;
    private bool playerInRange = false;
    private PlayerMove player;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        // 플레이어가 범위 안에 있고 F키 누르면 삭제
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (player != null)
            {
                Debug.Log("WorldMap으로 이동");
                Save.Instance.data.isFirst = false;

                // 플레이어 참조
                PlayerMove player = FindObjectOfType<PlayerMove>();

                // Stage1로 넘어가기 전에 저장
                Save.Instance.SaveGame(player.transform, "WorldMap");
                SceneManager.LoadScene("WorldMap");
            }
        }
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
}
