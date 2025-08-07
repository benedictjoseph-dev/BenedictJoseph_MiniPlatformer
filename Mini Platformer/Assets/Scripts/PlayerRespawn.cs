using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public static PlayerRespawn Instance;

    private Vector2 respawnPoint;
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }

        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // Set initial respawn point to player's starting position
        respawnPoint = transform.position;
    }

    public void SetCheckpoint(Vector2 newCheckpoint)
    {
        respawnPoint = newCheckpoint;
    }

    public void Respawn()
    {
        // Fully disable physics & movement
        playerMovement.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1f;

        // Move player to checkpoint
        transform.position = respawnPoint;

        // Now re-enable everything properly
        playerMovement.enabled = true;
        playerMovement.ResetState();

        // Optional: make sure sprite is visible again (if faded or hidden during death)
        if (sr != null)
            sr.enabled = true;
    }
}
