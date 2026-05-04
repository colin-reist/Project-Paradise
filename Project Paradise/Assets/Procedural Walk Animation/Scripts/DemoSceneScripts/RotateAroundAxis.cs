using UnityEngine;

namespace LolopupkaAnimations2D
{
public class RotateAroundAxis : MonoBehaviour
{
    public Vector3 rotationVector;
    public float rotationSpeed;
    void Update()
    {
        transform.Rotate(rotationVector, rotationSpeed * Time.deltaTime);
    }
}
}
