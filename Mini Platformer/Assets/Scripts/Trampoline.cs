using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float launchForce = 20f;
    public Sprite activeSprite;
    public Sprite defaultSprite;
    public float revertDelay = 0.3f;

    private SpriteRenderer sr;
    private AudioSource audioSource;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = defaultSprite;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, launchForce);

                // Change sprite and revert back to original
                if (activeSprite != null)
                {
                    sr.sprite = activeSprite;
                    Invoke(nameof(RevertSprite), revertDelay);
                }

                // Plays trampoline sound
                if (audioSource != null)
                    audioSource.Play();
            }
        }
    }

    void RevertSprite()
    {
        sr.sprite = defaultSprite;
    }
}
