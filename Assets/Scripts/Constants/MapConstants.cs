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
        Entity,
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

    public enum Direction
    {
        North,
        East,
        South,
        West
    }

}
