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
    [SerializeField, Range(0f, 1f)] private float _layerVolume = 0.25f;
    [SerializeField, Min(0.01f)] private float _fadeTime = 1f;
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _deathSource;
    private bool _deathMusicActive;
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
    private bool _endingBgmActive;
    private bool _endingBgmStopped;
    private float _endingBgmDuration;
    private float _endingBgmElapsed;
    private float[] _endingBgmStartVolumes;
    private bool[] _endingBgmWasPlaying;
    private int[] _endingBgmStopSamples;

    [Header("보스 2페이즈")]
    [SerializeField, Range(0f, 1f)] private float _bossPhase2Volume = 0.5f;

    private static readonly BgmType[] BossPhase2Layers =
    {
        BgmType.BossPhase2ChoirPad,
        BgmType.BossPhase2StringOstinato,
        BgmType.BossPhase2WarDrums
    };

    private static bool IsBossPhase2Layer(BgmType type)
    {
        return type == BgmType.BossPhase2ChoirPad ||
               type == BgmType.BossPhase2StringOstinato ||
               type == BgmType.BossPhase2WarDrums;
    }

    [Header("보스 3페이즈")]
    [SerializeField, Range(0f, 1f)] private float _bossPhase3Volume = 0.5f;

    private static readonly BgmType[] BossPhase3Layers =
    {
        BgmType.BossPhase3LowBrass,
        BgmType.BossPhase3Cello,
        BgmType.BossPhase3Violin
    };

    private static bool IsBossPhase3Layer(BgmType type)
    {
        return type == BgmType.BossPhase3LowBrass ||
               type == BgmType.BossPhase3Cello ||
               type == BgmType.BossPhase3Violin;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
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

        ResetEndingBgmFadeState();
        StopDeathBgm();
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
        if (_endingBgmActive)
        {
            UpdateEndingBgmFade();
            return;
        }
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
        foreach (BgmTrack track in _bgmTracks)
        {
            AudioSource source = track.Source;
            if (source == null)
            {
                continue;
            }

            float originalVolume = source.volume;
            source.playOnAwake = false;
            source.Stop();
            source.volume = 0f;
            if (source.clip == null)
            {
                continue;
            }

            int index = (int)track.Type;
            if (_bgmSources[index] != null)
            {
                continue;
            }

            source.loop = true;
            _bgmSources[index] = source;
            _bgmOriginVolumes[index] = originalVolume;
            _bgmRestartOnPlay[index] = track.RestartOnPlay;
        }

        double startDsp = AudioSettings.dspTime + 0.1;
        foreach (AudioSource source in _bgmSources)
        {
            if (source != null) source.PlayScheduled(startDsp);
        }
    }

    public void PlayBgm(BgmType type)
    {
        if (_deathMusicActive || _endingBgmActive)
        {
            return;
        }

        if (type == BgmType.Boss && !_bgmOn[(int)BgmType.Boss])
        {
            StopBossPhase2Layers();
            StopBossPhase3Layers();
        }

        SetBgm(type, true);
    }


    public void StopBgm(BgmType type) => SetBgm(type, false);

    public void PlayDeathBgm()
    {
        if (_deathMusicActive) return;
        CancelEndingBgmFade();
        _deathMusicActive = true;
        StopAllBgm();
        foreach (AudioSource source in _bgmSources)
        {
            if (source != null)
            {
                source.volume = 0f;
            }
        }

        if (_deathSource == null || _deathSource.clip == null)
        {
            return;
        }

        _deathSource.Stop();
        _deathSource.Play();
    }

    public void StopDeathBgm()
    {
        if (_deathSource != null) _deathSource.Stop();
        _deathMusicActive = false;
    }

    public void StopAllBgm()
    {
        if (_endingBgmActive) return;
        Array.Clear(_bgmOn, 0, _bgmOn.Length);
        ReCalculateBgmVolumes();
    }

    private void SetBgm(BgmType type, bool on)
    {
        if (_endingBgmActive) return;
        int index = (int)type;
        if (_bgmOn[index] == on)
        {
            return;
        }

        _bgmOn[index] = on;
        AudioSource source = _bgmSources[index];
        if (on && source != null && !source.isPlaying) source.Play();
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

            if (IsBossPhase2Layer((BgmType)i))
            {
                _bgmTargetVolumes[i] = _bossPhase2Volume;
                continue;
            }

            if (IsBossPhase3Layer((BgmType)i))
            {
                _bgmTargetVolumes[i] = _bossPhase3Volume;
                continue;
            }

            _bgmTargetVolumes[i] = _bgmOriginVolumes[i] * volume;
            volume *= _layerVolume;
        }
    }

    public void PlayBossPhase2Layers()
    {
        if (_deathMusicActive || _endingBgmActive)
        {
            return;
        }

        AudioSource boss = _bgmSources[(int)BgmType.Boss];

        if (boss == null || !boss.isPlaying || !_bgmOn[(int)BgmType.Boss])
        {
            Debug.LogWarning("보스 메인 BGM이 재생 중이 아니어서 2페이즈 레이어를 시작하지 못했습니다.", this);
            return;
        }

        bool alreadyOn = true;

        foreach (BgmType type in BossPhase2Layers)
        {
            AudioSource source = _bgmSources[(int)type];

            if (source == null || source.clip == null)
            {
                Debug.LogWarning("2페이즈 BGM 레이어 참조가 누락되었습니다: " + type, this);
                return;
            }

            alreadyOn &= _bgmOn[(int)type] && source.isPlaying;
        }

        if (alreadyOn)
        {
            return;
        }

        foreach (BgmType type in BossPhase2Layers)
        {
            int index = (int)type;

            StartSynchronizedBossLayer(_bgmSources[index], boss);
            _bgmOn[index] = true;
        }

        ReCalculateBgmVolumes();
    }

    public void StopBossPhase2Layers()
    {
        foreach (BgmType type in BossPhase2Layers)
        {
            _bgmOn[(int)type] = false;
        }

        ReCalculateBgmVolumes();
    }

    public void PlayBossPhase3Layers()
    {
        if (_deathMusicActive || _endingBgmActive)
        {
            return;
        }

        AudioSource boss = _bgmSources[(int)BgmType.Boss];

        if (boss == null || !boss.isPlaying || !_bgmOn[(int)BgmType.Boss])
        {
            Debug.LogWarning("보스 메인 BGM이 재생 중이 아니어서 3페이즈 레이어를 시작하지 못했습니다.", this);
            return;
        }

        bool alreadyOn = true;

        foreach (BgmType type in BossPhase3Layers)
        {
            AudioSource source = _bgmSources[(int)type];

            if (source == null || source.clip == null)
            {
                Debug.LogWarning("3페이즈 BGM 레이어 참조가 누락되었습니다: " + type, this);
                return;
            }

            alreadyOn &= _bgmOn[(int)type] && source.isPlaying;
        }

        // Phase 3 adds to the complete Phase 2 arrangement, including direct phase skips.
        PlayBossPhase2Layers();

        if (alreadyOn)
        {
            return;
        }

        foreach (BgmType type in BossPhase3Layers)
        {
            int index = (int)type;

            StartSynchronizedBossLayer(_bgmSources[index], boss);
            _bgmOn[index] = true;
        }

        ReCalculateBgmVolumes();
    }

    private void StartSynchronizedBossLayer(AudioSource source, AudioSource boss)
    {
        // Muted tracks may have stopped; raising their volume alone cannot resume them.
        if (!source.isPlaying) source.Play();
        double elapsedSeconds = (double)boss.timeSamples / boss.clip.frequency;
        long sample = (long)(elapsedSeconds * source.clip.frequency);
        source.timeSamples = (int)(sample % source.clip.samples);
    }

    public void StopBossPhase3Layers()
    {
        foreach (BgmType type in BossPhase3Layers)
        {
            _bgmOn[(int)type] = false;
        }

        ReCalculateBgmVolumes();
    }

    public void BeginEndingBgmFade(float duration)
    {
        if (_endingBgmActive || _bgmSources == null) return;
        _endingBgmActive = true;
        _endingBgmStopped = false;
        _endingBgmDuration = Mathf.Max(0f, duration);
        _endingBgmElapsed = 0f;
        _endingBgmStartVolumes = new float[_bgmSources.Length];
        _endingBgmWasPlaying = new bool[_bgmSources.Length];
        _endingBgmStopSamples = new int[_bgmSources.Length];
        for (int i = 0; i < _bgmSources.Length; i++)
        {
            var source = _bgmSources[i];
            if (source == null) continue;
            _endingBgmStartVolumes[i] = source.volume;
            _endingBgmWasPlaying[i] = source.isPlaying;
        }
        if (_endingBgmDuration <= 0f) FinishEndingBgmFade();
    }

    private void UpdateEndingBgmFade()
    {
        if (_endingBgmStopped) return;
        _endingBgmElapsed += Time.unscaledDeltaTime;
        float progress = Mathf.Clamp01(_endingBgmElapsed / Mathf.Max(0.001f, _endingBgmDuration));
        float gain = 1f - Mathf.SmoothStep(0f, 1f, progress);
        for (int i = 0; i < _bgmSources.Length; i++)
            if (_bgmSources[i] != null) _bgmSources[i].volume = _endingBgmStartVolumes[i] * gain;
        if (progress >= 1f) FinishEndingBgmFade();
    }

    public void FinishEndingBgmFade()
    {
        if (!_endingBgmActive || _endingBgmStopped) return;
        _endingBgmStopped = true;
        for (int i = 0; i < _bgmSources.Length; i++)
        {
            var source = _bgmSources[i];
            if (source == null) continue;
            _endingBgmStopSamples[i] = source.timeSamples;
            source.volume = 0f;
            source.Stop();
        }
    }

    public void CancelEndingBgmFade()
    {
        if (!_endingBgmActive) return;
        for (int i = 0; i < _bgmSources.Length; i++)
        {
            var source = _bgmSources[i];
            if (source == null) continue;
            source.volume = _endingBgmStartVolumes[i];
            if (_endingBgmStopped && _endingBgmWasPlaying[i])
            {
                source.Play();
                if (source.clip != null)
                    source.timeSamples = Mathf.Clamp(_endingBgmStopSamples[i], 0, source.clip.samples - 1);
            }
        }
        ResetEndingBgmFadeState();
    }

    private void ResetEndingBgmFadeState()
    {
        _endingBgmActive = _endingBgmStopped = false;
        _endingBgmStartVolumes = null;
        _endingBgmWasPlaying = null;
        _endingBgmStopSamples = null;
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
