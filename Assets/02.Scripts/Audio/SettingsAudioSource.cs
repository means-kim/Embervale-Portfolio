using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public sealed class SettingsAudioSource : MonoBehaviour
{
    public enum Category { Sfx, Music }
    [SerializeField] private Category _category;
    private AudioSource _source;
    private float _baseVolume;
    private bool _initialized;

    private void Awake() => Initialize();
    private void OnEnable()
    {
        Initialize();
        GameSettings.AudioChanged += Apply;
        Apply();
    }

    private void OnDisable() => GameSettings.AudioChanged -= Apply;

    private void Initialize()
    {
        if (_initialized) return;
        _source = GetComponent<AudioSource>();
        _baseVolume = _source.volume;
        _initialized = true;
    }

    private void Apply()
    {
        _source.volume = _baseVolume * (_category == Category.Music
            ? GameSettings.MusicVolume : GameSettings.SfxVolume);
    }

    public static SettingsAudioSource Bind(AudioSource source, Category category = Category.Sfx)
    {
        if (source == null) return null;
        var gain = source.GetComponent<SettingsAudioSource>();
        if (gain == null) gain = source.gameObject.AddComponent<SettingsAudioSource>();
        gain.Initialize();
        gain._category = category;
        gain.Apply();
        return gain;
    }

    public static float GetBaseVolume(AudioSource source)
    {
        var gain = source.GetComponent<SettingsAudioSource>();
        if (gain == null) return source.volume;
        gain.Initialize();
        return gain._baseVolume;
    }

    public static void SetBaseVolume(AudioSource source, float value)
    {
        var gain = source.GetComponent<SettingsAudioSource>();
        if (gain == null) gain = Bind(source);
        gain.Initialize();
        gain._baseVolume = Mathf.Clamp01(value);
        gain.Apply();
    }
}
