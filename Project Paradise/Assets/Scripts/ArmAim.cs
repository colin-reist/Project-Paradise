using UnityEngine;
using UnityEngine.InputSystem;

public class ArmAim : MonoBehaviour
{
    [SerializeField] private Transform ikTargetLeft;
    [SerializeField] private Transform ikTargetRight;
    
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorld = _cam.ScreenToWorldPoint(screenPos);
            mouseWorld.z = 0f;

            ikTargetLeft.position = mouseWorld;
            ikTargetRight.position = mouseWorld;
        }
    }
}