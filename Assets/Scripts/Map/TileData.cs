using UnityEngine;
using UnityEngine.Rendering;
using static MapConstants;

public class TileData
{
    public float Height { get; private set; }
    public TileType TileType { get; private set; }
    public Vector2Int WorldPosition {get; private set; }
    public bool HasStructure { get; private set; } = false;

    public TileData(float height, TileType tileType, Vector2Int worldPosition)
    {
        Height = height;
        TileType = tileType;
        WorldPosition = worldPosition;
    }

    public void SetHeight(float height)
    {
        Height = Mathf.Clamp01(height);
    }

    public void SetType(TileType tileType)
    {
        TileType = tileType;
    }

    public void SetHasStructure(bool toggle)
    {
        HasStructure = toggle;
    }
}
