using UnityEngine;

namespace LolopupkaAnimations2D
{

public class SimplePlatformerCharacterController : MonoBehaviour
{
    private float horizontal;
    [SerializeField]private float speed = 8f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }
}
}