using System;
using UnityEngine;
using Zenject;

public class Enemy : PathFollowerEntity, IAttackable
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [Tooltip("Color to be used when not pathing towards a player structure")]
    [SerializeField] private Color _neutralColor;
    [Tooltip("Color to be used when pathing towards a player structure")]
    [SerializeField] private Color _attackingColor;

    public Action<Enemy> OnDeath;

    [Inject]
    private void Initialize(float speed)
    {
        _speed = speed;
    }

    private void OnEnable()
    {
        OnTargetStructureChange += TargetStructureChanged;
    }

    private void OnDisable()
    {
        OnTargetStructureChange -= TargetStructureChanged;
    }

    private void TargetStructureChanged(StructureBase newStructure)
    {
        if (newStructure is EnemyStructure)
        {
            _spriteRenderer.color = _neutralColor;
        }
        else if (newStructure is PlayerStructure)
        {
            _spriteRenderer.color= _attackingColor;
        }
    }

    public void Attack()
    {
        Kill();
    }

    public override void Kill()
    {
        base.Kill();

        OnDeath?.Invoke(this);
    }

    public class Factory : PlaceholderFactory<float, Enemy> { };
}
