using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(LineRenderer))]
public class ChargeJumpArrow : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float _length = 1.6f;
    [SerializeField, Min(0f)] private float _startOffset = 0.25f;
    [SerializeField, Min(0.01f)] private float _headLength = 0.4f;
    [SerializeField, Min(0.01f)] private float _headWidth = 0.3f;
    [SerializeField, Min(0.01f)] private float _lineWidth = 0.08f;
    [SerializeField] private Color _color = new Color(0.55f, 0.95f, 1f, 1f);
    [Header("이미지 화살표 (없으면 기존 선 사용)")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [Header("등장 연출 · 실제 시간 기준")]
    [SerializeField, Range(0.01f, 1f)] private float _startScale = 0.25f;
    [SerializeField, Min(1f)] private float _peakScale = 1.3f;
    [SerializeField, Min(0.01f)] private float _growDuration = 0.1f;
    [SerializeField, Min(0.01f)] private float _settleDuration = 0.12f;

    private readonly Vector3[] _points = new Vector3[5];
    private LineRenderer _line;
    private bool _visible;
    private float _shownAt;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
        if (_spriteRenderer == null) _spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        _line.useWorldSpace = true;
        _line.loop = false;
        _line.positionCount = _points.Length;
        _line.widthCurve = AnimationCurve.Constant(0f, 1f, 1f);
        _line.widthMultiplier = _lineWidth;
        _line.startColor = _line.endColor = _color;
        _line.numCapVertices = 4;
        Hide();
    }

    public void Show(Vector3 origin, Vector2 direction)
    {
        if (!_visible)
        {
            _visible = true;
            _shownAt = Time.unscaledTime;
        }
        float elapsed = Time.unscaledTime - _shownAt;
        float popScale = elapsed < _growDuration
            ? Mathf.Lerp(_startScale, _peakScale, Mathf.SmoothStep(0f, 1f, elapsed / _growDuration))
            : Mathf.Lerp(_peakScale, 1f, Mathf.SmoothStep(0f, 1f, (elapsed - _growDuration) / _settleDuration));
        Vector3 forward = direction.normalized;
        Vector3 side = new Vector3(-forward.y, forward.x, 0f);
        if (_spriteRenderer != null && _spriteRenderer.sprite != null)
        {
            _line.enabled = false;
            _spriteRenderer.enabled = true;
            Transform visual = _spriteRenderer.transform;
            float spriteWidth = Mathf.Max(0.001f, _spriteRenderer.sprite.bounds.size.x);
            float scale = _length * popScale / spriteWidth;
            visual.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
            visual.localScale = Vector3.one * scale;
            visual.position = origin + forward * (_startOffset + _length * popScale * 0.5f);
            return;
        }
        if (_spriteRenderer != null) _spriteRenderer.enabled = false;
        Vector3 tip = origin + forward * (_startOffset + _length * popScale);
        Vector3 back = tip - forward * Mathf.Min(_headLength, _length) * popScale;
        _points[0] = origin + forward * _startOffset;
        _points[1] = tip;
        _points[2] = back + side * (_headWidth * 0.5f * popScale);
        _points[3] = tip;
        _points[4] = back - side * (_headWidth * 0.5f * popScale);
        _line.widthMultiplier = _lineWidth * popScale;
        _line.SetPositions(_points);
        _line.enabled = true;
    }

    public void Hide()
    {
        _visible = false;
        if (_line != null) _line.enabled = false;
        if (_spriteRenderer != null) _spriteRenderer.enabled = false;
    }
}
