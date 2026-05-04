using UnityEngine;

namespace LolopupkaAnimations2D
{
public class dampedTransform : MonoBehaviour
{
    public float rotationLimit = 5f; 
    public float rotationSpeed = 5f;
    public float offset;


    private Vector3 velocity = Vector3.zero;
    private Vector3 lastPosition;
    private Vector3 smoothedVelocity;
    private Vector3 lastVelocity;

    private Quaternion lastRotation;

    void Update()
    {
        TiltBody();
        calculateVelocity();
    }

    private void TiltBody()
    {
        transform.localRotation = Quaternion.Slerp(lastRotation, Quaternion.Euler(0, 0, GetTilAmmount() + offset), Time.deltaTime * 5f);
        lastRotation = transform.localRotation;
    }

    private float GetTilAmmount()
    {
        Vector3 tiltVector = velocity.normalized * rotationLimit;
        return -tiltVector.x + tiltVector.y;
    }

    private void calculateVelocity()
    {
        float smoothFactor = 0.1f;
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        smoothedVelocity = Vector3.Lerp(lastVelocity, velocity, smoothFactor);
        lastVelocity = velocity;
        lastPosition = transform.position;
    }
}
}
