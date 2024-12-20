using UnityEngine;
using Zenject;

public class PlayerStructure : StructureBase
{
    [Inject]
    private void Initialize(Vector2Int position)
    {
        transform.position = new Vector3(position.x, position.y);
    }

    public override void OnReached(PathFollowerEntity entity)
    {
        entity.OnPathEnded?.Invoke();
    }

    public class Factory : PlaceholderFactory<Vector2Int, PlayerStructure> { };
}
