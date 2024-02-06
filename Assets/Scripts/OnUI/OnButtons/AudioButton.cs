using UnityEngine;

[RequireComponent(typeof(ToggleButton))]
public class AudioButton : MonoBehaviour
{
    private enum Type
    {
        SFX,
        Music
    }
    [SerializeField] private Type _type;
    private ToggleButton _toggle;

    public void ChangeSetting()
    {
        switch (_type)
        {
            case Type.SFX:
                {
                    SoundManager.ToggleSFX();
                    break;
                }
            case Type.Music:
                {
                    SoundManager.ToggleMusic();
                    break;
                }
        }
        _toggle.Toggle();
    }

    private void OnEnable()
    {
        switch (_type)
        {
            case Type.SFX:
                {
                    if (SoundManager.SFXIsMuted)
                        if (_toggle.Status)
                            _toggle.Toggle();
                    break;
                }
            case Type.Music:
                {
                    if (SoundManager.MusicIsMuted)
                        if (_toggle.Status)
                            _toggle.Toggle();
                    break;
                }
        }
    }

    private void Awake()
    {
        _toggle = GetComponent<ToggleButton>();
    }
}
