using UnityEngine;


namespace LolopupkaAnimations2D
{
public class TeleportToPoint : MonoBehaviour
{
    public Transform transformToTeleport;
    public Transform teleportPosition;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            transformToTeleport.position = teleportPosition.position;
        }
    }
}
}
