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
    public float coyoteTime = 0.2f; // little grace period after leaving ground
    public float jumpBufferTime = 0.2f; // buffer to catch early jump press
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
    private CinemachineImpulseSource impulseSource; // *for future development*
    private AudioSource audioSource;
    private float originalGravityScale;

    void Awake()
    {
        // cache refs for performance & readability
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        audioSource = GetComponent<AudioSource>();
        originalGravityScale = rb.gravityScale;

        if (trail != null)
            trail.emitting = false; // keep dash trail off at start
    }

    void Update()
    {
        if (isDashing) return; // lock controls during dash

        HandleInput();
        HandleGroundCheck();
        HandleJump();
        HandleGravity();
        HandleDashInput();
    }

    void FixedUpdate()
    {
        // movement physics updates here
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // flip sprite based on direction
        if (horizontalInput != 0)
        {
            facingRight = horizontalInput > 0;
            spriteRenderer.flipX = facingRight;
        }

        // start jump buffer on press
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
            coyoteTimeCounter = coyoteTime; // reset coyote timer
            jumpUsed = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void HandleJump()
    {
        // jump if buffer + coyote time still active
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !jumpUsed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0;
            jumpUsed = true;
        }
    }

    void HandleGravity()
    {
        // faster fall
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // cut jump short if space released
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
            audioSource.Play(); // dash sfx

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
        impulseSource?.GenerateImpulse(); // lil camera shake (*for future development*)
        StartCoroutine(RespawnWithTinyDelay());
    }

    IEnumerator RespawnWithTinyDelay()
    {
        rb.linearVelocity = Vector2.zero;
        enabled = false; // stop movement
        yield return new WaitForSeconds(0.05f); // tiny delay so it feels snappy
        PlayerRespawn.Instance.Respawn();
    }

    public void ResetState()
    {
        // reset all movement/dash/jump variables to default
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
