using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System.Linq;

public class Tower : MonoBehaviour
{
    public Vector2Int location;
    private List<AttackableEntity> _targets;

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
        AttackableEntity target = _targets?.FirstOrDefault();
        _towerBlueprint.TryAttack(target);
    }

    public void AddTarget(AttackableEntity target)
    {
        if(_targets == null) _targets = new List<AttackableEntity>();

        _targets.Add(target);
    }

    public void RemoveTarget(AttackableEntity target)
    {
        if (_targets == null) return;

        _targets.Remove(target);
    }

    public class Factory : PlaceholderFactory<Vector2Int, Transform, TowerBlueprint, Tower> { }
}
