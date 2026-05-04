using UnityEngine;

namespace LolopupkaAnimations2D
{
public class SimpleTopDownCharacterController : MonoBehaviour
{

    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    private Rigidbody2D rb;

    private Vector2 movement;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() 
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        Debug.DrawRay(transform.position, movement, Color.cyan);
    }

    private void FixedUpdate() 
    {
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        if(movement != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, movement);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            rb.MoveRotation(rotation);
        }
    }
}
}
