using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private MapManager _mapManager;
    private EnemyManager _enemyManager;
    private SpawnerManager _spawnerManager;
    private TowerManager _towerManager;

    [Inject]
    private void Initialize(
        MapManager mapManager,
        EnemyManager enemyManager,
        SpawnerManager spawnerManager,
        TowerManager towerManager
        )
    {
        _mapManager = mapManager;
        _enemyManager = enemyManager;
        _spawnerManager = spawnerManager;
        _towerManager = towerManager;
    }

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        DestroyAllEnemies();
        DestroyAllTowers();
        _mapManager.GenerateMap();
        _mapManager.DrawMap();
    }

    public void DestroyAllEnemies() => _enemyManager.DestroyAllEnemies();
    public void DestroyAllTowers() => _towerManager.DestroyAllTowers();
    public void SpawnNewWave() => _spawnerManager.SpawnWave();
}
