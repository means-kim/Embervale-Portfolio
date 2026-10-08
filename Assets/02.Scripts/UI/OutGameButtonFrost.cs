using UnityEngine;
using UnityEngine.EventSystems;

public class OutGameButtonFrost : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("조각 프리팹")]
    [Tooltip("끌어다 놓을 OutGame_FrostPiece 프리팹")]
    [SerializeField] private OutGameFrostPiece _piecePrefab;

    [Header("컨테이너 (비워 두면 ButtonFrostContainer 자동 탐색)")]
    [SerializeField] private RectTransform _container;

    [Header("방출 설정 (캔버스 픽셀 단위)")]
    [Tooltip("조각 생성 간격(초)")]
    [SerializeField] private float _spawnInterval = 0.12f;
    [Tooltip("이동 속도(px/s). 90 ≈ 기존 0.8유닛/s")]
    [SerializeField] private float _speed = 90f;
    [Tooltip("속도 랜덤 비율")]
    [SerializeField] private float _speedRandom = 0.2f;
    [Tooltip("조각 수명(초)")]
    [SerializeField] private float _lifetime = 2.4f;
    [Tooltip("조각 기본 크기. 10 ≈ 기존 1.2유닛")]
    [SerializeField] private float _scale = 10f;
    [Tooltip("크기 랜덤 비율")]
    [SerializeField] private float _scaleRandom = 0.25f;

    private Camera _cam;
    private bool _isHovered;
    private float _timer;

    private void Awake()
    {
        _cam = GetComponentInParent<Canvas>()?.worldCamera;
        if (_cam == null) _cam = Camera.main;
        if (_container == null)
        {
            GameObject found = GameObject.Find("ButtonFrostContainer");
            if (found != null) _container = found.GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        if (!_isHovered || _piecePrefab == null || _container == null) return;

        _timer += Time.deltaTime;
        while (_timer >= _spawnInterval)
        {
            _timer -= _spawnInterval;
            SpawnPiece();
        }
    }

    private void OnDisable()
    {
        _isHovered = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;
        _timer = _spawnInterval; // 첫 조각 즉시 생성
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 기존 조각은 스스로 페이드아웃 후 소멸하므로 생성만 중단
        _isHovered = false;
    }

    private void SpawnPiece()
    {
        // 마우스 포인터의 캔버스 좌표 = 생성 위치
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _container, Input.mousePosition, _cam, out Vector2 local))
        {
            return;
        }

        OutGameFrostPiece piece = Instantiate(_piecePrefab, _container);
        piece.transform.localPosition = new Vector3(local.x, local.y, 0f);

        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
        float speed = _speed * (1f + Random.Range(-_speedRandom, _speedRandom));
        float life = _lifetime * Random.Range(0.85f, 1.15f);
        float scale = _scale * (1f + Random.Range(-_scaleRandom, _scaleRandom));
        piece.Setup(dir, speed, life, scale);
    }
}
