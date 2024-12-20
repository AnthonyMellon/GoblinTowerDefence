using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using static MapConstants;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private TowerBlueprint _currentTowerBlueprint;

    private InputProvider _inputProvider;
    private Tower.Factory _towerFactory;
    private MapManager _mapManager;
    private CursorManager _cursorManager;
    private PlayerManager _playerManager;

    private bool _enablePlacement;
    private List<Tower> _towers;

    private CursorManager.CursorAttachedObject _cursorAttachedObject;

    private List<TileType> _allowedTileTypes = new List<TileType> { TileType.Grass };

    [Inject]
    private void Initialize(
        InputProvider inputProvider,
        Tower.Factory towerFactory,
        MapManager mapManager,
        CursorManager cursorManager,
        PlayerManager playerManager
        )
    {
        _inputProvider = inputProvider;
        _towerFactory = towerFactory;
        _mapManager = mapManager;
        _cursorManager = cursorManager;
        _playerManager = playerManager;
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
            _cursorAttachedObject = new CursorManager.CursorAttachedObject(
                _currentTowerBlueprint.GetPlacementPreview(true),
                _currentTowerBlueprint.GetPlacementPreview(false),
                CanPlaceTower,
                CreateTower);
            _cursorManager.SetAttachedObject(_cursorAttachedObject);
        }
        else
        {
            _cursorManager.RemoveAttachedObject(_cursorAttachedObject);
        }
    }

    // Can the tower be placed here?
    private bool IsValidPlacementSpot(Vector2Int position)
    {
        TileData targetTile = _mapManager._map.GetTileAtPosition(position);
        if(
            targetTile != null
            && _currentTowerBlueprint.AllowedPlacements != null
            && _currentTowerBlueprint.AllowedPlacements.Contains(targetTile.TileType)
        )
        {            
            return true;
        }

        return false;
    }

    private bool CanPlaceTower(Vector2Int position)
    {
        if (_playerManager.Bank.CanAfford(_currentTowerBlueprint.Cost) == false) return false;
        if (IsValidPlacementSpot(position) == false) return false;

        return true;
    }
    private void CreateTower(Vector2Int location)
    {
        if (_mapManager?._map == null) return;

        if (_towers == null) _towers = new List<Tower>();

        TileData tile = _mapManager._map.GetTileAtPosition(location);
        if (!_allowedTileTypes.Contains(tile.TileType)) return;

        // Try buy the tower
        if (_playerManager.Bank.TryRemoveCurrency(_currentTowerBlueprint.Cost) == false) return;

        //Place tower
        Tower newTower = _towerFactory.Create(location, transform, _currentTowerBlueprint);
        _towers.Add(newTower);
        tile.SetType(TileType.Entity);

        EnableTowerPlacement(false);
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
