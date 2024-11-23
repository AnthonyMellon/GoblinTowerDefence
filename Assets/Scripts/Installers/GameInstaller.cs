using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private MapManager _mapManager;

    [Header("Configs")]
    [SerializeField] private MapConfig _mapConfig;
    [SerializeField] private ChunkConfig _chunkConfig;
    [SerializeField] private TerrainConfig _terrainConfig;
    [SerializeField] private StructureConfig _structureConfig;
    [SerializeField] private TileConfig _tileConfig;


    [Header("Structures")]
    [SerializeField] private PlayerStructure _playerStructure;
    [SerializeField] private EnemyStructure _enemyStructure;

    public override void InstallBindings()
    {
        Container.Bind<MapManager>().FromComponentInNewPrefab(_mapManager).AsSingle();

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
    }
}