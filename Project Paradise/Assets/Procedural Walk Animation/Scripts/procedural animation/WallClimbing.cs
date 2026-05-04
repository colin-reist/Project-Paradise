using UnityEngine;

/*
        __WARNING__
        This script is EXPEREMENTAL! You can try to use it but its unstable now. It will be available in future updates but for now you can just experement with it. Thanks! :)
*/
namespace LolopupkaAnimations2D
{

public class WallClimbing : MonoBehaviour
{
    [SerializeField] private float rayRot = 1f;
    [SerializeField] private float groundCheckRange = 3f;
    [SerializeField] private float smoothness = 5f;
    [SerializeField] private float innerRaysOffset = .5f;
    [SerializeField] private float outerRaysOffset = -2f;

    private Vector3 newRot;
    private Vector3 lastRot;

    void Start()
    {
        lastRot = transform.up;
    }

    void FixedUpdate()
    {
        newRot = GetUpVector();

        transform.up = Vector3.Lerp(lastRot, newRot, Time.deltaTime * smoothness);

        lastRot = transform.up;
    }

    private Vector3 GetUpVector()
    {
        Vector2 averageUp = new Vector2();
        averageUp += Physics2D.CircleCast(transform.position + transform.right * outerRaysOffset, .1f, -transform.up + transform.right * rayRot, groundCheckRange).normal;
        Debug.DrawRay(transform.position + transform.right * outerRaysOffset, (-transform.up + transform.right * rayRot) * groundCheckRange, Color.red);
        averageUp += Physics2D.CircleCast(transform.position + transform.right * outerRaysOffset, .1f, -transform.up - transform.right * rayRot, groundCheckRange).normal;
        Debug.DrawRay(transform.position - transform.right * outerRaysOffset, (-transform.up - transform.right * rayRot) * groundCheckRange, Color.red);
        averageUp += Physics2D.CircleCast(transform.position + transform.right * innerRaysOffset, .1f, -transform.up + transform.right * rayRot, groundCheckRange).normal;
        Debug.DrawRay(transform.position + transform.right * innerRaysOffset, (-transform.up + transform.right * rayRot) * groundCheckRange, Color.cyan);
        averageUp += Physics2D.CircleCast(transform.position + transform.right * innerRaysOffset, .1f, -transform.up - transform.right * rayRot, groundCheckRange).normal;
        Debug.DrawRay(transform.position - transform.right * innerRaysOffset, (-transform.up - transform.right * rayRot) * groundCheckRange, Color.cyan);

        averageUp /= 4;
        Debug.DrawRay(transform.position, averageUp, Color.black);
        return averageUp;
    }
}
}
