using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public Sprite activatedSprite;
    public Sprite defaultSprite;
    public AudioClip checkpointSFX;

    private bool isActivated = false;
    private SpriteRenderer sr;
    private AudioSource audioSource;

    private void Awake()
    {
        // Grab SpriteRenderer and start with the default sprite
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = defaultSprite;

        // Make sure we have an AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only trigger if player hits it and it’s not already activated
        if (!isActivated && collision.CompareTag("Player"))
        {
            isActivated = true;
            sr.sprite = activatedSprite;

            // Play sound
            if (checkpointSFX != null)
                audioSource.PlayOneShot(checkpointSFX);

            // Tell PlayerRespawn where to respawn
            PlayerRespawn.Instance.SetCheckpoint(transform.position);
        }
    }
}
