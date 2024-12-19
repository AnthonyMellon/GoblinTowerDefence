using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private AttackableEntity _entity;
    [SerializeField] private Image _fill;
    [SerializeField] private Gradient _fillGradient;

    private float _currentFillValue = 1;

    private void OnEnable()
    {
        _entity.OnHealthChange += OnHealthChanged;
        UpdateFill();
    }

    private void OnDisable()
    {
        _entity.OnHealthChange -= OnHealthChanged;
    }

    private void OnHealthChanged(float currHealth, float maxHealth)
    {
        float value = currHealth / maxHealth;
        _currentFillValue = value;

        UpdateFill();
    }

    private void UpdateFill()
    {
        _fill.fillAmount = _currentFillValue;
        _fill.color = _fillGradient.Evaluate(_currentFillValue);
    }
}
