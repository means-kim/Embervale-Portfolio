using UnityEngine;
using UnityEngine.UI;

public class OutGameDarkness : MonoBehaviour
{
    [Header("펄스 동기화")]
    [SerializeField] private OutGamePointerGlow _glow;
    [SerializeField] private bool _syncPulse = true;
    [SerializeField] private float _baseHoleRadius = 0.05f;

    [Header("타이틀 타원 구멍")]
    [Tooltip("끌어다 놓을 TitleImage")]
    [SerializeField] private RectTransform _title;
    [Tooltip("타이틀보다 얼마나 크게 뚫을지")]
    [SerializeField] private float _titlePadding = 1.2f;

    private Material _mat;
    private static readonly int HolePos = Shader.PropertyToID("_HolePos");
    private static readonly int Aspect = Shader.PropertyToID("_Aspect");
    private static readonly int HoleRadius = Shader.PropertyToID("_HoleRadius");
    private static readonly int Hole2Pos = Shader.PropertyToID("_Hole2Pos");
    private static readonly int Hole2Radius = Shader.PropertyToID("_Hole2Radius");

    private void Awake()
    {
        _mat = GetComponent<Image>().material;
    }

    private void Update()
    {
        if (_mat == null) return;
        float aspect = (float)Screen.width / Mathf.Max(1, Screen.height);
        float x = Input.mousePosition.x / Mathf.Max(1, Screen.width);
        float y = Input.mousePosition.y / Mathf.Max(1, Screen.height);
        _mat.SetVector(HolePos, new Vector4(x, y, 0, 0));
        _mat.SetFloat(Aspect, aspect);

        float factor = 1f;
        if (_syncPulse && _glow != null)
            factor = _glow.CurrentPulseFactor;
        _mat.SetFloat(HoleRadius, _baseHoleRadius * factor);

        if (_title != null)
        {
            Vector2 tsize = _title.rect.size;
            Vector3 tscale = _title.localScale;
            Vector2 tpos = _title.anchoredPosition;
            // Canvas가 가로 1920 기준 스케일이라는 전제 (현재 CanvasScaler 설정)
            float cx = 0.5f + tpos.x / 1920f;
            float cy = 0.5f + tpos.y * aspect / 1920f;
            float rx = (tsize.x * tscale.x * 0.5f * _titlePadding / 1920f) * aspect * factor;
            float ry = (tsize.y * tscale.y * 0.5f * _titlePadding * aspect / 1920f) * factor;
            _mat.SetVector(Hole2Pos, new Vector4(cx, cy, 0, 0));
            _mat.SetVector(Hole2Radius, new Vector4(rx, ry, 0, 0));
        }
    }
}