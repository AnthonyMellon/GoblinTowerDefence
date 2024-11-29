using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private MapManager _mapManager;
    private EnemyManager _enemyManager;
    private SpawnerManager _spawnerManager;

    [Inject]
    private void Initialize(MapManager mapManager, EnemyManager enemyManager, SpawnerManager spawnerManager)
    {
        _mapManager = mapManager;
        _enemyManager = enemyManager;
        _spawnerManager = spawnerManager;
    }

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        DestroyAllEnemies();
        _mapManager.GenerateMap();
        _mapManager.DrawMap();
    }

    public void DestroyAllEnemies() => _enemyManager.DestroyAllEnemies();
    public void SpawnNewWave() => _spawnerManager.SpawnWave();
}
