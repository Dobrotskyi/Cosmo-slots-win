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
        SlotMachine.PlayerWon += Show;
    }

    private void OnDestroy()
    {
        SlotMachine.PlayerWon -= Show;
    }
}
