using TMPro;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private SlotType _item;
    [SerializeField] private RectTransform _fog;
    [SerializeField] private RectTransform _mask;
    [SerializeField] private ParticleSystem _fadingEffect;
    [SerializeField] private bool _fogByDefault = true;
    [SerializeField] private TextMeshProUGUI _multipliersField;
    [SerializeField] private MultipliersConfigSO _multipliersConfig;

    [HideInInspector]
    public bool Visited;

    public float Multiplier => _multipliersConfig.GetMultiplierOf(Item, DifficultyLevel.Level);
    public SlotType Item => _item;
    public bool FogFaded => !_fog.gameObject.activeSelf;

    public void FadeFog(bool withEffect = true)
    {
        if (FogFaded)
            return;

        _fog.gameObject.SetActive(false);
        if (_mask != null)
            if (RectTransformUtility.RectangleContainsScreenPoint(_mask, _fog.position) && withEffect)
                Instantiate(_fadingEffect, _fog.position, Quaternion.identity);
    }

    public void InCombination()
    {
        _multipliersField.gameObject.SetActive(true);
        _multipliersField.text = "x" + Multiplier;
    }

    private void OnEnable()
    {
        SlotsGame.RoundStarted += NewRound;
        NewRound();
    }

    private void OnDisable()
    {
        SlotsGame.RoundStarted -= NewRound;
    }

    private void NewRound()
    {
        _multipliersField.gameObject.SetActive(false);
        Visited = false;
        if (_fogByDefault)
            _fog.gameObject.SetActive(true);
    }

}
