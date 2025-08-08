using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Patrol")]
    public float patrolDistance = 3f;
    public float speed = 2f;

    [Header("Death")]
    public GameObject deathEffect;
    public float bounceForce = 12f; // how much the player bounces after stomping
    public Sprite deadSprite;
    public float deathDelay = 0.3f; // wait before destroying enemy

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
        // move back and forth between startPos ± patrolDistance
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
                // stomped from above
                StartCoroutine(DieWithEffect());

                // give player a bounce after kill
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
            }
            else
            {
                // if the player hits from the side, the player dies
                PlayerMovement pm = collision.collider.GetComponent<PlayerMovement>();
                if (pm != null)
                    pm.SendMessage("Die");
            }
        }
    }

    IEnumerator DieWithEffect()
    {
        isDead = true;

        // spawn vfx if set
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // play sfx if audio attached
        if (audioSource != null)
            audioSource.Play();

        // stop any further interaction
        GetComponent<Collider2D>().enabled = false;
        speed = 0;

        // swap to dead sprite
        if (deadSprite != null)
            sr.sprite = deadSprite;

        yield return new WaitForSeconds(deathDelay);

        Destroy(gameObject);
    }
}
