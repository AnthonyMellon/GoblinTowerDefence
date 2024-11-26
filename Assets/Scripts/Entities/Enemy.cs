using UnityEngine;
using Zenject;

public class Enemy : PathFollowerEntity
{
    public class Factory : PlaceholderFactory<Enemy> { };
}
