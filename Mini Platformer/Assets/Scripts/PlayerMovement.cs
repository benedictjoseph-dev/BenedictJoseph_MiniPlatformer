using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TrailRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    private float horizontalInput;
    private bool facingRight = true;

    [Header("Jump")]
    public float jumpForce = 14f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    [Header("Better Jump")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Dash")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private TrailRenderer trail;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();

        if (trail != null)
            trail.emitting = false;
    }

    void Update()
    {
        if (isDashing) return;

        // Input
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Facing direction
        if (horizontalInput != 0)
        {
            facingRight = horizontalInput > 0;
            spriteRenderer.flipX = horizontalInput > 0;
        }

        // Grounded check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Coyote time logic
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // Jump buffer logic
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // Jump
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0;
        }

        // Better jump gravity
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Dash cooldown
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        // Dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0 && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Trap"))
        {
            Die();
        }
    }
    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    IEnumerator PerformDash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        float dashDirection = facingRight ? 1f : -1f;

        // Start trail
        if (trail != null) trail.emitting = true;

        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        // Stop trail
        if (trail != null) trail.emitting = false;

        rb.gravityScale = originalGravity;
        isDashing = false;
    }
}
