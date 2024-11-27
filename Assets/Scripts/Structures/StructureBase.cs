using System;
using UnityEngine;
using Zenject;

public abstract class StructureBase : MonoBehaviour
{
    public TileData OwnerTile { get; private set; }    

    public void SetOwnerTile(TileData ownerTile)
    {
        OwnerTile = ownerTile;
    }
    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    public abstract void OnReached(PathFollowerEntity entity);
}
