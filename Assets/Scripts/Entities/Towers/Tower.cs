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
        TryAttack();
    }

    private void TryAttack()
    {
        AttackableEntity target = _targets?.FirstOrDefault();
        LookAtTarget(target?.transform);
        _towerBlueprint.TryAttack(target);
    }

    private void LookAtTarget(Transform target)
    {
        if(target == null) return;

        Vector2 distance = new Vector2(
            target.position.x - transform.position.x,
            target.position.y - transform.position.y
            );

        float rotation = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg - 90; // apparently I need to remove 90 degres here
        Vector3 currRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(currRotation.x, currRotation.y, rotation);
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
