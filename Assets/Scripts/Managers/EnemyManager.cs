using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<Enemy> _enemies;

    public void AddEnemy(Enemy enemy)
    {
        if(_enemies == null) _enemies = new List<Enemy>();

        _enemies.Add(enemy);
    }
}
