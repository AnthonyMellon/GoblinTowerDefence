using UnityEngine;
using Zenject;

public class EnemyStructure : StructureBase
{
    [Inject]
    private void Initialize(Vector2Int position)
    {
        transform.position = new Vector3(position.x, position.y);
    }

    public class Factory : PlaceholderFactory<Vector2Int, EnemyStructure> { };
}
