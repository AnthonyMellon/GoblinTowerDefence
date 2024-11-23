using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using static MapConstants;

[CreateAssetMenu(fileName = "NewTileConfig", menuName = "TowerDefenceGame/Configs/TileConfig")]
public class TileConfig : ScriptableObject
{
    [SerializeField] private TileBase _tileCoast;
    [SerializeField] private TileBase _tileHill;
    [SerializeField] private TileBase _tileGrass;
    [SerializeField] private TileBase _tilePath;
    [SerializeField] private TileBase _tileFog;
    [SerializeField] private TileBase _tileInvalid;

    public TileBase GetTile(TileType tile)
    {
        switch(tile)
        {
            case TileType.Grass:
                return _tileGrass;
            case TileType.Hill:
                return _tileHill;
            case TileType.Coast:
                return _tileCoast;
            case TileType.Path:
                return _tilePath;
            case TileType.Fog:
                return _tileFog;
            case TileType.Invalid:
            default:
                return _tileInvalid;

        }
    }
}
