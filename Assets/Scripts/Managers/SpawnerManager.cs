using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    private List<EnemyStructure> _spawners;

    public void RegisterSpawner(EnemyStructure spawner)
    {
        if(_spawners == null) _spawners = new List<EnemyStructure>();

        _spawners.Add(spawner);
        spawner.OnDestroyed += RemoveSpawner;
    }

    private void RemoveSpawner(EnemyStructure spawner)
    {
        _spawners.Remove(spawner);
    }

    public void SpawnWave()
    {
        Debug.Log($"Attempting to spawn <color=cyan>{_spawners.Count}</color> enemies");
        for(int i = 0; i < _spawners.Count; i++)
        {
            _spawners[i].SpawnEnemy();
        }        
    }
}
