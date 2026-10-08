using UnityEngine;
using UnityEngine.UI;

public class OutGameTitleGlow : MonoBehaviour
{
    [Header("연결")]
    [Tooltip("끌어다 놓을 TitleImage")]
    [SerializeField] private RectTransform _title;

    [Header("밝기 / 크기")]
    [Range(0f, 2f)][SerializeField] private float _brightness = 0.6f;
    [SerializeField] private float _baseWidth = 1400f;
    [SerializeField] private float _baseHeight = 550f;

    [Header("펄스")]
    [SerializeField] private bool _pulseEnabled = true;
    [Tooltip("초당 반복 횟수. 0.35 = 약 3초에 한 번")]
    [SerializeField] private float _pulseSpeed = 0.35f;
    [Range(0f, 0.5f)][SerializeField] private float _pulseAmount = 0.12f;

    private RectTransform _rt;
    private Material _mat;
    private Color _baseColor;
    private float _phase;
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        _rt = (RectTransform)transform;
        if (_title == null)
            _title = GameObject.Find("TitleImage")?.GetComponent<RectTransform>();
        _mat = GetComponent<Image>().material;
        _baseColor = _mat.GetColor(ColorId);
        SyncToTitle(1f);
    }

    private void Update()
    {
        float factor = 1f;
        if (_pulseEnabled)
        {
            _phase += Time.unscaledDeltaTime * _pulseSpeed * Mathf.PI * 2f;
            factor = 1f + Mathf.Sin(_phase) * _pulseAmount;
        }
        SyncToTitle(factor);
        _mat.SetColor(ColorId, _baseColor * (_brightness * factor));
    }

    private void SyncToTitle(float factor)
    {
        if (_title != null)
            _rt.anchoredPosition = _title.anchoredPosition;
        _rt.sizeDelta = new Vector2(_baseWidth * factor, _baseHeight * factor);
    }
}