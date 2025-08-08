using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip levelMusic;
    public AudioClip mainMenuMusic;

    private void Awake()
    {
        // Make sure there’s only one of these (singleton)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // keep it when scenes change

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Play music for whatever scene we start in
        PlayCorrectMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayCorrectMusicForScene(scene.name);
    }

    private void PlayCorrectMusicForScene(string sceneName)
    {
        if (sceneName == "Main Menu")
        {
            PlayMainMenuMusic();
        }
        else if (sceneName == "Level 1" || sceneName == "Level 2")
        {
            PlayLevelMusic();
        }
        // Add more if you have extra scenes with unique tracks
    }

    public void PlayMainMenuMusic()
    {
        if (audioSource.clip != mainMenuMusic || !audioSource.isPlaying)
        {
            audioSource.clip = mainMenuMusic;
            audioSource.loop = true;
            audioSource.volume = 1f;
            audioSource.Play();
        }
    }

    public void PlayLevelMusic()
    {
        if (audioSource.clip != levelMusic || !audioSource.isPlaying)
        {
            audioSource.clip = levelMusic;
            audioSource.loop = true;
            audioSource.volume = 1f;
            audioSource.Play();
        }
    }

    public void FadeOutMusic(float fadeDuration = 1f)
    {
        StartCoroutine(FadeOutCoroutine(fadeDuration));
    }

    private System.Collections.IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // reset for next time
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
