using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private SlotType _item;
    [SerializeField] private RectTransform _fog;
    [SerializeField] private RectTransform _mask;
    [SerializeField] private ParticleSystem _fadingEffect;
    [SerializeField] private bool _fogByDefault = true;

    public bool Visited;
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

    private void OnEnable()
    {
        SlotMachine.HandlePulled += NewRound;
    }

    private void OnDisable()
    {
        SlotMachine.HandlePulled -= NewRound;
    }

    private void NewRound()
    {
        Visited = false;
        if (_fogByDefault)
            _fog.gameObject.SetActive(true);
    }

}
