using UnityEngine;

namespace LolopupkaAnimations2D
{

public class follow : MonoBehaviour
{
    public Transform target;
    public float offset_X;
    public float offset_Y;
    public float offset_Z;

    public enum UpdateMethod {Update, FixedUpdate, LateUpdate}
    public UpdateMethod updateMethod;

    public enum followposition {None, follow_X, follow_y, follow_z, follow_xz, follow_All}
    public followposition followTargetPosition;
    [SerializeField] private bool interpolatePosition;
    [SerializeField] private float positionInterpolationSpeed = 5f;
    
    public enum CopyRotation {None, rotation_X, rotation_y, rotation_z, rotation_All }
    public CopyRotation mimicRotation;
    public float rotationOffset;
    private Quaternion lastRotation;
    [SerializeField] private bool interpolateRotation;
    [SerializeField] private float rotationInterpolationSpeed = 5f;
    void Update()
    {

        if (updateMethod != UpdateMethod.Update) return;

        switch (followTargetPosition)
        {
            case followposition.follow_X:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x + offset_X, transform.position.y, transform.position.z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = new Vector3(target.position.x + offset_X, transform.position.y, transform.position.z);
            }
            break;

            case followposition.follow_y:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, target.position.y + offset_Y, transform.position.z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, target.position.y + offset_Y, transform.position.z);
            }
            break;

            case followposition.follow_z:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, target.position.z + offset_Z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z + offset_Z);
            }
            break;
            
            case followposition.follow_xz:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position = new Vector3(target.position.x + offset_X, transform.position.y, target.position.z + offset_Z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = new Vector3(target.position.x + offset_X, transform.position.y, target.position.z + offset_Z);
            }
            break;

            case followposition.follow_All:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, target.position + new Vector3(offset_X, offset_Y, offset_Z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = target.position + new Vector3(offset_X, offset_Y, offset_Z);
            }
            break;

        }

        switch (mimicRotation)
        {
            case CopyRotation.rotation_X:
                if(interpolateRotation)
                {
                    transform.rotation = Quaternion.Slerp(lastRotation, Quaternion.Euler(target.rotation.eulerAngles.x + rotationOffset, transform.rotation.y, transform.rotation.z), Time.deltaTime * rotationInterpolationSpeed);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(target.rotation.eulerAngles.x + rotationOffset, transform.rotation.y, transform.rotation.z);
                }
                break;

            case CopyRotation.rotation_y:
                if(interpolateRotation)
                {
                    transform.rotation = Quaternion.Slerp(lastRotation, Quaternion.Euler(transform.rotation.x, target.rotation.eulerAngles.y + rotationOffset, transform.rotation.z), Time.deltaTime * rotationInterpolationSpeed);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.x, target.rotation.eulerAngles.y + rotationOffset, transform.rotation.z);
                }
                break;

            case CopyRotation.rotation_z:
                if(interpolateRotation)
                {
                    transform.rotation = Quaternion.Slerp(lastRotation, Quaternion.Euler(transform.rotation.x, transform.rotation.z, target.rotation.eulerAngles.z + rotationOffset), Time.deltaTime * rotationInterpolationSpeed);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.z, target.rotation.eulerAngles.z + rotationOffset);
                }
                break;

            case CopyRotation.rotation_All:
                if(interpolateRotation)
                {
                    transform.rotation = Quaternion.Slerp(lastRotation, Quaternion.Euler(target.rotation.eulerAngles), Time.deltaTime * rotationInterpolationSpeed);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(target.rotation.eulerAngles);
                }
                break;
        }

        lastRotation = transform.rotation;
    }

    private void FixedUpdate() 
    {
        if (updateMethod != UpdateMethod.FixedUpdate) return;

        switch (followTargetPosition)
        {
            case followposition.follow_X:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x + offset_X, transform.position.y, transform.position.z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = new Vector3(target.position.x + offset_X, transform.position.y, transform.position.z);
            }
            break;

            case followposition.follow_y:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, target.position.y + offset_Y, transform.position.z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, target.position.y + offset_Y, transform.position.z);
            }
            break;

            case followposition.follow_z:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, target.position.z + offset_Z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z + offset_Z);
            }
            break;
            
            case followposition.follow_xz:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position = new Vector3(target.position.x + offset_X, transform.position.y, target.position.z + offset_Z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = new Vector3(target.position.x + offset_X, transform.position.y, target.position.z + offset_Z);
            }
            break;

            case followposition.follow_All:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, target.position + new Vector3(offset_X, offset_Y, offset_Z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = target.position + new Vector3(offset_X, offset_Y, offset_Z);
            }
            break;

        }

        switch (mimicRotation)
        {
            case CopyRotation.rotation_X:
                transform.rotation = Quaternion.Euler(target.rotation.eulerAngles.x, transform.rotation.y, transform.rotation.z);
                break;

            case CopyRotation.rotation_y:
                transform.rotation = Quaternion.Euler(transform.rotation.x, target.rotation.eulerAngles.y, transform.rotation.z);
                break;

            case CopyRotation.rotation_z:
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.z, target.rotation.eulerAngles.z);
                break;

            case CopyRotation.rotation_All:
                transform.rotation = Quaternion.Euler(target.rotation.eulerAngles);
                break;
        }
    }

    private void LateUpdate()
    {
        if (updateMethod != UpdateMethod.LateUpdate) return;

        switch (followTargetPosition)
        {
            case followposition.follow_X:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x + offset_X, transform.position.y, transform.position.z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = new Vector3(target.position.x + offset_X, transform.position.y, transform.position.z);
            }
            break;

            case followposition.follow_y:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, target.position.y + offset_Y, transform.position.z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, target.position.y + offset_Y, transform.position.z);
            }
            break;

            case followposition.follow_z:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, target.position.z + offset_Z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z + offset_Z);
            }
            break;
            
            case followposition.follow_xz:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position = new Vector3(target.position.x + offset_X, transform.position.y, target.position.z + offset_Z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = new Vector3(target.position.x + offset_X, transform.position.y, target.position.z + offset_Z);
            }
            break;

            case followposition.follow_All:
            if(interpolatePosition)
            {
                transform.position = Vector3.Lerp(transform.position, target.position + new Vector3(offset_X, offset_Y, offset_Z), Time.deltaTime * positionInterpolationSpeed);
            }
            else
            {
                transform.position = target.position + new Vector3(offset_X, offset_Y, offset_Z);
            }
            break;

        }

        switch (mimicRotation)
        {
            case CopyRotation.rotation_X:
                transform.rotation = Quaternion.Euler(target.rotation.eulerAngles.x, transform.rotation.y, transform.rotation.z);
                break;

            case CopyRotation.rotation_y:
                transform.rotation = Quaternion.Euler(transform.rotation.x, target.rotation.eulerAngles.y, transform.rotation.z);
                break;

            case CopyRotation.rotation_z:
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.z, target.rotation.eulerAngles.z);
                break;

            case CopyRotation.rotation_All:
                transform.rotation = Quaternion.Euler(target.rotation.eulerAngles);
                break;
        }
    }
}
}
