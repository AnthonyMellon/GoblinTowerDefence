using UnityEngine;

[CreateAssetMenu(fileName = "basicAttacker", menuName = "TowerDefenceGame/Towers/BasicAttacker")]
public class Tower_BasicAttacker : TowerBlueprint
{
    [Tooltip("How much damage each attack does")]
    [SerializeField] private int _power;
    [Tooltip("Time (in seconds) between each attack")]
    [SerializeField] private float _attackCooldown;

    private float _lastAttackCooldown = 0;

    protected override void Attack(AttackableEntity target)
    {
        if (target == null) return;

        target.Damage(_power);
        _lastAttackCooldown = _attackCooldown;
    }

    public override void TryAttack(AttackableEntity target)
    {
        _lastAttackCooldown -= Time.deltaTime;
        if(_lastAttackCooldown <= 0) Attack(target);
    }
}
