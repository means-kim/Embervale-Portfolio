using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[DefaultExecutionOrder(100)]
public sealed class BossStaggerBar : MonoBehaviour
{
    [SerializeField] private AirBossStagger _stagger;
    [SerializeField] private Image _fill;
    [SerializeField] private Image _fillTip;
    [SerializeField] private Color _tipColor = new Color(1f, 0.94f, 0.4f, 1f);
    [SerializeField, Min(1f)] private float _tipWidth = 6f;

    [Header("Visibility")]
    [SerializeField] private Color _gaugeColor = new Color(1f, 0.72f, 0.18f, 1f);
    [SerializeField] private Color _readyColor = new Color(1f, 0.91f, 0.48f, 1f);
    [SerializeField] private Color _staggerColor = new Color(1f, 0.96f, 0.72f, 1f);
    [SerializeField, Range(0.01f, 1f)] private float _pulseThreshold = 0.8f;
    [SerializeField, Min(0.01f)] private float _pulsePeriod = 1.4f;
    [SerializeField, Min(0f)] private float _hitFlashDuration = 0.18f;
    [SerializeField] private bool _animateBrightness = true;

    private Color _originalColor;
    private float _previousGauge;
    private float _flashRemaining;
    private float _pulseTime;
    private bool _wasRunning;
    private bool _hasOriginalColor;

    private void OnEnable()
    {
        if (_fill == null) return;
        _originalColor = _fill.color;
        _hasOriginalColor = true;
        _previousGauge = ReadGauge();
        _wasRunning = IsRunning;
        _flashRemaining = 0f;
        _pulseTime = 0f;
        Refresh(0f);
    }

    private void OnDisable()
    {
        if (_fill != null && _hasOriginalColor)
        {
            _fill.color = _originalColor;
            _fill.fillAmount = ReadGauge();
        }
        _hasOriginalColor = false;
        if (_fillTip != null) _fillTip.enabled = false;
    }

    private void LateUpdate() => Refresh(Time.unscaledDeltaTime);

    private bool IsRunning => _stagger != null && _stagger.isActiveAndEnabled && _stagger.IsRunning;

    private float ReadGauge() => _stagger != null && _stagger.isActiveAndEnabled
        ? Mathf.Clamp01(_stagger.NormalizedGauge)
        : 0f;

    private void Refresh(float deltaTime)
    {
        if (_fill == null) return;
        SetVisualState(ReadGauge(), IsRunning, deltaTime);
    }

    /// <summary>Updates UI only; does not alter the boss's gameplay gauge.</summary>
    public void SetVisualState(float gauge, bool running, float deltaTime)
    {
        if (_fill == null) return;
        gauge = Mathf.Clamp01(gauge);
        deltaTime = Mathf.Max(0f, deltaTime);
        // The gameplay gauge resets on entry; a full bright bar indicates the active opening.
        _fill.fillAmount = running ? 1f : gauge;
        if (gauge > _previousGauge + 0.001f || (running && !_wasRunning))
            _flashRemaining = _hitFlashDuration;

        Color color = running ? _staggerColor : _gaugeColor;
        if (_animateBrightness && (running || gauge >= _pulseThreshold))
        {
            _pulseTime = Mathf.Repeat(_pulseTime + deltaTime, Mathf.Max(0.01f, _pulsePeriod));
            float pulse = 0.5f - 0.5f * Mathf.Cos(_pulseTime / Mathf.Max(0.01f, _pulsePeriod) * Mathf.PI * 2f);
            color = Color.Lerp(color, running ? Color.white : _readyColor, pulse * (running ? 0.3f : 0.65f));
        }
        else
        {
            _pulseTime = 0f;
        }

        if (_animateBrightness && _hitFlashDuration > 0f)
            color = Color.Lerp(color, Color.white, Mathf.Clamp01(_flashRemaining / _hitFlashDuration) * 0.55f);

        _flashRemaining = Mathf.Max(0f, _flashRemaining - deltaTime);
        color.a = _hasOriginalColor ? _originalColor.a : 1f;
        _fill.color = color;
        UpdateFillTip(_fill.fillAmount);
        _previousGauge = gauge;
        _wasRunning = running;
    }

    private void UpdateFillTip(float amount)
    {
        if (_fillTip == null) return;
        float width = Mathf.Min(_tipWidth, _fill.rectTransform.rect.width * amount);
        _fillTip.enabled = width > 0f;
        _fillTip.color = _tipColor;
        var rect = _fillTip.rectTransform;
        rect.anchorMin = new Vector2(amount, 0f);
        rect.anchorMax = new Vector2(amount, 1f);
        rect.pivot = new Vector2(1f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(width, 0f);
    }
}
