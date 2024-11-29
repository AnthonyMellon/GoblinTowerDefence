using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Zenject;

public class InputProvider
{
    private InputSystem_Actions _inputActions;

    [Inject]
    private void Initialize()
    {
        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Enable();
        
        _inputActions.Player.CameraZoom.performed += CameraZoom;        
        _inputActions.Player.CameraPan.performed += CameraPan;
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
}
