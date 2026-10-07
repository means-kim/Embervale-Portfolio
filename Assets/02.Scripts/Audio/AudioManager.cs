using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [Serializable]
    private struct BgmTrack
    {
        public BgmType Type;
        public AudioSource Source;
        [Tooltip("켜질 때 처음부터 재생")] public bool RestartOnPlay;
    }
    [Header("BGM")]
    [SerializeField] private BgmTrack[] _bgmTracks;
    [Tooltip("위에 다른 곡이 얹혔을 때 아래 곡의 볼륨 배율")]
    [SerializeField, Range(0f, 1f)] private float _layerVolume = 0.25f;
    [SerializeField, Min(0.01f)] private float _fadeTime = 1f;
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _sfxSource;
    [Header("Footsteps (비워 두면 SFX Source로 대체)")]
    [SerializeField] private AudioSource _footstepSource;
    [Header("Scene Names")]
    [SerializeField] private string _outGameSceneName = "OutGameScene";
    [SerializeField] private string _inGameSceneName = "InGameScene";
    private AudioSource[] _bgmSources;
    private bool[] _bgmOn;
    private bool[] _bgmRestartOnPlay;
    private float[] _bgmTargetVolumes;
    private float[] _bgmOriginVolumes;
    private string _currentSceneName;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitBgmSources();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    {
        if (Instance != this) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive)
        {
            return;
        }
        StopAllBgm();
        if (GameIntroCinematic.IsTransitioning)
        {
            return;
        }
        if (scene.name == _outGameSceneName)
        {
            PlayBgm(BgmType.Title);
        }
        else if (scene.name == _inGameSceneName)
        {
            PlayBgm(BgmType.Snow);
        }
        _currentSceneName = scene.name;
    }
    private void Update()
    {
        for (int i = 0; i < _bgmSources.Length; i++)
        {
            AudioSource source = _bgmSources[i];
            if (source == null)
            {
                continue;
            }
            source.volume = Mathf.MoveTowards(source.volume, _bgmTargetVolumes[i], Time.unscaledDeltaTime / _fadeTime);
        }
    }
    private void InitBgmSources()
    {
        int count = Enum.GetValues(typeof(BgmType)).Length;
        _bgmSources = new AudioSource[count];
        _bgmOn = new bool[count];
        _bgmRestartOnPlay = new bool[count];
        _bgmTargetVolumes = new float[count];
        _bgmOriginVolumes = new float[count];
        double startDsp = AudioSettings.dspTime + 0.1;
        foreach (BgmTrack track in _bgmTracks)
        {
            AudioSource source = track.Source;
            if (source == null || source.clip == null)
            {
                continue;
            }
            int index = (int)track.Type;
            if (_bgmSources[index] != null)
            {
                continue;
            }
            source.loop = true;
            source.PlayScheduled(startDsp);
            _bgmSources[index] = source;
            _bgmOriginVolumes[index] = source.volume;
            _bgmRestartOnPlay[index] = track.RestartOnPlay;
            source.volume = 0f;
        }
    }
    public void PlayBgm(BgmType type) => SetBgm(type, true);
    public void StopBgm(BgmType type) => SetBgm(type, false);
    public void StopAllBgm()
    {
        Array.Clear(_bgmOn, 0, _bgmOn.Length);
        ReCalculateBgmVolumes();
    }
    private void SetBgm(BgmType type, bool on)
    {
        int index = (int)type;
        if (_bgmOn[index] == on)
        {
            return;
        }
        _bgmOn[index] = on;
        AudioSource source = _bgmSources[index];
        if (on && _bgmRestartOnPlay[index] && source != null)
        {
            source.time = 0f;
        }
        ReCalculateBgmVolumes();
    }
    private void ReCalculateBgmVolumes()
    {
        float volume = 1f;
        for (int i = _bgmOn.Length - 1; i >= 0; i--)
        {
            if (!_bgmOn[i])
            {
                _bgmTargetVolumes[i] = 0f;
                continue;
            }
            _bgmTargetVolumes[i] = _bgmOriginVolumes[i] * volume;
            volume *= _layerVolume;
        }
    }
    public void PlaySFX(SFXData sfxData)
    {
        if (sfxData == null || sfxData.Clip == null || _sfxSource == null) return;
        _sfxSource.pitch = sfxData.Pitch;
        _sfxSource.PlayOneShot(sfxData.Clip, sfxData.Volume);
    }
    public void PlayFootstep(FootstepSFXBank bank)
    {
        if (bank == null || !bank.HasClips)
        {
            return;
        }
        AudioClip clip = bank.PickRandom();
        if (clip == null)
        {
            return;
        }
        AudioSource source = _footstepSource != null ? _footstepSource : _sfxSource;
        if (source == null)
        {
            return;
        }
        source.pitch = bank.RollPitch();
        source.PlayOneShot(clip, bank.Volume);
    }
}
