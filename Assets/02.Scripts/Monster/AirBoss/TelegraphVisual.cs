using UnityEngine;

[DisallowMultipleComponent]
public sealed class TelegraphVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _visual;
    [SerializeField, Min(0.01f)] private float _duration = 1f;
    [SerializeField, Range(0f, 1f)] private float _minimumBrightness = 0.65f;
    [SerializeField, Range(0f, 1f)] private float _maximumBrightness = 1f;
    [SerializeField, Range(0f, 1f)] private float _minimumAlpha = 0.45f;
    [SerializeField, Range(0f, 1f)] private float _maximumAlpha = 0.9f;

    [Tooltip("Off: brighten once and hold until hidden. On: smoothly pulse repeatedly.")]
    [SerializeField] private bool _loop;

    private float _elapsed;

    private void OnEnable()
    {
        if (_visual == null) _visual = GetComponentInChildren<SpriteRenderer>(true);
        Restart();
    }

    public void Restart()
    {
        _elapsed = 0f;
        Apply(0f);
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;
        float phase = _elapsed / Mathf.Max(0.01f, _duration);
        float progress = _loop
            ? (1f - Mathf.Cos(phase * Mathf.PI)) * 0.5f
            : Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(phase));
        Apply(progress);
    }

    private void OnDisable()
    {
        Restart();
    }

    private void Apply(float progress)
    {
        if (_visual == null) return;
        float brightness = Mathf.Lerp(_minimumBrightness, _maximumBrightness, progress);
        _visual.color = new Color(brightness, brightness, brightness,
            Mathf.Lerp(_minimumAlpha, _maximumAlpha, progress));
    }
}