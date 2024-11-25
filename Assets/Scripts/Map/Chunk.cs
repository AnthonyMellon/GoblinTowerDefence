using ModestTree;
using System.Collections.Generic;
using UnityEngine;
using static MapConstants;

public class Chunk
{
    public List<List<TileData>> Tiles { get; private set; }
    public Vector2Int WorldPosition { get; private set; }
    public ChunkOwner Owner { get; private set; }
    public Vector2Int GridPosition => WorldPosition / Size;
    public int Size { get; private set; }
    public bool Revealed;
    //public List<(Vector2Int position, StructureType type)> Structures { get; private set; } = new List<(Vector2Int position, StructureType type)>();
    public List<StructureBase> Structures { get; private set; } = new List<StructureBase>();

    private List<TileData> _grassTiles;

    public Chunk(List<List<TileData>> tiles, Vector2Int worldPosition, int size, ChunkOwner owner, bool revealed = false)
    {
        Tiles = tiles;
        WorldPosition = worldPosition;
        Size = size;
        Owner = owner;
        Revealed = revealed;
    }

    public void AddStrucuture(StructureBase structure)
    {
        Structures.Add(structure);
    }

    public TileData GetCenterTile()
    {
        return Tiles[0 + Size / 2][0 + Size / 2];
    }

    public TileData GetRandomGrassTile()
    {
        if (_grassTiles == null) GetAllGrassTiles();
        if(_grassTiles.IsEmpty()) return null;

        int index = Random.Range(0, _grassTiles.Count);
        return _grassTiles[index];
    }

    private void GetAllGrassTiles()
    {
        List<TileData> foundGrassTiles = new List<TileData>();

        for(int x = 0; x < Tiles.Count; x++)
        {
            for(int y = 0; y < Tiles[x].Count; y++)
            {
                TileData tile = Tiles[x][y];
                if(tile.TileType == TileType.Grass) foundGrassTiles.Add(tile);
            }
        }

        _grassTiles = foundGrassTiles;
    }
}
