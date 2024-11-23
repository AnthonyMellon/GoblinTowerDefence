using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static MapConstants;

public class TerrainGenerator
{
    private TerrainConfig _config;

    [Inject]
    private void Initialize(TerrainConfig config)
    {
        _config = config;
    }

    public List<Chunk> GenerateTerrain(List<Chunk> chunks, Vector2Int playerSpawnPos, int seed)
    {
        //For each chunk
        for(int i = 0; i < chunks.Count; i++)
        {
            Chunk chunk = chunks[i];

            //For each tile
            for (int x = 0; x < chunk.Tiles.Count; x++)
            {
                for(int y = 0; y < chunk.Tiles[x].Count; y++)
                {
                    TileData tileData = chunk.Tiles[x][y];                    
                    tileData.SetHeight(WorldPositionToHeight(tileData.WorldPosition, playerSpawnPos, seed));                    
                    tileData.SetType(TileHeightToTileType(tileData.Height));
                }
            }
        }

        return chunks;
    }

    private float WorldPositionToHeight(Vector2Int tileWorldPos, Vector2Int playerSpawnPos, int seed)
    {
        float tileValue = 0;

        float amplitude = _config.GetAmplitude();
        float frequency = _config.GetFrequency();

        //Convert position to tile value
        for (int i = 0; i < _config.GetOctaveCount(); i++)
        {
            tileValue += amplitude * Mathf.PerlinNoise((tileWorldPos.x + seed + i) * frequency, (tileWorldPos.y + seed + i) * frequency);
            amplitude *= _config.GetPersistance();
            frequency *= _config.GetLacunarity();
        }

        //Drag tile value towards grass
        float distanceFromSpawn = Vector2.Distance(tileWorldPos, playerSpawnPos);
        float grassLandPreference = 1 - Mathf.Clamp01(distanceFromSpawn / _config.GetSafeZoneSize());
        float distanceFromGrass = _config.GetGrassHeight() - tileValue;
        tileValue += distanceFromGrass * grassLandPreference;

        return tileValue;
    }

    private TileType TileHeightToTileType(float height)
    {
        if (height < _config.GetMaxCoastHeight()) return TileType.Coast;
        if(height > _config.GetMinHillHeight()) return TileType.Hill;

        return TileType.Grass;
    }
}
