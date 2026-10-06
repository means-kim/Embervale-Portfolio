using UnityEngine;

// 공용 히트스톱
public class HitImpactFeedback : MonoBehaviour
{
    public static HitImpactFeedback Instance { get; private set; }
    [SerializeField, Min(0f)] private float _bossHitstopDuration = 0.05f;
    [SerializeField, Min(0f)] private float _playerHitstopDuration = 0.08f;
    private bool _isStopping;
    private float _resumeAt;
    private float _previousTimeScale;
    private float _appliedTimeScale;
    public bool IsStopping => _isStopping;

    public static HitImpactFeedback EnsureInstance()
    {
        if (Instance != null && Instance.isActiveAndEnabled)
        {
            return Instance;
        }

        var existing = FindFirstObjectByType<HitImpactFeedback>();
        if (existing != null && existing.isActiveAndEnabled)
        {
            return existing;
        }

        return new GameObject(nameof(HitImpactFeedback)).AddComponent<HitImpactFeedback>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (_isStopping && Time.unscaledTime >= _resumeAt) RestoreTimeScale();
    }

    public void RequestBossHitstop() => RequestHitstop(_bossHitstopDuration);
    public void RequestPlayerHitstop() => RequestHitstop(_playerHitstopDuration);

    public void RequestHitstop(float duration, float timeScale = 0f)
    {
        if (!isActiveAndEnabled || Instance != this || duration <= 0f)
        {
            return;
        }

        timeScale = Mathf.Clamp01(timeScale);
        float resumeAt = Time.unscaledTime + duration;
        if (_isStopping)
        {
            if (!Mathf.Approximately(Time.timeScale, _appliedTimeScale))
            {
                _isStopping = false;
                return;
            }

            _resumeAt = Mathf.Max(_resumeAt, resumeAt);
            _appliedTimeScale = Mathf.Min(_appliedTimeScale, timeScale);
        }
        else
        {
            if (Time.timeScale <= 0f)
            {
                return;
            }

            _previousTimeScale = Time.timeScale;
            _resumeAt = resumeAt;
            _appliedTimeScale = Mathf.Min(Time.timeScale, timeScale);
            _isStopping = true;
        }

        Time.timeScale = _appliedTimeScale;
    }

    private void RestoreTimeScale()
    {
        if (!_isStopping)
        {
            return;
        }

        _isStopping = false;
        if (Mathf.Approximately(Time.timeScale, _appliedTimeScale))
            Time.timeScale = _previousTimeScale;
    }

    private void OnDisable()
    {
        RestoreTimeScale();
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        RestoreTimeScale();
        if (Instance == this)
        {
            Instance = null;
        }
    }
}