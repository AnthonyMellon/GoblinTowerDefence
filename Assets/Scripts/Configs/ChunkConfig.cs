using UnityEngine;

[CreateAssetMenu(fileName = "NewChunkConfig", menuName = "TowerDefenceGame/Configs/ChunkConfig")]
public class ChunkConfig : ScriptableObject
{
    [SerializeField] private int _chunkSize;
    [Header("Measured in Chunks")]
    [SerializeField] private int _mapRadius;
    [Header("Measured in Chunks")]
    [SerializeField] private int _initialRevealRadius;

    public int GetChunkSize() => _chunkSize;
    public int GetMapRadius() => _mapRadius;
    public int GetInitialRevealRadius() => _initialRevealRadius;
}
