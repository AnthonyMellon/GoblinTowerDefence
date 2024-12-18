using System;
using UnityEngine;

public class AttackableEntity : MonoBehaviour
{
    private float _maxHealth;
    private float _currentHealth;

    public Action OnDeath;

    public void SetStats(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = _maxHealth;
    }

    public void Damage(float damage)
    {
        _currentHealth -= damage;
        Debug.Log($"New health is <color=red>{_currentHealth}</color>/<color=green>{_maxHealth}</color>");
        if (_currentHealth <= 0) OnDeath?.Invoke();
    }
}
