using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackableEntity : MonoBehaviour
{
    private float _maxHealth;
    private float _currentHealth;

    public Action OnDeath;
    /// <summary>
    /// currHealth, maxHealth
    /// </summary>
    public Action<float, float> OnHealthChange;

    public void SetStats(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = _maxHealth;
    }

    public void Damage(float damage)
    {
        _currentHealth -= damage;
        OnHealthChange?.Invoke(_currentHealth, _maxHealth);
        if (_currentHealth <= 0) OnDeath?.Invoke();
    }
}
