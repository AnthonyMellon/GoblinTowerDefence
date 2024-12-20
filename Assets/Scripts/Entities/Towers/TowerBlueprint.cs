using System.Collections.Generic;
using UnityEngine;
using static MapConstants;

public abstract class TowerBlueprint : ScriptableObject
{
    [SerializeField] private Sprite _validPlacementPreview;
    [SerializeField] private Sprite _invalidPlacementPreview;
    [SerializeField] public List<TileType> AllowedPlacements;
    [SerializeField] private int _cost;
    public int Cost { get { return _cost; } private set { _cost = value; } }

    public Sprite GetPlacementPreview(bool valid)
    {
        if (valid) return _validPlacementPreview;
        else return _invalidPlacementPreview;
    }    

    protected abstract void Attack(AttackableEntity target);
    public abstract void TryAttack(AttackableEntity target);
}
