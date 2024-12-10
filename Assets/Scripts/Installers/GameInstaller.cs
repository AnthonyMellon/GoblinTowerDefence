using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private Camera _playerCamera;

    [Header("Managers")]
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private SpawnerManager _spawnerManager;
    [SerializeField] private TowerManager _towerManager;
    [SerializeField] private CursorManager _cursorManager;

    [Header("Configs")]
    [SerializeField] private MapConfig _mapConfig;
    [SerializeField] private ChunkConfig _chunkConfig;
    [SerializeField] private TerrainConfig _terrainConfig;
    [SerializeField] private StructureConfig _structureConfig;
    [SerializeField] private TileConfig _tileConfig;


    [Header("Structures")]
    [SerializeField] private PlayerStructure _playerStructure;
    [SerializeField] private EnemyStructure _enemyStructure;

    [Header("Entities")]
    [SerializeField] private Enemy _enemy;
    [SerializeField] private Tower _tower;

    public override void InstallBindings()
    {
        Container.Bind<InputProvider>().FromNew().AsSingle();
        Container.Bind<Camera>().FromComponentInHierarchy(_playerCamera).AsSingle();

        //Managers
        Container.Bind<MapManager>().FromComponentInNewPrefab(_mapManager).AsSingle();
        Container.Bind<EnemyManager>().FromComponentInNewPrefab(_enemyManager).AsSingle();
        Container.Bind<SpawnerManager>().FromComponentInNewPrefab(_spawnerManager).AsSingle();
        Container.Bind<TowerManager>().FromComponentInNewPrefab(_towerManager).AsSingle().NonLazy();
        Container.Bind<CursorManager>().FromComponentInNewPrefab(_cursorManager).AsSingle();

        //Configs
        Container.Bind<MapConfig>().FromScriptableObject(_mapConfig).AsSingle();
        Container.Bind<ChunkConfig>().FromScriptableObject(_chunkConfig).AsSingle();
        Container.Bind<TerrainConfig>().FromScriptableObject(_terrainConfig).AsSingle();
        Container.Bind<StructureConfig>().FromScriptableObject(_structureConfig).AsSingle();
        Container.Bind<TileConfig>().FromScriptableObject(_tileConfig).AsSingle();

        //Generators
        Container.Bind<StructureGenerator>().FromNew().AsSingle();
        Container.Bind<ChunkGenerator>().FromNew().AsSingle();
        Container.Bind<TerrainGenerator>().FromNew().AsSingle();
        Container.Bind<PathGenerator>().FromNew().AsSingle();

        //Factories
        Container.BindFactory<Map, Map.Factory>();
        Container.BindFactory<Tilemap, MapDrawer, MapDrawer.Factory>();
        Container.BindFactory<Vector2Int, PlayerStructure, PlayerStructure.Factory>().FromComponentInNewPrefab(_playerStructure).AsSingle();
        Container.BindFactory<Vector2Int, EnemyStructure, EnemyStructure.Factory>().FromComponentInNewPrefab(_enemyStructure).AsSingle();
        Container.BindFactory<float, Enemy, Enemy.Factory>().FromComponentInNewPrefab(_enemy).AsSingle();
        Container.BindFactory<Vector2Int, Transform, Tower, Tower.Factory>().FromComponentInNewPrefab(_tower).AsSingle();
    }
}