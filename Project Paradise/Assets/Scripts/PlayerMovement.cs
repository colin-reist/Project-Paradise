using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    
    private PlayerInput _playerInput;
    private InputAction _jumpAction;
    private Rigidbody2D _rb;
    private Animator _animator;
    private float _moveInput;
    private bool _isGrounded;
    private bool _facingRight = true;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _playerInput = GetComponent<PlayerInput>();
        _jumpAction = _playerInput.actions["Jump"];
    }

    private void Update()
    {
        _moveInput = _playerInput.actions["Move"].ReadValue<Vector2>().x;
        _animator.SetFloat(SpeedHash, Mathf.Abs(_moveInput));
        
        if (_jumpAction.triggered)
        {
            Jump();
        }
        
        HandleFlip();
    }

    private void Jump() {
        if (_isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        _rb.linearVelocity = new Vector2(_moveInput * moveSpeed, _rb.linearVelocity.y);
    }

    private void HandleFlip()
    {
        if (_moveInput > 0 && !_facingRight || _moveInput < 0 && _facingRight)
        {
            Flip();
        }
    }
    

    private void Flip()
    {
        _facingRight = !_facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck is null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}