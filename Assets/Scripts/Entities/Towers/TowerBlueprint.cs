using System.Collections.Generic;
using UnityEngine;
using static MapConstants;

public abstract class TowerBlueprint : ScriptableObject
{
    [SerializeField] private Sprite _validPlacementPreview;
    [SerializeField] private Sprite _invalidPlacementPreview;
    [SerializeField] public List<TileType> AllowedPlacements;

    public Sprite GetPlacementPreview(bool valid)
    {
        if (valid) return _validPlacementPreview;
        else return _invalidPlacementPreview;
    }    

    public abstract void Attack(IAttackable target);
}
