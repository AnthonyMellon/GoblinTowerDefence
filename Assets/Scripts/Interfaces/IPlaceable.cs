using UnityEngine;

public interface IPlaceable
{
    public bool IsValidPlacementSpot(Vector2Int position);
    public void Place(Vector2Int position);
}
