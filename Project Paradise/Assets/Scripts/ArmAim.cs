using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(200)]
public class ArmAim : MonoBehaviour
{
    [Header("Target des bras")] 
    [SerializeField] private Transform _armTargetF;
    [SerializeField] private Transform _armTargetB;
    private PlayerInput _playerInput;
    private InputAction _aimAction;
    

    private Camera _cam;
    

    private void Awake()
    {
        _cam = Camera.main;
        _playerInput = GetComponent<PlayerInput>();
        _aimAction = _playerInput.actions["Aim"];
    }

    private void LateUpdate()
    {
        _armTargetB.position = _cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Update()
    {
        if (_aimAction.IsPressed())
        {
            // distance maximale entre l'épaule et la main pour éviter de tendre le bras au max comme cassé
            
            //
            print("Vise");
        }
    }
}