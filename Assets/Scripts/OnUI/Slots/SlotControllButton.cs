using UnityEngine;
using UnityEngine.UI;

public class SlotControllButton : MonoBehaviour
{
    [SerializeField] private Button _spin;
    [SerializeField] private Button _levelSelected;

    private void Awake()
    {
        SlotsGame.LastRowStoped += ShowSelectorButton;
        SlotsGame.RoundEnded += ShowSpinButton;
    }

    private void OnDestroy()
    {
        SlotsGame.LastRowStoped -= ShowSelectorButton;
        SlotsGame.RoundEnded -= ShowSpinButton;
    }

    private void ShowSelectorButton()
    {
        _spin.gameObject.SetActive(false);
        _levelSelected.gameObject.SetActive(true);
    }

    private void ShowSpinButton()
    {
        _spin.gameObject.SetActive(true);
        _levelSelected.gameObject.SetActive(false);
    }
}
