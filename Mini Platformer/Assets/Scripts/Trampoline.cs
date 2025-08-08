using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float launchForce = 20f; // How high the player jumps
    public Sprite activeSprite;     // Sprite when trampoline is pressed
    public Sprite defaultSprite;    // Sprite when trampoline is idle
    public float revertDelay = 0.3f; // Time before sprite reverts back

    private SpriteRenderer sr;
    private AudioSource audioSource;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = defaultSprite; // start with the idle sprite
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only trigger if the thing colliding is the player
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Launch player upwards, keep their X speed same
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, launchForce);

                // Change sprite to �active� and switch back later
                if (activeSprite != null)
                {
                    sr.sprite = activeSprite;
                    Invoke(nameof(RevertSprite), revertDelay);
                }

                // Play boing sound, if any
                if (audioSource != null)
                    audioSource.Play();
            }
        }
    }

    void RevertSprite()
    {
        sr.sprite = defaultSprite; // go back to idle sprite
    }
}
