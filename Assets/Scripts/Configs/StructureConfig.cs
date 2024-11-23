
using UnityEngine;

[CreateAssetMenu(fileName = "NewStructureConfig", menuName = "TowerDefenceGame/Configs/StructureConfig")]
public class StructureConfig : ScriptableObject
{
    [SerializeField] private PlayerStructure _playerStructure;
    [SerializeField] private EnemyStructure _enemyStructure;

    public PlayerStructure GetPlayerStructure() => _playerStructure;
    public EnemyStructure GetEnemyStructure() => _enemyStructure;
}
