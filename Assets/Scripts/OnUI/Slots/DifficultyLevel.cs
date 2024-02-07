using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyLevel : MonoBehaviour
{
    private const string PREF = "Level:";
    [SerializeField] private Button _next;
    [SerializeField] private Button _prev;
    [SerializeField] private TextMeshProUGUI _textField;

    public Levels Level { private set; get; } = Levels.First;
    public string CreateString => PREF + ((int)Level + 1).ToString();
    public void Next()
    {
        Level = Level.GetNext();
        UpdateFields();
    }

    public void Prev()
    {
        Level = Level.GetPrev();
        UpdateFields();
    }

    private void Awake()
    {
        UpdateFields();
    }

    private void UpdateFields()
    {
        if (Level == Level.GetNext())
            _next.interactable = false;
        else
            _next.interactable = true;
        if (Level == Level.GetPrev())
            _prev.interactable = false;
        else
            _prev.interactable = true;

        _textField.text = CreateString;
    }
}
