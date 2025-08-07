using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;

    public AudioSource audioSource;
    public AudioClip levelMusic;
    public AudioClip mainMenuMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;

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
        else
        {
            // Additonal scene music  can be handeled here
        }
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
        audioSource.volume = startVolume;
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
