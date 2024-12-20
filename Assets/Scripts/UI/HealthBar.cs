using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private AttackableEntity _entity;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _fill;
    [SerializeField] private Gradient _fillGradient;
    [SerializeField] private float _displayCooldown;
    private float _currentDisplayCooldown;
    private float _currentFillValue = 1;

    private bool b_shouldDisplay;
    private bool _shouldDisplay { 
        get { return b_shouldDisplay; } 
        set { 
            b_shouldDisplay = value;
            UpdateCanvas();
        } 
    }

    private void OnEnable()
    {
        _entity.OnHealthChange += OnHealthChanged;
        UpdateFill();
    }

    private void OnDisable()
    {
        _entity.OnHealthChange -= OnHealthChanged;
    }

    private void Update()
    {
        if(_shouldDisplay)
        {
            UpdateCooldown();
        }
    }

    private void OnHealthChanged(float currHealth, float maxHealth)
    {
        float value = currHealth / maxHealth;
        _currentFillValue = value;

        _shouldDisplay = true;
        _currentDisplayCooldown = _displayCooldown;

        UpdateFill();
    }

    private void UpdateCooldown()
    {
        if(_currentDisplayCooldown > 0)
        {
            _currentDisplayCooldown -= Time.deltaTime;
        }

        if(_currentDisplayCooldown <= 0)
        {
            _shouldDisplay = false;
        }
    }

    private void UpdateCanvas()
    {
        _canvas.gameObject.SetActive(_shouldDisplay);
    }

    private void UpdateFill()
    {
        _fill.fillAmount = _currentFillValue;
        _fill.color = _fillGradient.Evaluate(_currentFillValue);
    }
}
