using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Bank _bank;
    public Bank Bank { get { return _bank; } private set { _bank = value; } }
}
