using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField, Range(0f, 1f)] private float airControlMultiplier = 0.7f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpHorizontalForce = 8f;
    [SerializeField] private float wallJumpVerticalForce = 12f;
    [SerializeField] private float wallSlideSpeed = 2.5f;
    [SerializeField] private float wallJumpLockTime = 0.5f;

    [Header("Environment Probe")]
    [SerializeField] private Transform probeOrigin;
    [SerializeField] private float probeRadius = 0.18f;
    [SerializeField] private float groundProbeDistance = 0.25f;
    [SerializeField] private float wallProbeDistance = 0.35f;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField, Range(0f, 1f)] private float groundNormalThreshold = 0.45f;
    [SerializeField, Range(0f, 1f)] private float wallNormalThreshold = 0.4f;

    private Rigidbody2D rb;
    private float moveInput;
    private bool facingRight = true;

    private bool isGrounded;
    private bool isOnWall;
    private Vector2 wallNormal;

    private float wallJumpLockTimer;
    private int forcedHorizontalDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        UpdateSurfaceState();
        UpdateWallJumpLock();
        HandleJumpInput();
        HandleFlip();
    }

    private void FixedUpdate()
    {
        float control = isGrounded ? 1f : airControlMultiplier;
        float horizontalInput = forcedHorizontalDir != 0 ? forcedHorizontalDir : moveInput;

        Vector2 velocity = rb.linearVelocity;
        velocity.x = horizontalInput * moveSpeed * control;

        if (isOnWall && !isGrounded)
        {
            velocity.y = Mathf.Max(velocity.y, -wallSlideSpeed);
        }

        rb.linearVelocity = velocity;
    }

    private void UpdateSurfaceState()
    {
        isGrounded = false;
        isOnWall = false;
        wallNormal = Vector2.zero;

        if (Probe(Vector2.down, groundProbeDistance, out RaycastHit2D groundHit) &&
            groundHit.normal.y >= groundNormalThreshold)
        {
            isGrounded = true;
            forcedHorizontalDir = 0;
            wallJumpLockTimer = 0f;
        }

        bool leftWall = Probe(Vector2.left, wallProbeDistance, out RaycastHit2D leftHit) &&
                        Mathf.Abs(leftHit.normal.x) >= wallNormalThreshold;
        bool rightWall = Probe(Vector2.right, wallProbeDistance, out RaycastHit2D rightHit) &&
                         Mathf.Abs(rightHit.normal.x) >= wallNormalThreshold;

        if (!isGrounded && (leftWall || rightWall))
        {
            isOnWall = true;
            wallNormal = leftWall ? leftHit.normal : rightHit.normal;
        }
    }

    private void UpdateWallJumpLock()
    {
        if (wallJumpLockTimer > 0f)
        {
            wallJumpLockTimer -= Time.deltaTime;
            if (wallJumpLockTimer <= 0f)
            {
                wallJumpLockTimer = 0f;
                forcedHorizontalDir = 0;
            }
        }
    }

    private void HandleJumpInput()
    {
        if (!Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.W))
            return;

        Vector2 velocity = rb.linearVelocity;

        if (isGrounded)
        {
            velocity.y = jumpForce;
        }
        else if (isOnWall)
        {
            Debug.Log("Saute du mur" + isOnWall);
            float horizontalDir = Mathf.Abs(wallNormal.x) > 0.01f
                ? Mathf.Sign(wallNormal.x)
                : (facingRight ? 1f : -1f);

            velocity.x = horizontalDir * wallJumpHorizontalForce;
            velocity.y = wallJumpVerticalForce;

            forcedHorizontalDir = horizontalDir > 0f ? 1 : -1;
            wallJumpLockTimer = wallJumpLockTime;
        }
        else
        {
            return;
        }

        rb.linearVelocity = velocity;
    }

    private void HandleFlip()
    {
        float displayInput = forcedHorizontalDir != 0 ? forcedHorizontalDir : moveInput;

        if (displayInput > 0f && !facingRight)
            Flip();
        else if (displayInput < 0f && facingRight)
            Flip();
    }

    private bool Probe(Vector2 direction, float distance, out RaycastHit2D hit)
    {
        Vector2 origin = probeOrigin != null ? (Vector2)probeOrigin.position : rb.position;
        hit = Physics2D.CircleCast(origin, probeRadius, direction, distance, environmentLayer);
        return hit.collider != null;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (probeOrigin == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(probeOrigin.position + Vector3.down * groundProbeDistance, probeRadius);
        Gizmos.DrawWireSphere(probeOrigin.position + Vector3.left * wallProbeDistance, probeRadius);
        Gizmos.DrawWireSphere(probeOrigin.position + Vector3.right * wallProbeDistance, probeRadius);
    }
}
