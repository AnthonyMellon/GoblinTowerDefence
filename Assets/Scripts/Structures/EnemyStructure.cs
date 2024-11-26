using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyStructure : StructureBase
{
    private List<Vector2Int> _path;
    private StructureBase _targetStructure;

    [Inject]
    private void Initialize(Vector2Int position)
    {
        transform.position = new Vector3(position.x, position.y);
    }

    public void SetPath(List<Vector2Int> newPath, StructureBase nextStructure)
    {
        _path = newPath;
        _targetStructure = nextStructure;
    }

    public class Factory : PlaceholderFactory<Vector2Int, EnemyStructure> { };
}
