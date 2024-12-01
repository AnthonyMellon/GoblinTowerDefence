using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Zenject;
using static MapConstants;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private Transform _structureContainer;
    [SerializeField] private Transform _enemyContainer;

    private Map.Factory _mapFactory;
    public Map _map { get; private set; }
    private MapDrawer.Factory _mapDrawerFactory;
    private MapDrawer _mapDrawer;
    private Coroutine _currentDrawRoutine;
    private Dictionary<Direction, int> _mapEdges;


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
        UpdateMapEdges();
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

    public Vector3 BindPositionToMap(Vector3 position)
    {
        if (_mapEdges == null) return position;

        if (_mapEdges.ContainsKey(Direction.North) && position.y > _mapEdges[Direction.North]) position.y = _mapEdges[Direction.North];
        if (_mapEdges.ContainsKey(Direction.South) && position.y < _mapEdges[Direction.South]) position.y = _mapEdges[Direction.South];
        if (_mapEdges.ContainsKey(Direction.West) && position.x < _mapEdges[Direction.West]) position.x = _mapEdges[Direction.West];
        if (_mapEdges.ContainsKey(Direction.East) && position.x > _mapEdges[Direction.East]) position.x = _mapEdges[Direction.East];

        return position;
    }

    public void UpdateMapEdges()
    {
        Dictionary<Direction, int> edges;
        edges = _map?.GetEdges();
        _mapEdges = edges;
    }
}
