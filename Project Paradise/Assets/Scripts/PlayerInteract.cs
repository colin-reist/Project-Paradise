using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInteract : MonoBehaviour
{
    private PlayerInput _playerInput;
    private InputAction _interactAction;
    private GameObject _gameObject;
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
            _currentInteractible = interactible;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactible = other.GetComponent<Iinteractible>();
        if (interactible != null)
            _currentInteractible = null;
    }

    void Update()
    {
        if (_interactAction.triggered && _currentInteractible != null)
        {
            _currentInteractible.Interact();
        }
    }
}
