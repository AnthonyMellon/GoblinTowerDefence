using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputProvider
{
    private InputSystem_Actions _inputActions;
    private Camera _playerCamera;

    [Inject]
    private void Initialize(Camera playerCamera)
    {
        _playerCamera = playerCamera;

        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Enable();
        
        _inputActions.Player.CameraZoom.performed += CameraZoom;        
        _inputActions.Player.CameraPan.performed += CameraPan;
        _inputActions.Player.Interact.performed += Interact;
    }

    public Action<float> OnCameraZoom;
    private void CameraZoom(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        float verticalScrollValue = value.y;
        OnCameraZoom?.Invoke(verticalScrollValue);
    }


    public Action<Vector2> OnCameraPan;
    private void CameraPan(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        OnCameraPan?.Invoke(value);
    }

    public Action<Vector2> OnInteract;
    private void Interact(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        Vector2Int worldPoint = Vector2Int.RoundToInt(_playerCamera.ScreenToWorldPoint(value));
        OnInteract?.Invoke(worldPoint);
    }
}
