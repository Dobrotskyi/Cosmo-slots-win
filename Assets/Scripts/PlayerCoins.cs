using System;
using TMPro;
using UnityEngine;

public class PlayerCoins : MonoBehaviour
{
    public static event Action NotEnoughCoins;
    public static event Action AmountChanged;

    private const string KEY = "Player Coins";

    [SerializeField] private TextMeshProUGUI _field;

    public static int Amount
    {
        get => PlayerPrefs.GetInt(KEY, 1000);
        set
        {
            PlayerPrefs.SetInt(KEY, value);
            AmountChanged?.Invoke();
        }
    }

    public static void InvokeIfNotEnough()
    {
        if (Amount < Bet.MIN_BET)
            NotEnoughCoins?.Invoke();
    }

    public static void AddCoins(int amt)
    {
        Amount += amt;
    }

    public static bool WithdrawCoins(int amt)
    {
        if (Amount < amt) return false;
        Amount -= amt;
        return true;
    }

    private void Awake()
    {
        UpdateField();
        AmountChanged += UpdateField;
    }

    private void OnDestroy()
    {
        AmountChanged -= UpdateField;
    }

    private void UpdateField()
    {
        _field.text = Amount.ToString();
    }
}
