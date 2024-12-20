using System;
using UnityEngine;

public class Bank : MonoBehaviour
{
    [SerializeField] private int _startCurrency;

    private int b_storedCurrency;
    public int StoredCurrency { 
        get 
        { 
            return b_storedCurrency;
        }
        private set
        {
            b_storedCurrency = value;
            OnCurrencyChage?.Invoke(b_storedCurrency);
        }
    }
    public bool CanAfford(int amount) => amount <= StoredCurrency;

    public Action<int> OnCurrencyChage;

    private void Start()
    {
        StoredCurrency = _startCurrency;
    }

    public void AddCurrency(int amount)
    {
        StoredCurrency += amount;
    }

    /// <summary>
    /// Tries to remove from stored currency
    /// </summary>
    /// <param name="amount">Amount to remove</param>
    /// <returns>If removal was successful</returns>
    public bool TryRemoveCurrency(int amount)
    {
        if(CanAfford(amount))
        {
            StoredCurrency -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }
}
