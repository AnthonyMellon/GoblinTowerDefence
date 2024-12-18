using System;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [Tooltip("Color to be used when not pathing towards a player structure")]
    [SerializeField] private Color _neutralColor;
    [Tooltip("Color to be used when pathing towards a player structure")]
    [SerializeField] private Color _attackingColor;

    [SerializeField] private PathFollowerEntity _pathFollower;
    [SerializeField] private AttackableEntity _attackable;
    [SerializeField] private Collider2D _collider;

    public Action<Enemy> OnDeath;

    [Inject]
    private void Initialize(float speed, float maxHealth)
    {
        _pathFollower.SetStats(speed);
        _attackable.SetStats(maxHealth);
    }

    private void OnEnable()
    {
        _pathFollower.OnTargetStructureChange += UpdateColour;
        _pathFollower.OnPathEnded += Kill;
        _attackable.OnDeath += Kill;
    }

    private void OnDisable()
    {
        _pathFollower.OnTargetStructureChange -= UpdateColour;
        _pathFollower.OnPathEnded -= Kill;
        _attackable.OnDeath -= Kill;
    }

    private void UpdateColour(StructureBase targetStructure)
    {
        if(targetStructure is EnemyStructure)
        {
            _spriteRenderer.color = _neutralColor;
        }
        else if (targetStructure is PlayerStructure)
        {
            _spriteRenderer.color = _attackingColor;
        }
    }

    public void Kill()
    {
        OnDeath?.Invoke(this);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void SetPath(List<Vector2Int> points, StructureBase targetStructure)
    {
        _pathFollower.SetPath(points, targetStructure);
    }

    public void EnableCollision(bool enable)
    {
        _collider.enabled = enable;
    }

    public class Factory : PlaceholderFactory<float, float, Enemy> { };
}
