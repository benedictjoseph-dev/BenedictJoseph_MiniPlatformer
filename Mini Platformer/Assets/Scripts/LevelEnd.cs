using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName;

    [Header("Floating Message Settings")]
    public GameObject floatingMessagePrefab;
    public Transform messageSpawnPoint;
    public Transform uiCanvas;

    private Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();

        // Ensure the portal starts with isTrigger = false
        if (col != null)
            col.isTrigger = false;
    }

    void Update()
    {
        // Unlock the portal when all coins are collected
        if (CoinManager.instance != null &&
            CoinManager.instance.coinCount >= CoinManager.instance.GetRequiredCoinCount())
        {
            if (col != null && !col.isTrigger)
                col.isTrigger = true;
        }
    }

    // When the player bumps into the portal (before all coins collected)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (CoinManager.instance != null &&
                CoinManager.instance.coinCount < CoinManager.instance.GetRequiredCoinCount())
            {
                ShowFloatingMessage();
            }
        }
    }

    // When the player walks through the portal (after isTrigger = true)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (CoinManager.instance != null &&
                CoinManager.instance.coinCount >= CoinManager.instance.GetRequiredCoinCount())
            {
                StartCoroutine(LoadNextScene());
            }
        }
    }

    void ShowFloatingMessage()
    {
        if (floatingMessagePrefab != null && uiCanvas != null)
        {
            GameObject msg = Instantiate(floatingMessagePrefab, uiCanvas);
            Vector3 spawnPos = messageSpawnPoint != null ? messageSpawnPoint.position : transform.position;
            msg.transform.position = Camera.main.WorldToScreenPoint(spawnPos);
        }
    }

    private System.Collections.IEnumerator LoadNextScene()
    {
        BackgroundMusicManager music = Object.FindAnyObjectByType<BackgroundMusicManager>();
        if (music != null)
            music.FadeOutMusic();

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(nextSceneName);
    }
}
