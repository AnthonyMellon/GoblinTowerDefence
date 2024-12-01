using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using static MapConstants;

public class TowerManager : MonoBehaviour
{
    private InputProvider _inputProvider;
    private Tower.Factory _towerFactory;
    private MapManager _mapManager;

    private List<Tower> _towers;

    private List<TileType> _allowedTileTypes = new List<TileType> { TileType.Grass };

    [Inject]
    private void Initialize(InputProvider inputProvider, Tower.Factory towerFactory, MapManager mapManager)
    {
        _inputProvider = inputProvider;
        _towerFactory = towerFactory;
        _mapManager = mapManager;
    }

    private void OnEnable()
    {
        _inputProvider.OnInteract += CreateTower;
    }

    private void OnDisable()
    {
        _inputProvider.OnInteract -= CreateTower;
    }

    private void CreateTower(Vector2Int location)
    {
        if (_mapManager?._map == null) return;

        if (_towers == null) _towers = new List<Tower>();        

        TileData tile = _mapManager._map.GetTileAtPosition(location);
        if (!_allowedTileTypes.Contains(tile.TileType)) return;

        //Place tower
        Tower newTower = _towerFactory.Create(location, transform);
        _towers.Add(newTower);
        tile.SetType(TileType.Entity);

    }

    public void DestroyAllTowers()
    {
        if (_towers == null) return;

        for (int i = _towers.Count - 1; i >= 0; i--)
        {
            Tower tower = _towers[i];
            _towers.RemoveAt(i);
            Destroy(tower.gameObject);
        }
    }
}
