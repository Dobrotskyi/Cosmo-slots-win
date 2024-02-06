using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private GameObject _on;
    [SerializeField] private GameObject _off;

    public bool Status => _on.gameObject.activeSelf;

    public void Toggle()
    {
        _on.SetActive(!_on.activeSelf);
        _off.SetActive(!_on.activeSelf);
    }

    private void Awake()
    {
        _on.SetActive(true);
        _off.SetActive(false);
    }
}
