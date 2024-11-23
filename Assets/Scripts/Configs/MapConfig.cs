using UnityEngine;

[CreateAssetMenu(fileName = "NewMapConfig", menuName = "TowerDefenceGame/Configs/MapConfig")]
public class MapConfig : ScriptableObject
{
    [Tooltip("0 for random")]
    [SerializeField] private int _defaultSeed;
    [SerializeField] private Vector2Int _playerSpawnPos;

    [Header("Generation Stuff")]
    [SerializeField] private bool _generateTerrain;
    [SerializeField] private bool _generateStructures;
    [SerializeField] private bool _generatePaths;

    public Vector2Int GetPlayerSpawnPos() => _playerSpawnPos;
    public bool ShouldGenerateTerain() => _generateTerrain;
    public bool ShouldGenerateStructures() => _generateStructures;
    public bool ShouldGeneratePaths() => _generatePaths;


    public int GetDefaultSeed()
    {
        int seed = _defaultSeed == 0 ?
            Random.Range(-1000000, 1000000) //Get a random seed
            : _defaultSeed; //Use the set seed

        return seed;
    }
}
 