using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(AudioSource))]
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
    private bool jumpUsed;

    [Header("Better Jump")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Dash")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing;
    private float dashCooldownTimer;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private TrailRenderer trail;
    private CinemachineImpulseSource impulseSource;
    private AudioSource audioSource;
    private float originalGravityScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        audioSource = GetComponent<AudioSource>();
        originalGravityScale = rb.gravityScale;

        if (trail != null)
            trail.emitting = false;
    }

    void Update()
    {
        if (isDashing) return;

        HandleInput();
        HandleGroundCheck();
        HandleJump();
        HandleGravity();
        HandleDashInput();
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput != 0)
        {
            facingRight = horizontalInput > 0;
            spriteRenderer.flipX = facingRight;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    void HandleGroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            jumpUsed = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void HandleJump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !jumpUsed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0;
            jumpUsed = true;
        }
    }

    void HandleGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void HandleDashInput()
    {
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0 && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    IEnumerator PerformDash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        float dashDirection = facingRight ? 1f : -1f;

        if (audioSource != null)
            audioSource.Play();

        if (trail != null)
            trail.emitting = true;

        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        if (trail != null)
            trail.emitting = false;

        rb.gravityScale = originalGravity;
        isDashing = false;
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
        impulseSource?.GenerateImpulse();
        StartCoroutine(RespawnWithTinyDelay());
    }

    IEnumerator RespawnWithTinyDelay()
    {
        rb.linearVelocity = Vector2.zero;
        enabled = false;
        yield return new WaitForSeconds(0.05f);
        PlayerRespawn.Instance.Respawn();
    }

    public void ResetState()
    {
        isDashing = false;
        dashCooldownTimer = 0f;
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
        isGrounded = false;
        jumpUsed = false;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravityScale;

        if (trail != null)
            trail.emitting = false;
    }
}
