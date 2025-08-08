using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public AudioSource clickSound;

    public void PlayGame()
    {
        if (clickSound != null) clickSound.Play();

        // kill background music obj so it doesnt carry over to the next scene
        BackgroundMusicManager music = FindAnyObjectByType<BackgroundMusicManager>();
        if (music != null) Destroy(music.gameObject);

        // lil delay so click sfx can be heard before scene switch
        StartCoroutine(LoadSceneAfterDelay("Level 1", 0.2f));
    }

    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
