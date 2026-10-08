using UnityEngine;
using UnityEngine.UI;

public class OutGamePointerGlow : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private Canvas _canvas;

    [Header("밝기 / 크기")]
    [Range(0f, 2f)][SerializeField] private float _brightness = 0.9f;
    [SerializeField] private float _baseSize = 256f;

    [Header("펄스 (서서히 커졌다 작아지기)")]
    [SerializeField] private bool _pulseEnabled = true;
    [Tooltip("초당 반복 횟수. 0.5 = 2초에 한 번")]
    [SerializeField] private float _pulseSpeed = 0.5f;
    [Range(0f, 0.5f)][Tooltip("변동 폭. 0.15 = ±15%")][SerializeField] private float _pulseAmount = 0.15f;

    private RectTransform _rt;
    private RectTransform _canvasRt;
    private Material _mat;
    private Color _baseColor;
    private float _phase;
    public float CurrentPulseFactor { get; private set; } = 1f;
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        _rt = (RectTransform)transform;
        if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
        _canvasRt = (RectTransform)_canvas.transform;
        // Image.material 접근 시 자동 복제되므로 에셋 원본이 더렵혀지지 않음
        _mat = GetComponent<Image>().material;
        _baseColor = _mat.GetColor(ColorId);
        _rt.sizeDelta = new Vector2(_baseSize, _baseSize);
    }

    private void Update()
    {
        // 위치 추적 (ScreenSpace-Camera 기준)
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRt, Input.mousePosition, _canvas.worldCamera, out local);
        _rt.anchoredPosition = local;

        // 펄스 계수
        float factor = 1f;
        if (_pulseEnabled)
        {
            _phase += Time.unscaledDeltaTime * _pulseSpeed * Mathf.PI * 2f;
            factor = 1f + Mathf.Sin(_phase) * _pulseAmount;
        }
        CurrentPulseFactor = factor; // 추가

        // 크기 반영
        float size = _baseSize * factor;
        _rt.sizeDelta = new Vector2(size, size);

        // 밝기 반영 (기본색 × 슬라이더 × 펄스)
        _mat.SetColor(ColorId, _baseColor * (_brightness * factor));
    }
}