using UnityEngine;
using TMPro;

public class FloatingMessage : MonoBehaviour
{
    public float floatSpeed = 30f;
    public float duration = 2f; // how long the msg stays before gone
    public float fadeTime = 1f; // how fast it fades at the end

    private TextMeshProUGUI tmp;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();

        // make sure we have a CanvasGroup so we can control fade easily
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        StartCoroutine(FloatAndFade());
    }

    System.Collections.IEnumerator FloatAndFade()
    {
        float timer = 0f;
        Vector3 startPos = transform.position;

        // slowly float upward and fade at the end of life
        while (timer < duration)
        {
            transform.position = startPos + Vector3.up * floatSpeed * (timer / duration);

            // start fade in the last "fadeTime" seconds
            if (timer > duration - fadeTime)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, (timer - (duration - fadeTime)) / fadeTime);

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // done, so remove self
    }
}
