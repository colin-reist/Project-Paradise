using UnityEngine;

namespace LolopupkaAnimations2D
{

public class TargetPoint
{

    public static Vector3 FitToTheGround(Vector3 origin, Transform body, LayerMask layerMask, float yOffset, float rayLength, float sphereCastRadius, proceduralAnimation.CharacterType characterType)
    {
        if(characterType == proceduralAnimation.CharacterType.platformer)
        {
            if(Physics2D.Raycast(origin + body.up * yOffset, -body.up, rayLength, layerMask))
            {
                return Physics2D.Raycast(origin + body.up * yOffset, -body.up, rayLength, layerMask).point;
            }
            else if(Physics2D.CircleCast(origin + body.up * yOffset, sphereCastRadius, -body.up, rayLength, layerMask))
            {
                return Physics2D.CircleCast(origin + body.up * yOffset, sphereCastRadius, -body.up, rayLength, layerMask).point;
            } 
            else
            {
                return origin;  
            } 
        }
        else
        {
            if(Physics2D.OverlapCircle(origin, sphereCastRadius, layerMask))
            {
                Vector3 targetPoint = Physics2D.OverlapCircle(origin, sphereCastRadius, layerMask).ClosestPoint(origin);
                return targetPoint;
            }
            else
            {
                return origin;  
            }
        }
    }

    public static bool IsPointInsideCollider(Vector3 point, LayerMask layerMask)
    {
        if (Physics2D.OverlapCircle(point, 0f, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}

}
