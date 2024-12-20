using TMPro;
using UnityEngine;
using Zenject;

public class GameUI : MonoBehaviour
{
    [Header("PlayerInfo")]
    [SerializeField] private TMP_Text _currency;
    private string _currencyString;

    private PlayerManager _playerManager;

    [Inject]
    private void Initialize(PlayerManager playerManager)
    {
        _playerManager = playerManager;
        _currencyString = LocalisationProvider.LocaliseText("UI_CURRENCY", null, false);
    }

    private void OnEnable()
    {
        _playerManager.Bank.OnCurrencyChage += UpdateCurrency;
    }

    private void OnDisable()
    {
        _playerManager.Bank.OnCurrencyChage -= UpdateCurrency;
    }

    private void UpdateCurrency(int newCurrency)
    {
        string[] args = new string[]
        {
            _playerManager.Bank.StoredCurrency.ToString()
        };
        _currency.text = LocalisationProvider.ReplaceTokens(_currencyString, args);
    }
}
