using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))] // On s'assure aussi d'avoir un Animator
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D _rb;
    private Animator _animator; // Référence à l'Animator
    private float _moveInput;
    private bool _isGrounded;
    private bool _facingRight = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>(); // On récupère le composant Animator
    }

    private void Update()
    {
        _moveInput = Input.GetAxisRaw("Horizontal");

        // On envoie la vitesse (en valeur absolue) à l'Animator
        _animator.SetFloat(Animator.StringToHash("Speed"), Mathf.Abs(_moveInput));

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        }

        HandleFlip();
    }

    private void FixedUpdate()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        _rb.linearVelocity = new Vector2(_moveInput * moveSpeed, _rb.linearVelocity.y);
    }

    private void HandleFlip()
    {
        if (_moveInput > 0 && !_facingRight)
        {
            Flip();
        }
        else if (_moveInput < 0 && _facingRight)
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
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}