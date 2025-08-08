using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName;

    [Header("Floating Message Settings")]
    public GameObject floatingMessagePrefab; // shown when player tries to exit w/o enough coins
    public Transform messageSpawnPoint;
    public Transform uiCanvas;

    [Header("Audio Settings")]
    public AudioClip deniedSFX; // sfx if player tries to exit too early
    public AudioClip successSFX; // sfx when player actually completes
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Visual Effects")]
    public ParticleSystem successParticles;

    private Collider2D col;
    private AudioSource audioSource;

    void Start()
    {
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        // start with trigger off so exit is blocked
        if (col != null)
            col.isTrigger = false;

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // when player got all coins, allow them to pass through exit trigger
        if (CoinManager.instance != null &&
            CoinManager.instance.coinCount >= CoinManager.instance.GetRequiredCoinCount())
        {
            if (col != null && !col.isTrigger)
                col.isTrigger = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Player bumped into exit but doesn’t have enough coins
        if (collision.collider.CompareTag("Player"))
        {
            if (CoinManager.instance != null &&
                CoinManager.instance.coinCount < CoinManager.instance.GetRequiredCoinCount())
            {
                PlaySFX(deniedSFX);
                ShowFloatingMessage();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player has enough coins, so complete the level
        if (collision.CompareTag("Player"))
        {
            if (CoinManager.instance != null &&
                CoinManager.instance.coinCount >= CoinManager.instance.GetRequiredCoinCount())
            {
                PlaySFX(successSFX);
                PlaySuccessParticles();
                StartCoroutine(LoadNextScene());
            }
        }
    }

    void ShowFloatingMessage()
    {
        // spawns UI message above player to tell them they need more coins
        if (floatingMessagePrefab != null && uiCanvas != null)
        {
            GameObject msg = Instantiate(floatingMessagePrefab, uiCanvas);
            Vector3 spawnPos = messageSpawnPoint != null ? messageSpawnPoint.position : transform.position;
            msg.transform.position = Camera.main.WorldToScreenPoint(spawnPos);
        }
    }

    private void PlaySuccessParticles()
    {
        if (successParticles != null)
        {
            ParticleSystem ps = Instantiate(successParticles, transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }
    }

    private System.Collections.IEnumerator LoadNextScene()
    {
        // optional fade out music if the manager exists
        BackgroundMusicManager music = Object.FindAnyObjectByType<BackgroundMusicManager>();
        if (music != null)
            music.FadeOutMusic();

        yield return new WaitForSeconds(2f); // small delay before scene switch
        SceneManager.LoadScene(nextSceneName);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, sfxVolume);
        }
    }
}
