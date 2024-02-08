using TMPro;
using UnityEngine;

public class WinningPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _amtField;
    [SerializeField] private Layout _layout;

    public void Show(int amt)
    {
        _amtField.text = "+" + amt.ToString();
        _layout.gameObject.SetActive(true);
    }

    private void Awake()
    {
        SlotsGame.PlayerWon += Show;
    }

    private void OnDestroy()
    {
        SlotsGame.PlayerWon -= Show;
    }
}
