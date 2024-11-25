using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using static MapConstants;

public class MapDrawer
{
    private Tilemap _tileMap;
    private TileConfig _tileConfig;

    [Inject]
    public void Initialize(Tilemap tileMap, TileConfig tileConfig)
    {
        _tileMap = tileMap;
        _tileConfig = tileConfig;
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
    public IEnumerator DrawMapRoutine(Map map)
    {
        _tileMap.ClearAllTiles();

        for (int i = 0; i < map.Chunks.Count; i++)
        {
            DrawChunkTiles(map.Chunks[i]);
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

    public class Factory : PlaceholderFactory<Tilemap, MapDrawer> { };
}
