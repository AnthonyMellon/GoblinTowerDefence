using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System.Linq;

public class Tower : MonoBehaviour
{
    public Vector2Int location;
    private List<IAttackable> _targets;

    private TowerBlueprint _towerBlueprint;

    [Inject]
    private void Initialize(Vector2Int worldPosition, Transform parent, TowerBlueprint towerBlueprint)
    {
        transform.parent = parent;
        transform.localPosition = new Vector3(worldPosition.x, worldPosition.y, 0);
        _towerBlueprint = towerBlueprint;
    }

    private void Update()
    {
        IAttackable target = _targets?.FirstOrDefault();
        _towerBlueprint.Attack(target);
    }

    public void AddTarget(IAttackable target)
    {
        if(_targets == null) _targets = new List<IAttackable>();

        _targets.Add(target);
    }

    public void RemoveTarget(IAttackable target)
    {
        if (_targets == null) return;

        _targets.Remove(target);
    }

    public class Factory : PlaceholderFactory<Vector2Int, Transform, TowerBlueprint, Tower> { }
}
