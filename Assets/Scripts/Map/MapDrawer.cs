using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using static MapConstants;

public class MapDrawer
{
    private Tilemap _tileMap;
    private TileConfig _tileConfig;
    private PlayerStructure.Factory _playerStructureFactory;
    private EnemyStructure.Factory _enemyStructureFactory;

    [Inject]
    public void Initialize(Tilemap tileMap, TileConfig tileConfig, PlayerStructure.Factory playerStructureFactory, EnemyStructure.Factory enemyStructureFactory)
    {
        _tileMap = tileMap;
        _tileConfig = tileConfig;
        _playerStructureFactory = playerStructureFactory;
        _enemyStructureFactory = enemyStructureFactory;
    }

    /// <summary>
    /// Draws the map all at once, likely to freeze the game for a short time
    /// </summary>
    /// <param name="map">The map to be drawn</param>
    public void DrawMapStatic(Map map)
    {
        _tileMap.ClearAllTiles();

        for (int i = 0; i < map.Chunks.Count; i++)
        {
            DrawChunkTiles(map.Chunks[i]);
        }
    }

    /// <summary>
    /// Coroutine to draw the map, shouldn't freeze the game while it runs
    /// </summary>
    /// <param name="map">The map to be drawn</param>
    /// <returns></returns>
    public IEnumerator DrawMapRoutine(Map map, StructureContainer structureContainer)
    {
        _tileMap.ClearAllTiles();
        structureContainer.DestroyAllStructures();

        for (int i = 0; i < map.Chunks.Count; i++)
        {
            DrawChunkTiles(map.Chunks[i]);
            DrawChunkStructures(map.Chunks[i], structureContainer);
            yield return null;
        }
    }

    /// <summary>
    /// Draw a given chunk to the tile map
    /// </summary>
    /// <param name="chunk">The chunk to be drawn</param>
    public void DrawChunkTiles(Chunk chunk)
    {
        for (int x = 0; x < chunk.Tiles.Count; x++)
        {
            for (int y = 0; y < chunk.Tiles[x].Count; y++)
            {                
                TileData tileData = chunk.Tiles[x][y];

                TileBase tile = chunk.Revealed ? _tileConfig.GetTile(tileData.TileType) : _tileConfig.GetTile(TileType.Fog);
                if (tileData.HasStructure) tile = _tileConfig.GetTile(TileType.Path);

                Vector3Int tileMapPos = new Vector3Int(tileData.WorldPosition.x, tileData.WorldPosition.y);
                _tileMap.SetTile(tileMapPos, tile);
            }
        }
    }

    public void DrawChunkStructures(Chunk chunk, StructureContainer container)
    {
        for(int i = 0; i < chunk.Structures.Count; i++)
        {
            StructureBase createdStructure = null;

            switch (chunk.Structures[i].type)
            {
                case StructureType.Player:
                    createdStructure = _playerStructureFactory.Create(chunk.Structures[i].position);
                    break;
                case StructureType.Enemy:
                    createdStructure = _enemyStructureFactory.Create(chunk.Structures[i].position);
                    break;
                default:
                    Debug.LogWarning($"Attempting to draw invalid structure of type {chunk.Structures[i].type}");
                    break;
            }

            if(createdStructure != null)
            {
                container.AddStructure(createdStructure);
            }
        }
    }

    public class Factory : PlaceholderFactory<Tilemap, MapDrawer> { };
}
