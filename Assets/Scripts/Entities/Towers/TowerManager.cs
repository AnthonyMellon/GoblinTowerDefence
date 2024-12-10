using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using static MapConstants;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private Sprite _placementPreview;    

    private InputProvider _inputProvider;
    private Tower.Factory _towerFactory;
    private MapManager _mapManager;
    private CursorManager _cursorManager;

    private bool _enablePlacement;
    private List<Tower> _towers;

    private List<TileType> _allowedTileTypes = new List<TileType> { TileType.Grass };

    [Inject]
    private void Initialize(InputProvider inputProvider, Tower.Factory towerFactory, MapManager mapManager, CursorManager cursorManager)
    {
        _inputProvider = inputProvider;
        _towerFactory = towerFactory;
        _mapManager = mapManager;
        _cursorManager = cursorManager;
    }

    private void OnEnable()
    {
        _inputProvider.OnToggleTowerPlacement += ToggleTowerPlacement;
    }

    private void OnDisable()
    {
        _inputProvider.OnToggleTowerPlacement -= ToggleTowerPlacement;
    }

    private void ToggleTowerPlacement()
    {
        EnableTowerPlacement(!_enablePlacement);
    }

    private void EnableTowerPlacement(bool enable)
    {
        // Prevents double setting
        if(_enablePlacement == enable) return;

        _enablePlacement = enable;
        if (_enablePlacement)
        {
            _inputProvider.OnInteract += CreateTower;
            _cursorManager.SetAttachedSprite(_placementPreview);
        }
        else
        {
            _inputProvider.OnInteract -= CreateTower;
            _cursorManager.ClearAttachedSprite(); //TODO: This part shouldn't be in charge of this, what if something else has changed the attached sprite before now?
        }
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
