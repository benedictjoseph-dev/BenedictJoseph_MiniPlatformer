using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Sprite activatedSprite;
    public Sprite defaultSprite;
    public AudioClip checkpointSFX;

    private bool isActivated = false;
    private SpriteRenderer sr;
    private AudioSource audioSource;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = defaultSprite;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivated && collision.CompareTag("Player"))
        {
            isActivated = true;
            sr.sprite = activatedSprite;

            // Plays the checkpoint SFX
            if (checkpointSFX != null)
                audioSource.PlayOneShot(checkpointSFX);

            PlayerRespawn.Instance.SetCheckpoint(transform.position);
        }
    }
}
