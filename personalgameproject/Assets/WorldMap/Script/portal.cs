using UnityEngine;
using UnityEngine.SceneManagement;

public class portal : MonoBehaviour
{
    public string destinationSceneName;
    public int stageID;

    private bool isPlayerNear = false;
    public bool isopen = false;

    public Sprite openSprite;
    public Sprite closedSprite;
    private SpriteRenderer mySpriteRenderer;
    private bool lastIsOpenState;
    PlayerMove player;
    void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        if (GameManager.instance != null)
        {
            isopen = (stageID <= GameManager.instance.highestStageUnlocked);
        }
        else
        {
            Debug.LogWarning("GameManager가 씬에 없습니다!");
            isopen = false;
        }

        UpdatePortalSprite();
        lastIsOpenState = isopen;
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.UpArrow) && isopen == true)
        {
            SceneManager.LoadScene(destinationSceneName);
            Save.Instance.SaveGame(player.transform, SceneManager.GetActiveScene().name);
        }

        if (GameManager.instance != null)
        {
            isopen = (stageID <= GameManager.instance.highestStageUnlocked);
        }

        if (isopen != lastIsOpenState)
        {
            UpdatePortalSprite();
            lastIsOpenState = isopen;
        }
    }

    void UpdatePortalSprite()
    {
        if (mySpriteRenderer == null) return;
        mySpriteRenderer.sprite = isopen ? openSprite : closedSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { isPlayerNear = true; }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { isPlayerNear = false; }
    }
}