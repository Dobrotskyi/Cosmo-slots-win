using UnityEngine;
using UnityEngine.UI;

public abstract class BonusUser : MonoBehaviour
{
    [SerializeField] private Button _button;

    public abstract Bonus Type { get; }
    public bool Used
    {
        protected set;
        get;
    }

    public void Use()
    {
        Used = true;
        _button.interactable = false;
        Apply();
        BonusTracker.UseBonus(Type);
    }

    protected abstract void Apply();

    private void OnEnable()
    {
        SlotsGame.RoundStarted += HandlePulled;
        SlotsGame.LastRowStoped += EnableButton;
        BonusTracker.BonusAmtChanged += UpdateUseButton;
        SlotsGame.Revealed += HandlePulled;
        _button.interactable = false;
    }

    private void OnDisable()
    {
        SlotsGame.RoundStarted -= HandlePulled;
        SlotsGame.Revealed -= HandlePulled;
        SlotsGame.LastRowStoped -= EnableButton;
        BonusTracker.BonusAmtChanged -= UpdateUseButton;
    }

    private void UpdateUseButton()
    {
        if (BonusTracker.GetAmount(Type) <= 0)
            _button.interactable = false;
    }

    private void HandlePulled()
    {
        _button.interactable = false;
        Used = false;
    }

    private void EnableButton()
    {
        if (BonusTracker.GetAmount(Type) <= 0 || Used)
            return;
        _button.interactable = true;
    }
}
