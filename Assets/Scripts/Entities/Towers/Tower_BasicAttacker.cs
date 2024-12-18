using UnityEngine;

[CreateAssetMenu(fileName = "basicAttacker", menuName = "TowerDefenceGame/Towers/BasicAttacker")]
public class Tower_BasicAttacker : TowerBlueprint
{
    public override void Attack(IAttackable target)
    {
        if (target == null) return;

        target.Attack();
    }
}
