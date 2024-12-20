using UnityEngine;

public class Bank : MonoBehaviour
{
    [SerializeField] private int _startCurrency;
    public int _storedCurrency { get; private set; }
    public bool CanAfford(int amount) => amount <= _storedCurrency;

    private void Start()
    {
        _storedCurrency = _startCurrency;
    }

    public void AddCurrency(int amount)
    {
        _storedCurrency += amount;
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
            _storedCurrency -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }
}
