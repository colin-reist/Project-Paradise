using System;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject playerToFollow;
    [SerializeField] private bool playerIsRight;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private bool _isGrounded;
    private Rigidbody2D _rb;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        playerIsRight = playerToFollow.transform.position.x >= transform.position.x;
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        float currentSpeed = moveSpeed;
        _rb.linearVelocity = new Vector2(currentSpeed, _rb.linearVelocity.y);
    }
}
