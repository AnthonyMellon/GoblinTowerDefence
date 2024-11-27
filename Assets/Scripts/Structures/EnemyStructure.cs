using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyStructure : StructureBase
{
    private List<Vector2Int> _path;
    private StructureBase _targetStructure;
    private Enemy.Factory _enemyFactory;
    private EnemyManager _enemyManager;

    [Inject]
    private void Initialize(Vector2Int position, Enemy.Factory enemyFactory, EnemyManager enemyManager)
    {
        _enemyFactory = enemyFactory;
        transform.position = new Vector3(position.x, position.y);
        _enemyManager = enemyManager;
    }

    public void SetPath(List<Vector2Int> newPath, StructureBase nextStructure)
    {
        _path = newPath;
        _targetStructure = nextStructure;
    }

    public override void OnReached(PathFollowerEntity entity)
    {
        entity.SetPath(_path, _targetStructure);
    }

    public void SpawnEnemy()
    {
        // Don't spawn an enemy if I don't have a path to give it
        if(_path == null) return;

        Enemy enemy = _enemyFactory.Create();
        enemy.SetPath(_path, _targetStructure);
        _enemyManager.AddEnemy(enemy);
    }

    public class Factory : PlaceholderFactory<Vector2Int, EnemyStructure> { };
}
