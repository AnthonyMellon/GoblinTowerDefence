using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using static MapConstants;

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

    public void Unload()
    {
        ClearAllCurrentChunks();
    }

    public Map GenerateChunks()
    {
        Chunks = _chunkGenerator.GenerateChunks();
        return this;
    }

    public Map GenerateTerrain()
    {
        if (!_config.ShouldGenerateTerain()) return this;

        Chunks = _terrainGenerator.GenerateTerrain(Chunks, _config.GetPlayerSpawnPos(), _seed);
        return this;
    }

    public Map GenerateStructures(Transform structureContainer, Transform enemyContianer)
    {
        if(!_config.ShouldGenerateStructures()) return this;

        (List<Chunk> chunks, Vector2Int playerPosition) = _structureGenerator.GenerateStructures(Chunks, structureContainer, enemyContianer);
        Chunks = chunks;
        _playerPosition = playerPosition;
        return this;
    }

    public Map GeneratePaths()
    {
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

        Chunks = _pathGenerator.GeneratePaths(this);

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

    /// <summary>
    /// Gets positions of all player structures (should only ever be one in normal gameplay)
    /// </summary>
    /// <returns>List of player structure positions</returns>
    public List<StructureBase> GetAllPlayerStructures()
    {
        List<StructureBase> structures = new List<StructureBase>();

        for (int c = 0; c < Chunks.Count; c++)
        {
            Chunk chunk = Chunks[c];
            for (int s = 0; s < chunk.Structures.Count; s++)
            {
                StructureBase structure = chunk.Structures[s];
                if(structure is PlayerStructure)
                {
                    structures.Add(structure);
                }
            }
        }

        return structures;
    }

    private void ClearAllCurrentChunks()
    {
        //No need to clear chunks that don't exist
        if (Chunks == null) return;
        
        for(int i = 0; i < Chunks.Count; i++)
        {
            Chunks[i].Unload();
        }
        Chunks.Clear();
    }

    public Dictionary<Direction, int> GetEdges()
    {
        Dictionary<Direction, int> edges = new Dictionary<Direction, int>();
        
        for (int i = 0; i < Chunks.Count; i++)
        {
            Chunk chunk = Chunks[i];

            if(!edges.ContainsKey(Direction.North) || chunk.WorldPosition.y > edges[Direction.North]) edges[Direction.North] = chunk.GetEdge(Direction.North);
            if(!edges.ContainsKey(Direction.South) || chunk.WorldPosition.y < edges[Direction.South]) edges[Direction.South] = chunk.GetEdge(Direction.South);
            if(!edges.ContainsKey(Direction.West) || chunk.WorldPosition.x < edges[Direction.West]) edges[Direction.West] = chunk.GetEdge(Direction.West);
            if(!edges.ContainsKey(Direction.East) || chunk.WorldPosition.x > edges[Direction.East]) edges[Direction.East] = chunk.GetEdge(Direction.East);
        }
        return edges;
    }

    public class Factory : PlaceholderFactory<Map> { };
}
