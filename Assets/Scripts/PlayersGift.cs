using TMPro;
using UnityEngine;

public class PlayersGift : MonoBehaviour
{
    [SerializeField] private Vector2 RangeMinMax = new(500, 1000);
    [SerializeField] private Layout _layout;
    [SerializeField] private Layout _giftLayout;
    [SerializeField] private TextMeshProUGUI _textField;
    private bool _given = false;

    public void TakeGift()
    {
        if (_given) return;
        _given = true;
        int gift = Random.Range((int)RangeMinMax.x, (int)RangeMinMax.y);
        PlayerCoins.AddCoins(gift);
        _textField.text = "+" + gift.ToString();
        _giftLayout.gameObject.SetActive(true);
        _giftLayout.gameObject.SetActive(true);
    }

    private void Awake()
    {
        PlayerCoins.NotEnoughCoins += Show;
        _layout.Closed += Closed;
    }

    private void OnDestroy()
    {
        PlayerCoins.NotEnoughCoins -= Show;
        _layout.Closed -= Closed;
    }

    private void Show()
    {
        _layout.gameObject.SetActive(true);
    }

    private void Closed()
    {
        _given = false;
        _giftLayout.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    public bool Test;
    private void Update()
    {
        if (Test)
        {
            Test = false;
            Show();
        }
    }
#endif
}
