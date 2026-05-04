using UnityEngine;
using UnityEngine.U2D;

namespace LolopupkaAnimations2D
{
public class SpriteShapePointFollow : MonoBehaviour
{
    public Transform targetFollowTransform;
    public int pointIndex;
    private SpriteShapeController spriteShapeController;
    private bool isFollowing = true;
    Spline spline;

    private void Awake() {
        spriteShapeController = GetComponent<SpriteShapeController>();
        spline = spriteShapeController.spline;
    }

    private void Update() {
        
            spriteShapeController.BakeMesh();
    }

    private void LateUpdate() 
    {
         if(isFollowing)
         {
            Vector3 targetPosition = spriteShapeController.transform.InverseTransformPoint(targetFollowTransform.position);
            spline.SetPosition(pointIndex, targetPosition); 
            spriteShapeController.BakeMesh();
            spriteShapeController.RefreshSpriteShape();
         }
    }
}
}
