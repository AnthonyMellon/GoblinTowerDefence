using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private Transform _structureContainer;
    [SerializeField] private Transform _enemyContainer;

    private Map.Factory _mapFactory;
    private Map _map;
    private MapDrawer.Factory _mapDrawerFactory;
    private MapDrawer _mapDrawer;
    private Coroutine _currentDrawRoutine;

    public Action OnMapGenerateStart;
    public Action OnMapGenerateComplete;

    [Inject]
    private void Initialize(Map.Factory mapFactory, MapDrawer.Factory mapDrawerFactory)
    {
        _mapFactory = mapFactory;
        _mapDrawerFactory = mapDrawerFactory;
    }

    /// <summary>
    /// Generate all chunks in the map
    /// </summary>
    public void GenerateMap()
    {
        //If there is an old map we should unload it to make sure all entities are cleared
        _map?.Unload();

        Map map = _mapFactory
            .Create()
            .GenerateChunks()
            .GenerateTerrain()
            .GenerateStructures(_structureContainer, _enemyContainer)
            .GeneratePaths();

        _map = map;
    }
    
    /// <summary>
    /// Draw the map the the screen
    /// </summary>
    public void DrawMap()
    {
        if (_mapDrawer == null) _mapDrawer = _mapDrawerFactory.Create(_tileMap);

        if (_currentDrawRoutine != null) StopCoroutine(_currentDrawRoutine);
        _currentDrawRoutine = StartCoroutine(_mapDrawer.DrawMapRoutine(_map));

    }
}
