using UnityEngine;

public static class MapConstants
{
    public enum TileType
    {
        Grass,
        Fog,
        Path,
        Coast,
        Hill,
        Invalid
    }

    public enum ChunkOwner
    {
        None,
        Player,
        Enemy
    }

    public enum StructureType
    {
        Player,
        Enemy
    }
}
