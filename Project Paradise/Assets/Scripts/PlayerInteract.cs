using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInteract : MonoBehaviour
{
    private PlayerInput _playerInput;
    private InputAction _interactAction;
    private Iinteractible _currentInteractible;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _interactAction = _playerInput.actions["Interact"];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactible = other.GetComponent<Iinteractible>();
        if (interactible != null)
        {
            _currentInteractible?.RemoveTarget();
            _currentInteractible = interactible;
            _currentInteractible.BecomeTarget();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactible = other.GetComponent<Iinteractible>();
        if (interactible != null)
        {
            interactible.RemoveTarget();
            if (_currentInteractible == interactible)
            {
                _currentInteractible = null;
            }
        }
    }

    void Update()
    {
        if (_interactAction.triggered && _currentInteractible != null)
        {
            _currentInteractible.Interact();
        }
    }
}
