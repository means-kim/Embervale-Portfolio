using UnityEngine;
using UnityEngine.UI;

public class OutGameDarkness : MonoBehaviour
{
    [Header("펄스 동기화")]
    [Tooltip("끌어다 놓을 PointerGlow 오브젝트")]
    [SerializeField] private OutGamePointerGlow _glow;
    [Tooltip("체크 해제하면 구멍은 펄스 무시하고 고정")]
    [SerializeField] private bool _syncPulse = true;
    [Tooltip("구멍 기본 반경. 머티리얼 _HoleRadius와 같은 값으로 시작")]
    [SerializeField] private float _baseHoleRadius = 0.15f;

    private Material _mat;
    private static readonly int HolePos = Shader.PropertyToID("_HolePos");
    private static readonly int Aspect = Shader.PropertyToID("_Aspect");
    private static readonly int HoleRadius = Shader.PropertyToID("_HoleRadius");

    private void Awake()
    {
        _mat = GetComponent<Image>().material;
    }

    private void Update()
    {
        if (_mat == null) return;
        float x = Input.mousePosition.x / Mathf.Max(1, Screen.width);
        float y = Input.mousePosition.y / Mathf.Max(1, Screen.height);
        _mat.SetVector(HolePos, new Vector4(x, y, 0, 0));
        _mat.SetFloat(Aspect, (float)Screen.width / Mathf.Max(1, Screen.height));

        // 글로우 펄스와 동일한 배율로 구멍 반경 조절
        float factor = 1f;
        if (_syncPulse && _glow != null)
            factor = _glow.CurrentPulseFactor;
        _mat.SetFloat(HoleRadius, _baseHoleRadius * factor);
    }
}