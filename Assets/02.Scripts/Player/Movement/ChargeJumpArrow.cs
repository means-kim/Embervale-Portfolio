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

    private readonly Vector3[] _points = new Vector3[5];
    private LineRenderer _line;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
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
        Vector3 forward = direction.normalized;
        Vector3 side = new Vector3(-forward.y, forward.x, 0f);
        Vector3 tip = origin + forward * (_startOffset + _length);
        Vector3 back = tip - forward * Mathf.Min(_headLength, _length);
        _points[0] = origin + forward * _startOffset;
        _points[1] = tip;
        _points[2] = back + side * (_headWidth * 0.5f);
        _points[3] = tip;
        _points[4] = back - side * (_headWidth * 0.5f);
        _line.SetPositions(_points);
        _line.enabled = true;
    }

    public void Hide()
    {
        if (_line != null) _line.enabled = false;
    }
}
