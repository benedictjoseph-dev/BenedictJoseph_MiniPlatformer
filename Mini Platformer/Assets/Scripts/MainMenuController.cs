using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public AudioSource clickSound;

    public void PlayGame()
    {
        if (clickSound != null) clickSound.Play();

        // Stop background music before loading next scene
        BackgroundMusicManager music = FindAnyObjectByType<BackgroundMusicManager>();
        if (music != null) Destroy(music.gameObject);

        // Delay load to let the click sound finish
        StartCoroutine(LoadSceneAfterDelay("Level 1", 0.2f));
    }

    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
