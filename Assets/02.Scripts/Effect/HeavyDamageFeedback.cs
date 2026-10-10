using UnityEngine;
using UnityEngine.UI;

// 플레이어 피격 연출만 담당한다. 기존 Canvas와 AudioListener의 필터를 사용한다.
public sealed class HeavyDamageFeedback : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Image _vignette;
    [SerializeField] private AudioLowPassFilter _lowPass;
    [SerializeField, Min(1)] private int _damageThreshold = 80;
    [SerializeField] private Color _color = new Color(0.7f, 0.015f, 0.025f, 1f);
    [SerializeField, Range(0f, 1f)] private float _opacity = 0.28f;
    [SerializeField, Min(0.01f)] private float _attackDuration = 0.08f;
    [SerializeField, Min(0f)] private float _holdDuration = 0.25f;
    [SerializeField, Min(0.01f)] private float _releaseDuration = 1.2f;
    [SerializeField, Range(10f, 22000f)] private float _muffledCutoff = 900f;

    public bool IsPlaying { get; private set; }
    private float _elapsed;
    private float _strength;
    private float _startStrength;
    private float _originalCutoff;
    private bool _originalFilterEnabled;

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.OnDamaged += HandleDamage;
            _health.OnDie += StopFeedback;
        }
        SetVignette(0f);
    }

    private void HandleDamage(int damage)
    {
        if (_health == null || _health.IsDead || damage < _damageThreshold)
        {
            return;
        }
        if (!IsPlaying && _lowPass != null)
        {
            _originalCutoff = _lowPass.cutoffFrequency;
            _originalFilterEnabled = _lowPass.enabled;
        }
        _startStrength = _strength;
        _elapsed = 0f;
        IsPlaying = true;
    }

    private void Update()
    {
        if (!IsPlaying)
        {
            return;
        }
        if (_health == null || !_health.gameObject.activeInHierarchy || _health.IsDead)
        {
            StopFeedback();
            return;
        }
        // 히트스톱이나 슬로모션 중에도 원래 소리로 복귀한다.
        _elapsed += Time.unscaledDeltaTime;
        float attack = Mathf.Max(0.01f, _attackDuration);
        float release = Mathf.Max(0.01f, _releaseDuration);
        if (_elapsed < attack)
        {
            _strength = Mathf.Lerp(_startStrength, 1f, _elapsed / attack);
        }
        else
        {
            _strength = 1f - Mathf.SmoothStep(0f, 1f, (_elapsed - attack - _holdDuration) / release);
        }
        if (_elapsed >= attack + _holdDuration + release)
        {
            StopFeedback();
            return;
        }
        SetVignette(_strength);
        if (_lowPass != null)
        {
            float clearCutoff = _originalFilterEnabled ? _originalCutoff : 22000f;
            float target = Mathf.Min(clearCutoff, _muffledCutoff);
            _lowPass.cutoffFrequency = Mathf.Exp(Mathf.Lerp(Mathf.Log(Mathf.Max(10f, clearCutoff)), Mathf.Log(Mathf.Max(10f, target)), _strength));
            _lowPass.enabled = true;
        }
    }

    private void SetVignette(float strength)
    {
        if (_vignette == null)
        {
            return;
        }
        Color color = _color;
        color.a = _opacity * strength;
        _vignette.color = color;
        _vignette.enabled = strength > 0f;
    }

    private void StopFeedback()
    {
        if (IsPlaying && _lowPass != null)
        {
            _lowPass.cutoffFrequency = _originalCutoff;
            _lowPass.enabled = _originalFilterEnabled;
        }
        IsPlaying = false;
        _strength = 0f;
        SetVignette(0f);
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnDamaged -= HandleDamage;
            _health.OnDie -= StopFeedback;
        }
        StopFeedback();
    }
}
