using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System.Linq;

public class Tower : MonoBehaviour
{
    public Vector2Int location;
    private List<IAttackable> _targets;

    [Inject]
    private void Initialize(Vector2Int worldPosition, Transform parent)
    {
        transform.parent = parent;
        transform.localPosition = new Vector3(worldPosition.x, worldPosition.y, 0);
    }

    public void Attack(IAttackable target)
    {
        target?.Attack();
    }

    private void Update()
    {
        IAttackable target = _targets?.FirstOrDefault();
        Attack(target);
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

    public class Factory : PlaceholderFactory<Vector2Int, Transform, Tower> { }
}
