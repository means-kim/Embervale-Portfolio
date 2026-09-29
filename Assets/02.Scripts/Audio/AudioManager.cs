using UnityEngine;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    [Header("BGM - Title (OutGameScene 전용)")]
    [SerializeField] private AudioClip _titleBgm;
    [Header("BGM - InGame 1~3 (플레이 중에만, 진입 시 랜덤 1곡)")]
    [SerializeField] private AudioClip _inGameBgm1;
    [SerializeField] private AudioClip _inGameBgm2;
    [SerializeField] private AudioClip _inGameBgm3;
    [Header("BGM - Default (그 외 상황)")]
    [SerializeField] private AudioClip _defaultBgm;
    [Header("Scene Names")]
    [SerializeField] private string _outGameSceneName = "OutGameScene";
    [SerializeField] private string _inGameSceneName = "InGameScene";
    private string _currentSceneName;
    private AudioClip _pickedInGameBgm;
    private bool _wasPlaying;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _currentSceneName = SceneManager.GetActiveScene().name;
    }
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentSceneName = scene.name;
        _wasPlaying = IsPlayingNow();
        if (_wasPlaying)
        {
            _pickedInGameBgm = PickRandomInGameBgm();
        }
        RefreshBGM();
    }
    private void Update()
    {
        bool isPlaying = IsPlayingNow();
        if (isPlaying && !_wasPlaying) _pickedInGameBgm = PickRandomInGameBgm();
        _wasPlaying = isPlaying;
        RefreshBGM();
    }
    private bool IsPlayingNow()
    {
        if (_currentSceneName != _inGameSceneName) return false;
        return true;
    }
    public void UpdateAndPlayBGM() => RefreshBGM();
    private void RefreshBGM()
    {
        if (_bgmSource == null) return;
        AudioClip targetClip = ResolveTargetClip();
        _bgmSource.pitch = 1f;
        if (_bgmSource.clip == targetClip) return;
        _bgmSource.clip = targetClip;
        _bgmSource.loop = true;
        if (targetClip != null) _bgmSource.Play();
        else _bgmSource.Stop();
    }
    private AudioClip ResolveTargetClip()
    {
        if (_currentSceneName == _outGameSceneName) return _titleBgm;
        if (IsPlayingNow()) return _pickedInGameBgm != null ? _pickedInGameBgm : _defaultBgm;
        return _defaultBgm;
    }
    private AudioClip PickRandomInGameBgm()
    {
        AudioClip[] candidates = { _inGameBgm1, _inGameBgm2, _inGameBgm3 };
        int validCount = 0;
        for (int i = 0; i < candidates.Length; i++)
        {
            if (candidates[i] != null) validCount++;
        }
        if (validCount == 0) return null;

        int pick = Random.Range(0, validCount);
        for (int i = 0; i < candidates.Length; i++)
        {
            if (candidates[i] == null) continue;
            if (pick == 0) return candidates[i];
            pick--;
        }
        return null;
    }
    public void PlaySFX(SFXData sfxData)
    {
        if (sfxData == null || sfxData.Clip == null || _sfxSource == null) return;
        _sfxSource.pitch = sfxData.Pitch;
        _sfxSource.PlayOneShot(sfxData.Clip, sfxData.Volume);
    }
}
