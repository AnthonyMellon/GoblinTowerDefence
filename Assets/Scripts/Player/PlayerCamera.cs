using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static MapConstants;

[RequireComponent (typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    private Camera _camera;
    private InputProvider _inputProvider;
    private MapManager _mapManager;

    [SerializeField] private float _minCameraZoom;
    [SerializeField] private float _maxCameraZoom;
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _panSpeed;
    private float _currentZoomLevel = 0;

    [Inject]
    private  void Initialize(InputProvider inputProvider, MapManager mapManager)
    {
        _inputProvider = inputProvider;
        _mapManager = mapManager;
    }

    private void OnEnable()
    {
        if(_camera == null) _camera = GetComponent<Camera>();

        UpdateZoomLevel();

        _inputProvider.OnCameraZoom += ZoomCamera;
        _inputProvider.OnCameraPan += PanCamera;
    }

    private void OnDisable()
    {
        _inputProvider.OnCameraZoom -= ZoomCamera;
        _inputProvider.OnCameraPan -= PanCamera;
    }

    private void ZoomCamera(float direction)
    {
        float currentZoom = _camera.orthographicSize;
        float newZoom = currentZoom + (direction * _zoomSpeed);
        newZoom = Mathf.Clamp(newZoom, _minCameraZoom, _maxCameraZoom);
        _camera.orthographicSize = newZoom;

        UpdateZoomLevel();
    }

    private void UpdateZoomLevel()
    {
        _currentZoomLevel = (_camera.orthographicSize - _minCameraZoom) / (_maxCameraZoom - _minCameraZoom);
    }

    private void PanCamera(Vector2 direction)
    {
        Vector3 currentPosition = transform.position;
        Vector2 panDistance = direction * _panSpeed * (_currentZoomLevel + 0.5f);
        Vector3 newPosition = new Vector3(
            currentPosition.x + panDistance.x,
            currentPosition.y + panDistance.y,
            currentPosition.z
            );

        newPosition = _mapManager.BindPositionToMap(newPosition);
        transform.position = newPosition;
    }
}
