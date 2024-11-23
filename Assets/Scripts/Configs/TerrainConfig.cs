using UnityEngine;

[CreateAssetMenu(fileName = "NewTerrainConfig", menuName = "TowerDefenceGame/Configs/TerrainConfig")]
public class TerrainConfig : ScriptableObject
{
    [Header("Spawns Terrain")]
    [Tooltip("How large of an area around spawn should be grass land")]
    [SerializeField] float _safeZoneSize;

    [Header("Terrain Heights")]
    [SerializeField] float _maxCoastHeight;
    [SerializeField] float _minHillHeight;

    [Header("Terrain Noise Settings")]
    [SerializeField] uint _octaveCount;
    [SerializeField] float _frequency;
    [SerializeField] float _amplitude;
    [SerializeField] float _persistance;
    [SerializeField] float _lacunarity;

    public float GetMaxCoastHeight() => _maxCoastHeight;
    public float GetMinHillHeight() => _minHillHeight;
    public float GetGrassHeight() => (_maxCoastHeight + _minHillHeight) / 2;
    public float GetSafeZoneSize() => _safeZoneSize;
    public uint GetOctaveCount() => _octaveCount;
    public float GetFrequency() => _frequency;
    public float GetAmplitude() => _amplitude;
    public float GetPersistance() => _persistance;
    public float GetLacunarity() => _lacunarity;
}
