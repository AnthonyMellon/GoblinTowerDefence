using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyStructure : StructureBase
{
    private List<Vector2Int> _path;
    private StructureBase _targetStructure;
    private Enemy.Factory _enemyFactory;
    private Transform _enemyContainer;

    [Inject]
    private void Initialize(Vector2Int position, Transform enemyContainer, Enemy.Factory enemyFactory)
    {
        _enemyFactory = enemyFactory;
        transform.position = new Vector3(position.x, position.y);
        _enemyContainer = enemyContainer;        
    }

    public void SetPath(List<Vector2Int> newPath, StructureBase nextStructure)
    {
        _path = newPath;
        _targetStructure = nextStructure;

        //Temp
        SpawnEnemy();
    }

    public override void OnReached(PathFollowerEntity entity)
    {
        entity.SetPath(_path, _targetStructure);
    }

    public void SpawnEnemy()
    {
        Enemy enemy = _enemyFactory.Create();
        enemy.transform.SetParent(_enemyContainer, true);
        enemy.SetPath(_path, _targetStructure);
    }

    public class Factory : PlaceholderFactory<Vector2Int, Transform, EnemyStructure> { };
}
