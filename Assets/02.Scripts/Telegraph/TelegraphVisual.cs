using UnityEngine;
using System.Collections;

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

    [SerializeField, Min(0.01f)] private float _fadeOutDuration = 0.2f;

    private bool _isFadingOut;

    private void OnEnable()
    {
        if (_visual == null) _visual = GetComponentInChildren<SpriteRenderer>(true);
        Restart();
    }

    public void Restart()
    {
        _isFadingOut = false;
        _elapsed = 0f;
        Apply(0f);
    }

    private void Update()
    {
        if (_isFadingOut) return;

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

    public IEnumerator FadeOut()
    {
        _isFadingOut = true;

        if (_visual == null) yield break;

        Color startColor = _visual.color;
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, _fadeOutDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(
                0f, 1f, Mathf.Clamp01(elapsed / duration));

            Color color = startColor;
            color.a = Mathf.Lerp(startColor.a, 0f, progress);
            _visual.color = color;

            yield return null;
        }
    }
}