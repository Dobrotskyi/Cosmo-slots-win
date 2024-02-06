using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static event Action SettingsChanged;
    public static bool SFXIsMuted => _sfxIsMuted;
    public static bool MusicIsMuted => _musicIsMuted;

    private static bool _musicIsMuted = true;
    private static bool _sfxIsMuted;

    public static void ToggleMusic() => Toggle(ref _musicIsMuted);

    public static void ToggleSFX() => Toggle(ref _sfxIsMuted);

    private static void Toggle(ref bool param)
    {
        param = !param;
        SettingsChanged?.Invoke();
    }

    private static SoundManager _instance = null;

    private const int MUSIC_OFF = -80;
    private const int MUSIC_ON = 0;

    //Music track: Wine by Lukrembo
    //Source: https://freetouse.com/music
    //Free Background Music for Videos
    [SerializeField] private AudioSource _musicAS;
    [SerializeField] private AudioSource _soundAS;
    [SerializeField] private AudioMixer _mixer;
    Button[] _buttons;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneLoadTracker.SceneLoaded += SubscribeButtons;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        OnAudioSettingsChanged();
        SettingsChanged += OnAudioSettingsChanged;
        SubscribeButtons();

        float test;
        _mixer.GetFloat("VolumeMusic", out test);
    }

    private void OnDestroy()
    {
        SettingsChanged -= OnAudioSettingsChanged;
        SceneLoadTracker.SceneLoaded -= SubscribeButtons;
        UnsubscribeButtons();
    }

    private void SubscribeButtons()
    {
        UnsubscribeButtons();
        _buttons = FindObjectsOfType<Button>(true);
        for (int i = 0; i < _buttons.Length; i++)
            _buttons[i].onClick.AddListener(PlayButtonSound);
    }

    private void UnsubscribeButtons()
    {
        if (_buttons == null)
            return;
        for (int i = 0; i < _buttons.Length; i++)
            _buttons[i].onClick.RemoveListener(PlayButtonSound);
    }

    private void OnAudioSettingsChanged()
    {
        if (MusicIsMuted)
            _mixer.SetFloat("VolumeMusic", MUSIC_OFF);
        else
            _mixer.SetFloat("VolumeMusic", MUSIC_ON);

        if (SFXIsMuted)
            _mixer.SetFloat("VolumeSFX", MUSIC_OFF);
        else
            _mixer.SetFloat("VolumeSFX", MUSIC_ON);
    }

    private void PlayButtonSound()
    {
        if (!SFXIsMuted)
            _soundAS.Play();
    }
}
