using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusShop : MonoBehaviour
{
    [SerializeField] private Bonus _type;
    [SerializeField] private TextMeshProUGUI _priceField;
    [SerializeField] private TextMeshProUGUI _amtField;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Layout _popUp;

    private int Price => BonusTracker.PriceList[_type];

    public void BuyBonus()
    {
        if (PlayerCoins.Amount < Price)
        {
            _popUp.gameObject.SetActive(true);
            return;
        }

        PlayerCoins.WithdrawCoins(Price);
        BonusTracker.BuyBonus(_type);
    }

    private void OnEnable()
    {
        UpdateFields();
        BonusTracker.BonusAmtChanged += UpdateFields;
    }

    private void OnDisable()
    {
        BonusTracker.BonusAmtChanged -= UpdateFields;
    }

    private void UpdateFields()
    {
        _priceField.text = Price.ToString();
        _amtField.text = BonusTracker.GetAmount(_type).ToString();
    }
}
