using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Zenject;

public class Map
{
    public List<Chunk> Chunks { get; private set; }
    private MapConfig _config;
    private int _seed;
    private Vector2Int _playerPosition = Vector2Int.zero;

    StructureGenerator _structureGenerator;
    ChunkGenerator _chunkGenerator;
    TerrainGenerator _terrainGenerator;
    PathGenerator _pathGenerator;

    public int GetChunkSize() => _chunkGenerator.GetChunkSize();

    [Inject]
    private void Initialize(
        MapConfig config,
        StructureGenerator structureGenerator,
        ChunkGenerator chunkGenerator,
        TerrainGenerator terrainGenerator,
        PathGenerator pathGenerator
        )
    {
        _config = config;
        _structureGenerator = structureGenerator;
        _chunkGenerator = chunkGenerator;
        _terrainGenerator = terrainGenerator;
        _pathGenerator = pathGenerator;

        _seed = _config.GetDefaultSeed();
    }


    public Map GenerateChunks()
    {
        Chunks?.Clear();
        Chunks = _chunkGenerator.GenerateChunks();
        return this;
    }

    public Map GenerateTerrain()
    {
        if (!_config.ShouldGenerateTerain()) return this;

        Chunks = _terrainGenerator.GenerateTerrain(Chunks, _config.GetPlayerSpawnPos(), _seed);
        return this;
    }

    public Map GenerateStructures()
    {
        if(!_config.ShouldGenerateStructures()) return this;

        (List<Chunk> chunks, Vector2Int playerPosition) = _structureGenerator.GenerateStructures(Chunks);
        Chunks = chunks;
        _playerPosition = playerPosition;
        return this;
    }

    public Map GeneratePaths()
    {        
        Stopwatch sw = Stopwatch.StartNew();

        List<TileData> tiles = new List<TileData>();
        for(int c = 0; c < Chunks.Count; c++)
        {
            Chunk chunk = Chunks[c];
            for(int x = 0; x < chunk.Tiles.Count; x++)
            {
                for(int y = 0; y < chunk.Tiles[x].Count; y++)
                {
                    tiles.Add(chunk.Tiles[x][y]);
                }
            }
        }
        if(!_config.ShouldGeneratePaths()) return this;

        Chunks = _pathGenerator.GeneratePaths(this, _playerPosition);

        UnityEngine.Debug.Log($"Time to generate: {sw.ElapsedMilliseconds}");

        return this;
    }

    public Chunk GetChunkNearPosition(Vector2Int worldPosition)
    {
        int chunkSize = _chunkGenerator.GetChunkSize();

        //Convert world position to grid position
        Vector2Int gridPosition = new Vector2Int(
            Mathf.FloorToInt(worldPosition.x / chunkSize),
            Mathf.FloorToInt(worldPosition.y / chunkSize)
            );

        //Find chunk with grid position
        return Chunks.Where(chunk => chunk.GridPosition == gridPosition).FirstOrDefault();
    }

    public class Factory : PlaceholderFactory<Map> { };
}
