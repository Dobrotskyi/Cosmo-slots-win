using TMPro;
using UnityEngine;

public class Bet : MonoBehaviour
{
    public const int MIN_BET = 100;

    public int Amount { private set; get; } = 100;
    [SerializeField] private TextMeshProUGUI _field;
    private int _step = 100;

    public void Add()
    {
        if (PlayerCoins.Amount < Amount + _step)
            Amount = PlayerCoins.Amount;
        else
            Amount = Amount + _step;
        UpdateField();
    }

    public void Decrease()
    {
        if (Amount - _step < MIN_BET)
            Amount = MIN_BET;
        else
            Amount -= _step;
        UpdateField();
    }

    private void UpdateField()
    {
        _field.text = Amount.ToString();
    }

    private void OnEnable()
    {
        UpdateField();
    }
}
