using System.Collections.Generic;
using UnityEngine;

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
    private PlayerAnimator _playerAnimator;
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
        {
            _targetRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        }

        PlayerRig rig = PlayerRig.Resolve(gameObject);
        _playerDash = rig.Dash;
        _playerAnimator = rig.PlayerAnimator;

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
        _playerDash.OnDashStarted += HandleDashStarted;
        _playerDash.OnDashEnded += HandleDashEnded;
    }

    private void OnDisable()
    {
        _playerDash.OnDashStarted -= HandleDashStarted;
        _playerDash.OnDashEnded -= HandleDashEnded;

        ClearGhosts();
    }

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

        float fadeSpeed = float.MaxValue;
        if (_fadeTime > 0f)
        {
            fadeSpeed = 1f / _fadeTime;
        }
        foreach (Ghost ghost in _pool)
        {
            if (ghost == null || ghost.Go == null || ghost.Renderer == null)
            {
                continue;
            }
            if (!ghost.Go.activeSelf)
            {
                continue;
            }

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
        {
            _targetRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        }

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
        _spawning = _chargeJumpSpawning;
    }

    public void BeginChargeJumpLaunch()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        if (_targetRenderer == null)
        {
            _targetRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        }
        _chargeJumpSpawning = true;
        _spawning = true;
        _spawnTimer = _chargeJumpSpawnDelay;
    }

    public void EndChargeJumpLaunch()
    {
        _chargeJumpSpawning = false;
        _spawning = _playerDash.IsDashing;
    }

    private void Spawn()
    {
        if (_targetRenderer == null || _targetRenderer.sprite == null)
        {
            return;
        }
        SpriteRenderer renderer = _playerAnimator.GetChargeJumpVisualRenderer(_targetRenderer);

        foreach (Ghost ghost in _pool)
        {
            if (ghost == null || ghost.Go == null || ghost.Renderer == null)
            {
                continue;
            }
            if (ghost.Go.activeSelf)
            {
                continue;
            }

            ghost.Renderer.sharedMaterial = renderer.sharedMaterial;
            ghost.Renderer.sprite = renderer.sprite;
            ghost.Renderer.flipX = renderer.flipX;
            ghost.Renderer.flipY = renderer.flipY;
            ghost.Renderer.sortingLayerID = renderer.sortingLayerID;
            ghost.Renderer.sortingOrder = renderer.sortingOrder + _sortingOrderOffset;

            Color color = _tint;
            color.a = _initialAlpha;
            ghost.Renderer.color = color;

            ghost.Go.layer = renderer.gameObject.layer;
            Vector3 right = renderer.transform.TransformVector(Vector3.right);
            Vector3 up = renderer.transform.TransformVector(Vector3.up);
            float angle = Mathf.Atan2(right.y, right.x) * Mathf.Rad2Deg;
            float handedness = 1f;
            if (Vector3.Cross(right, up).z < 0f)
            {
                handedness = -1f;
            }
            ghost.Go.transform.SetPositionAndRotation(renderer.transform.position, Quaternion.Euler(0f, 0f, angle));
            ghost.Go.transform.localScale = new Vector3(right.magnitude, up.magnitude * handedness, 1f);

            ghost.LifeRemaining = _fadeTime;
            ghost.Renderer.enabled = true;
            ghost.Go.SetActive(true);
            return;
        }
    }

    public void ClearGhosts()
    {
        _spawning = false;
        _chargeJumpSpawning = false;
        foreach (Ghost ghost in _pool)
        {
            if (ghost == null || ghost.Go == null || ghost.Renderer == null)
            {
                continue;
            }
            ghost.LifeRemaining = 0f;
            ghost.Renderer.enabled = false;
            ghost.Go.SetActive(false);
        }
    }

    public void ResetForRespawn() => ClearGhosts();
    private void OnDestroy()
    {
        if (_holder != null)
        {
            Destroy(_holder.gameObject);
        }
    }
}
