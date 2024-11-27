using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<Enemy> _enemies;

    public void AddEnemy(Enemy enemy)
    {
        if(_enemies == null) _enemies = new List<Enemy>();

        _enemies.Add(enemy);

        enemy.OnDeath += RemoveEnemy;
        enemy.transform.SetParent(transform, true);
    }

    private void RemoveEnemy(Enemy enemy)
    {
        _enemies?.Remove(enemy);
    }

    public void DestroyAllEnemies()
    {
        // Not gonna destroy enemies that don't exist...
        if (_enemies == null) return;

        // Stepping backwards through the list since enemies get removed from it when killed
        for(int i = _enemies.Count - 1; i >= 0; i--)
        {
            _enemies[i].Kill();
        }
    }
}
