using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackTargeter : MonoBehaviour
{
    [SerializeField] private Tower _attacker;
    [SerializeField] private Collider2D _attackArea;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IAttackable target))
        {
            _attacker.AddTarget(target);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IAttackable target))
        {
            _attacker.RemoveTarget(target);
        }
    }
}
