using UnityEngine;
using TMPro;

public class FloatingMessage : MonoBehaviour
{
    public float floatSpeed = 30f;
    public float duration = 2f;
    public float fadeTime = 1f;

    private TextMeshProUGUI tmp;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();

        // Add a CanvasGroup for fade control if not already
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

        // Float up and fade
        while (timer < duration)
        {
            transform.position = startPos + Vector3.up * floatSpeed * (timer / duration);
            if (timer > duration - fadeTime)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, (timer - (duration - fadeTime)) / fadeTime);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
