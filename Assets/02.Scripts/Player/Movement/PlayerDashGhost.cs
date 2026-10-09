using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대시 중 플레이어 스프라이트를 일정 간격으로 복사해 남기는 잔상 효과.
/// 고스트는 월드에 고정되며, 페이드 후 풀로 반환된다.
/// </summary>
[RequireComponent(typeof(PlayerDash))]
public class PlayerDashGhost : MonoBehaviour
{
    [Header("원본")]
    [Tooltip("비워 두면 같은 오브젝트의 SpriteRenderer를 자동 사용.")]
    [SerializeField] private SpriteRenderer _targetRenderer;

    [Header("잔상")]
    [SerializeField, Min(1)] private int _poolSize = 10;
    [Tooltip("대시 시작 후 첫 잔상이 나오기까지 대기 시간. 서있는 자세 복제를 피하려면 0.1 정도.")]
    [SerializeField, Min(0f)] private float _spawnDelay = 0.1f;
    [Tooltip("차지 점프 발사 후 첫 잔상 지연. 대시 자세가 반영된 뒤 복사합니다.")]
    [SerializeField, Min(0f)] private float _chargeJumpSpawnDelay = 0.02f;
    [SerializeField, Min(0.01f)] private float _spawnInterval = 0.04f;
    [SerializeField, Min(0.05f)] private float _fadeTime = 0.4f;
    [SerializeField, Range(0f, 1f)] private float _initialAlpha = 0.6f;
    [SerializeField] private Color _tint = Color.white;
    [SerializeField] private int _sortingOrderOffset = -1;

    private PlayerDash _playerDash;
    private Transform _holder;
    private readonly List<Ghost> _pool = new();
    private float _spawnTimer;
    private bool _spawning;
    private bool _chargeJumpSpawning;

    private sealed class Ghost
    {
        public GameObject Go;
        public SpriteRenderer Renderer;
        public float LifeRemaining;
    }

    private void Awake()
    {
        if (_targetRenderer == null)
            _targetRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();

        _playerDash = GetComponent<PlayerDash>();

        _holder = new GameObject($"{gameObject.name}_DashGhosts").transform;
        _holder.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        _holder.localScale = Vector3.one;

        for (int i = 0; i < _poolSize; i++)
        {
            var go = new GameObject($"Ghost_{i}");
            go.transform.SetParent(_holder);
            go.SetActive(false);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.enabled = false;

            _pool.Add(new Ghost { Go = go, Renderer = sr, LifeRemaining = 0f });
        }
    }

    private void OnEnable()
    {
        if (_playerDash != null)
        {
            _playerDash.OnDashStarted += HandleDashStarted;
            _playerDash.OnDashEnded += HandleDashEnded;
        }
    }

    private void OnDisable()
    {
        if (_playerDash != null)
        {
            _playerDash.OnDashStarted -= HandleDashStarted;
            _playerDash.OnDashEnded -= HandleDashEnded;
        }

        ClearGhosts();
    }

    // Animator가 현재 프레임의 스프라이트를 적용한 뒤 복사합니다.
    private void LateUpdate()
    {
        if (_spawning)
        {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                _spawnTimer += _spawnInterval;
                Spawn();
            }
        }

        float fadeSpeed = _fadeTime > 0f ? 1f / _fadeTime : float.MaxValue;
        foreach (Ghost ghost in _pool)
        {
            if (ghost == null || ghost.Go == null || ghost.Renderer == null) continue;
            if (!ghost.Go.activeSelf) continue;

            ghost.LifeRemaining -= Time.deltaTime;
            if (ghost.LifeRemaining <= 0f)
            {
                ghost.Go.SetActive(false);
                ghost.Renderer.enabled = false;
                continue;
            }

            Color color = ghost.Renderer.color;
            color.a = _initialAlpha * Mathf.Clamp01(ghost.LifeRemaining * fadeSpeed);
            ghost.Renderer.color = color;
        }
    }

    private void HandleDashStarted()
    {
        if (_targetRenderer == null)
            _targetRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();

        _spawning = true;
        _spawnTimer = _spawnDelay;
        if (_spawnTimer <= 0f)
        {
            _spawnTimer = _spawnInterval;
            Spawn();
        }
    }

    private void HandleDashEnded()
    {
        // 스폰만 중단하고, 이미 나온 잔상은 자연 페이드.
        _spawning = _chargeJumpSpawning;
    }

    public void BeginChargeJumpLaunch()
    {
        if (!isActiveAndEnabled) return;
        if (_targetRenderer == null)
            _targetRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        _chargeJumpSpawning = true;
        _spawning = true;
        _spawnTimer = _chargeJumpSpawnDelay;
    }

    public void EndChargeJumpLaunch()
    {
        _chargeJumpSpawning = false;
        _spawning = _playerDash != null && _playerDash.IsDashing;
    }

    private void Spawn()
    {
        if (_targetRenderer == null || _targetRenderer.sprite == null) return;

        foreach (Ghost ghost in _pool)
        {
            if (ghost == null || ghost.Go == null || ghost.Renderer == null) continue;
            if (ghost.Go.activeSelf) continue;

            ghost.Renderer.sharedMaterial = _targetRenderer.sharedMaterial;
            ghost.Renderer.sprite = _targetRenderer.sprite;
            ghost.Renderer.flipX = _targetRenderer.flipX;
            ghost.Renderer.flipY = _targetRenderer.flipY;
            ghost.Renderer.sortingLayerID = _targetRenderer.sortingLayerID;
            ghost.Renderer.sortingOrder = _targetRenderer.sortingOrder + _sortingOrderOffset;

            Color color = _tint;
            color.a = _initialAlpha;
            ghost.Renderer.color = color;

            ghost.Go.layer = _targetRenderer.gameObject.layer;
            ghost.Go.transform.SetPositionAndRotation(_targetRenderer.transform.position, _targetRenderer.transform.rotation);
            ghost.Go.transform.localScale = _targetRenderer.transform.lossyScale;

            ghost.LifeRemaining = _fadeTime;
            ghost.Renderer.enabled = true;
            ghost.Go.SetActive(true);
            return;
        }
    }

    /// <summary>리스폰/텔레포트 시 남은 잔상을 즉시 지운다.</summary>
    public void ClearGhosts()
    {
        _spawning = false;
        _chargeJumpSpawning = false;
        foreach (Ghost ghost in _pool)
        {
            if (ghost == null || ghost.Go == null || ghost.Renderer == null) continue;
            ghost.LifeRemaining = 0f;
            ghost.Renderer.enabled = false;
            ghost.Go.SetActive(false);
        }
    }

    public void ResetForRespawn() => ClearGhosts();
    private void OnDestroy()
    {
        if (_holder != null) Destroy(_holder.gameObject);
    }
}
