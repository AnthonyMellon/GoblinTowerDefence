using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private MapManager _mapManager;

    [Inject]
    private void Initialize(MapManager mapManager)
    {
        _mapManager = mapManager;        
    }

    void Start()
    {
        _mapManager.GenerateMap();
        _mapManager.DrawMap();
    }
}
