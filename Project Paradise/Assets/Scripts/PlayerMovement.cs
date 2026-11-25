using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Camera Setup")]
    [Tooltip("La caméra qui gère l'affichage final à l'écran")]
    [SerializeField] private Camera displayCamera; 
    
    [Tooltip("La caméra qui suit le joueur avec Cinemachine")]
    [SerializeField] private Camera worldCamera;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Tuning")]
    [SerializeField] private float orientationDeadZone = 0.1f;

    public bool FacingRight => _facingRight;

    private static readonly int SpeedHash        = Animator.StringToHash("Speed");
    private static readonly int IsBackwardsHash  = Animator.StringToHash("IsBackwards");

    private PlayerInput _playerInput;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private Rigidbody2D _rb;
    private Animator _animator;
    private float _moveInput;
    private bool _isSprinting;
    private bool _isGrounded;
    private bool _facingRight = true;

    private void Awake()
    {
        _rb           = GetComponent<Rigidbody2D>();
        _animator     = GetComponent<Animator>();
        _playerInput  = GetComponent<PlayerInput>();
        _jumpAction   = _playerInput.actions["Jump"];
        _sprintAction = _playerInput.actions["Sprint"];
    }

    private void Update()
    {
        _moveInput  = _playerInput.actions["Move"].ReadValue<Vector2>().x;
        _isSprinting = _sprintAction.IsPressed();

        float absInput = Mathf.Abs(_moveInput);
        float animSpeed = 0f;
        if (absInput > 0.1f)
            animSpeed = _isSprinting ? 1f : 0.5f;

        // Marche en arrière si on se déplace à l'opposé de l'orientation (non applicable en sprint)
        bool isBackwards = !_isSprinting && absInput > 0.1f && ((_moveInput > 0) != _facingRight);

        _animator.SetFloat(SpeedHash, animSpeed);
        // _animator.SetBool(IsBackwardsHash, isBackwards);

        if (_jumpAction.triggered)
            Jump();
        
        HandleFlip();
    }

    private void FixedUpdate()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        float currentSpeed = _isSprinting ? runSpeed : moveSpeed;
        _rb.linearVelocity = new Vector2(_moveInput * currentSpeed, _rb.linearVelocity.y);
    }

    private void Jump()
    {
        if (_isGrounded)
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
    }

    private void HandleFlip()
    {
        if (_isSprinting && Mathf.Abs(_moveInput) > 0.1f)
        {
            bool shouldFaceRight = _moveInput > 0;
            if (shouldFaceRight != _facingRight)
                Flip();
        }
        else
        {
            Vector3 viewportPosition = displayCamera.ScreenToViewportPoint(Input.mousePosition);
            
            Vector2 mouseWorldPosition = worldCamera.ViewportToWorldPoint(viewportPosition);
            
            float horizontalDistance = mouseWorldPosition.x - transform.position.x;

            if (horizontalDistance > orientationDeadZone && !_facingRight)
            {
                Flip();
            }
            else if (horizontalDistance < -orientationDeadZone && _facingRight)
            {
                Flip();
            }
        }
    }

    private void Flip()
    {
        _facingRight = !_facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        var pivot = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pivot, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(pivot.x - orientationDeadZone, pivot.y - 0.5f, pivot.z), new Vector3(pivot.x - orientationDeadZone, pivot.y + 0.5f, pivot.z));
        Gizmos.DrawLine(new Vector3(pivot.x + orientationDeadZone, pivot.y - 0.5f, pivot.z), new Vector3(pivot.x + orientationDeadZone, pivot.y + 0.5f, pivot.z));
    }
}