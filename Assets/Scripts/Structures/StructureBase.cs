using UnityEngine;
using Zenject;

public class StructureBase : MonoBehaviour
{
    public TileData OwnerTile { get; private set; }

    public void SetOwnerTile(TileData ownerTile)
    {
        OwnerTile = ownerTile;
    }
}
