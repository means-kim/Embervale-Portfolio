using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public sealed class BossPhase3Camera : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private Transform _player;
    [SerializeField] private Collider2D _playerCollider;
    [SerializeField] private BossPhase3Cinematic _phase3;
    [SerializeField] private PolygonCollider2D _upperArea;
    [SerializeField] private float _zoomStartY = 161f;
    [SerializeField] private float _zoomEndY = 245f;
    [SerializeField, Min(1f)] private float _upperSize = 12f;
    [SerializeField, Min(0.01f)] private float _zoomSmoothTime = 0.35f;
    [SerializeField, Min(0f)] private float _upperLookAheadY = 8f;
    [SerializeField, Min(0f)] private float _boundaryBlendTime = 0.8f;
    [Header("3페이즈 상단 도착 시 활성화")]
    [Tooltip("상단 카메라로 전환되면 켜지고, 낙하 중에도 유지됩니다. 페이즈 초기화 시 꺼집니다.")]
    [SerializeField] private GameObject[] _upperArrivalObjects = new GameObject[0];
    private bool _upperArrivalActivated;

    private CinemachineConfiner2D _confiner;
    private Collider2D _originalArea;
    private float _originalSize;
    private float _zoomVelocity;
    private PolygonCollider2D _travelArea;
    private bool _initialized;
    private bool _inUpperArea;
    private CinemachinePositionComposer _composer;
    private Vector3 _originalOffset;
    private ConfinerBoundaryBlend _boundaryBlend;

    private void Awake()
    {
        SetArrivalObjects(false);
        if (_camera == null || _player == null || _phase3 == null || _upperArea == null)
        {
            return;
        }

        _confiner = _camera.GetComponent<CinemachineConfiner2D>();
        if (_confiner == null)
        {
            return;
        }

        _originalArea = _confiner.BoundingShape2D;
        _boundaryBlend = _camera.GetComponent<ConfinerBoundaryBlend>();
        if (_boundaryBlend == null) _boundaryBlend = _camera.gameObject.AddComponent<ConfinerBoundaryBlend>();
        _originalSize = _camera.Lens.OrthographicSize;
        _composer = _camera.GetComponent<CinemachinePositionComposer>();
        if (_composer != null)
        {
            _originalOffset = _composer.TargetOffset;
        }

        _initialized = true;
    }

    private void LateUpdate()
    {
        if (!_initialized)
        {
            return;
        }

        bool phase3 = _phase3 != null && (_phase3.IsPlaying || _phase3.IsPhase3Active);
        float feetY;
        if (_playerCollider != null)
        {
            feetY = _playerCollider.bounds.min.y;
        }
        else
        {
            feetY = _player.position.y;
        }

        float t = phase3 ? Mathf.InverseLerp(_zoomStartY, Mathf.Max(_zoomStartY + 1f, _zoomEndY), feetY) : 0f;
        t = Mathf.SmoothStep(0f, 1f, t);
        if (_composer != null)
        {
            _composer.TargetOffset = _originalOffset + Vector3.up * (_upperLookAheadY * t);
        }

        float aspect;

        if (Camera.main != null)
        {
            aspect = Camera.main.aspect;
        }
        else
        {
            aspect = 16f / 9f;
        }

        Bounds bounds = _upperArea.bounds;
        float fitSize = Mathf.Min(bounds.size.y * 0.5f, bounds.size.x / (2f * Mathf.Max(0.1f, aspect))) * 0.9f;
        float upperSize = Mathf.Min(_originalSize, Mathf.Max(1f, Mathf.Min(_upperSize, fitSize)));
        var lens = _camera.Lens;
        lens.OrthographicSize = Mathf.SmoothDamp(lens.OrthographicSize, Mathf.Lerp(_originalSize, upperSize, t),
            ref _zoomVelocity, _zoomSmoothTime);
        if (Mathf.Abs(lens.OrthographicSize - _camera.Lens.OrthographicSize) > 0.0001f)
        {
            _camera.Lens = lens;
            _confiner.InvalidateLensCache();
        }

        if (!phase3)
        {
            _inUpperArea = false;
        }
        else if (feetY >= _zoomEndY - 0.1f)
        {
            _inUpperArea = true;
        }
        else if (feetY < _zoomEndY - 2f)
        {
            _inUpperArea = false;
        }

        if (!phase3)
        {
            SetArea(_originalArea);
            if (_upperArrivalActivated) SetArrivalObjects(false);
        }
        else if (_inUpperArea)
        {
            SetArea(_upperArea);
            if (!_upperArrivalActivated) SetArrivalObjects(true);
        }
        else
        {
            if (_travelArea == null)
            {
                BuildTravelArea();
            }

            SetArea(_travelArea != null ? _travelArea : _originalArea);
        }
    }

    private void SetArrivalObjects(bool active)
    {
        _upperArrivalActivated = active;
        foreach (var target in _upperArrivalObjects)
        {
            if (target != null) target.SetActive(active);
        }
    }

    private void SetArea(Collider2D area)
    {
        if (_confiner.BoundingShape2D == area) return;
        _boundaryBlend?.BeginTransition(_boundaryBlendTime);
        _confiner.BoundingShape2D = area;
        _confiner.InvalidateBoundingShapeCache();
    }

    private static void AddPoints(PolygonCollider2D polygon, List<Vector2> points)
    {
        for (int i = 0; i < polygon.pathCount; i++)
        {
            foreach (var point in polygon.GetPath(i))
            {
                points.Add(polygon.transform.TransformPoint(point + polygon.offset));
            }
        }
    }

    private static float Cross(Vector2 a, Vector2 b, Vector2 c) =>
        (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);

    private void BuildTravelArea()
    {
        if (!(_originalArea is PolygonCollider2D original))
        {
            return;
        }

        var points = new List<Vector2>();
        AddPoints(original, points);
        AddPoints(_upperArea, points);
        points.Sort((a, b) => a.x != b.x ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));
        var hull = new List<Vector2>();
        foreach (var point in points)
        {
            while (hull.Count >= 2 && Cross(hull[hull.Count - 2], hull[hull.Count - 1], point) <= 0f)
            {
                hull.RemoveAt(hull.Count - 1);
            }

            hull.Add(point);
        }

        int lowerCount = hull.Count;
        for (int i = points.Count - 2; i >= 0; i--)
        {
            while (hull.Count > lowerCount && Cross(hull[hull.Count - 2], hull[hull.Count - 1], points[i]) <= 0f)
            {
                hull.RemoveAt(hull.Count - 1);
            }

            hull.Add(points[i]);
        }

        if (hull.Count > 1)
        {
            hull.RemoveAt(hull.Count - 1);
        }

        if (hull.Count < 3)
        {
            return;
        }

        var go = new GameObject("Phase3_CameraTravelBounds_Runtime");
        _travelArea = go.AddComponent<PolygonCollider2D>();
        _travelArea.enabled = false;
        _travelArea.isTrigger = true;
        _travelArea.SetPath(0, hull.ToArray());
    }

    private void OnDisable()
    {
        SetArrivalObjects(false);
        if (_initialized && _camera != null && _confiner != null)
        {
            var lens = _camera.Lens;
            lens.OrthographicSize = _originalSize;
            _camera.Lens = lens;
            _zoomVelocity = 0f;
            if (_composer != null)
            {
                _composer.TargetOffset = _originalOffset;
            }

            SetArea(_originalArea);
            _boundaryBlend?.ResetBlend();
            _confiner.InvalidateLensCache();
        }

        if (_travelArea != null)
        {
            Destroy(_travelArea.gameObject);
        }
    }
}
