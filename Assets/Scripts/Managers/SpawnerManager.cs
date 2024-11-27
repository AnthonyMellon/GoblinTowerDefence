using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    private List<EnemyStructure> _spawners;

    public void RegisterSpawner(EnemyStructure spawner)
    {
        if(_spawners == null) _spawners = new List<EnemyStructure>();

        _spawners.Add(spawner);
    }

    public void SpawnWave()
    {
        for(int i = 0; i < _spawners.Count; i++)
        {
            _spawners[i].SpawnEnemy();
        }
    }
}
