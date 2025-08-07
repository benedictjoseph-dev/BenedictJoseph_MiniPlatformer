using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Patrol")]
    public float patrolDistance = 3f;
    public float speed = 2f;

    [Header("Death")]
    public GameObject deathEffect;
    public float bounceForce = 12f;
    public Sprite deadSprite;
    public float deathDelay = 0.3f;

    private Vector2 startPos;
    private bool movingRight = true;
    private SpriteRenderer sr;
    private bool isDead = false;
    private AudioSource audioSource;

    void Awake()
    {
        startPos = transform.position;
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isDead)
            Patrol();
    }

    void Patrol()
    {
        float moveDir = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * speed * moveDir * Time.deltaTime);

        if (movingRight && transform.position.x >= startPos.x + patrolDistance)
        {
            movingRight = false;
            sr.flipX = true;
        }
        else if (!movingRight && transform.position.x <= startPos.x - patrolDistance)
        {
            movingRight = true;
            sr.flipX = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();

            if (playerRb != null && collision.contacts[0].normal.y < -0.5f)
            {
                // Player stomped
                StartCoroutine(DieWithEffect());

                // Bounces player up
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
            }
            else
            {
                // Player hit from side
                PlayerMovement pm = collision.collider.GetComponent<PlayerMovement>();
                if (pm != null)
                    pm.SendMessage("Die");
            }
        }
    }

    IEnumerator DieWithEffect()
    {
        isDead = true;

        //  death effect
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Play death sound
        if (audioSource != null)
            audioSource.Play();

        // Disable enemy movement
        GetComponent<Collider2D>().enabled = false;
        speed = 0;

        // Change sprite
        if (deadSprite != null)
            sr.sprite = deadSprite;

        yield return new WaitForSeconds(deathDelay);

        Destroy(gameObject);
    }
}
