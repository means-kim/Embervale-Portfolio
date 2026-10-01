using System;
using UnityEngine;

public enum PlayerAttackType
{
    SideAttack,
    Stab,
    Horizontal,
    Down,
    Up,
}

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerMove))]
[RequireComponent(typeof(PlayerDash))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField, Min(0)] private int _damage = 10;
    [Header("타입별 판정 (씬뷰에서 5개 박스로 확인, 좌우 반전 포함)")]
    [SerializeField] private PlayerAttackHitbox _side = new() { Size = new Vector2(1.4f, 1.2f), Offset = new Vector2(0.8f, 0.8f), VfxOffset = new Vector2(0.8f, 0.8f), Duration = 0.38f };
    [SerializeField] private PlayerAttackHitbox _stab = new() { Size = new Vector2(2.2f, 0.8f), Offset = new Vector2(1.1f, 0.8f), VfxOffset = new Vector2(1.1f, 0.8f), Duration = 0.67f };
    [SerializeField] private PlayerAttackHitbox _horizontal = new() { Size = new Vector2(1.4f, 1.2f), Offset = new Vector2(0.8f, 0.8f), VfxOffset = new Vector2(0.8f, 0.8f), Duration = 1.17f };
    [SerializeField] private PlayerAttackHitbox _down = new() { Size = new Vector2(1.2f, 1.6f), Offset = new Vector2(0.2f, -1.0f), VfxOffset = new Vector2(0.2f, -1.0f), Duration = 1.17f };
    [SerializeField] private PlayerAttackHitbox _up = new() { Size = new Vector2(1.2f, 1.8f), Offset = new Vector2(0.2f, 1.2f), VfxOffset = new Vector2(0.2f, 1.2f), Duration = 1.17f };
    [Header("Stab 투사체 (지정 시 Stab 데미지는 투사체가 전담. 비어 있으면 즉발 판정)")]
    [SerializeField] private GameObject _stabProjectilePrefab;
    [Header("디버그 (플레이 중 발동 판정 표시)")]
    [SerializeField] private bool _showHitboxes;
    private PlayerAttackType _debugLastType;
    private Vector2 _debugLastCenter;
    private Vector2 _debugLastSize;
    private float _debugBoxUntil;
    private readonly Collider2D[] _hitBuffer = new Collider2D[16];
    private PlayerInput _playerInput;
    private PlayerMove _playerMove;
    private PlayerDash _playerDash;
    public event Action OnAttackStarted;
    public event Action OnAttackEnded;
    public bool IsAttacking { get; private set; }
    public string CurrentAttackClip { get; private set; } = "SideAttack";
    private Rigidbody2D _rigidbody;
    private float _attackTimeRemaining;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _playerMove = GetComponent<PlayerMove>();
        _playerDash = GetComponent<PlayerDash>();
    }
    private void OnEnable()
    {
        _playerInput.OnAttackInput += HandleAttack;
        _playerInput.OnAttackTypeInput += HandleTypeAttack;
    }
    private void OnDisable()
    {
        _playerInput.OnAttackInput -= HandleAttack;
        _playerInput.OnAttackTypeInput -= HandleTypeAttack;
        EndAttack();
    }
    private void Update()
    {
        if (!IsAttacking) return;
        _attackTimeRemaining -= Time.deltaTime;
        if (_attackTimeRemaining <= 0f) EndAttack();
    }
    private void HandleAttack()
    {
        if (IsAttacking || _playerDash.IsDashing) return;
        var direction = _playerMove.FacingDirection;
        _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
        IsAttacking = true;
        CurrentAttackClip = "SideAttack";
        _attackTimeRemaining = _side.Duration;
        DoHit(direction, _side, PlayerAttackType.SideAttack);
        OnAttackStarted?.Invoke();
    }
    private void HandleTypeAttack(PlayerAttackType type)
    {
        if (IsAttacking || _playerDash.IsDashing) return;
        var direction = _playerMove.FacingDirection;
        _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
        IsAttacking = true;
        CurrentAttackClip = type == PlayerAttackType.SideAttack ? "SideAttack" : type + "Attack";
        PlayerAttackHitbox box = GetHitbox(type);
        _attackTimeRemaining = box.Duration;
        if (type == PlayerAttackType.Stab && _stabProjectilePrefab != null)
        {
            FireStab(direction, box);
        }
        else
        {
            if (type == PlayerAttackType.Stab)
                Debug.LogWarning("Stab 투사체 프리팹 미지정: 즉발 판정으로 대체한다.", this);
            DoHit(direction, box, type);
        }
        OnAttackStarted?.Invoke();
    }
    /// <summary>
    /// Stab 전용. 즉발 박스 대신 투사체를 날린다. 애니·발동락·머즐 VFX 흐름은 그대로 쓴다.
    /// _stab의 Duration(공격 경직)과 VfxOffset(발사 위치)은 그대로 사용하고 Size/Offset 판정은 쓰지 않는다.
    /// </summary>
    private void FireStab(int direction, PlayerAttackHitbox box)
    {
        var spawnPos = (Vector2)transform.position + new Vector2(direction * box.VfxOffset.x, box.VfxOffset.y);
        PlayAttackVfx(PlayerAttackType.Stab, spawnPos, direction);
        var spawned = Instantiate(_stabProjectilePrefab, spawnPos, Quaternion.identity);
        var projectile = spawned.GetComponent<StabProjectile>();
        if (projectile == null)
        {
            Debug.LogError("Stab 투사체 프리팹에 StabProjectile이 없음. 즉발 판정으로 대체한다.", spawned);
            Destroy(spawned);
            DoHit(direction, box, PlayerAttackType.Stab);
            return;
        }
        projectile.Launch(_damage, direction);
        if (_showHitboxes)
        {
            // 디버그는 박스 대신 투사체 비행 경로로 표시한다.
            _debugLastType = PlayerAttackType.Stab;
            _debugLastCenter = spawnPos + new Vector2(direction * projectile.MaxDistance * 0.5f, 0f);
            _debugLastSize = new Vector2(projectile.MaxDistance, box.Size.y);
            _debugBoxUntil = Time.time + Mathf.Max(box.Duration, 0.05f);
        }
    }
    /// <summary>씬뷰 에디터와 런타임이 함께 쓰는 타입별 판정 조회.</summary>
    public PlayerAttackHitbox GetHitbox(PlayerAttackType type)
    {
        switch (type)
        {
            case PlayerAttackType.Stab: return _stab;
            case PlayerAttackType.Horizontal: return _horizontal;
            case PlayerAttackType.Down: return _down;
            case PlayerAttackType.Up: return _up;
            default: return _side;
        }
    }
    public bool DebugDrawEnabled => _showHitboxes;
    /// <summary>플레이 중 마지막 발동 판정. _showHitboxes가 켜져 있을 때만 유효.</summary>
    public bool TryGetDebugBox(out PlayerAttackType type, out Vector2 center, out Vector2 size)
    {
        type = _debugLastType;
        center = _debugLastCenter;
        size = _debugLastSize;
        return _showHitboxes && Time.time < _debugBoxUntil;
    }
    private void DoHit(int direction, PlayerAttackHitbox box, PlayerAttackType type)
    {
        var center = (Vector2)transform.position + new Vector2(direction * box.Offset.x, box.Offset.y);
        if (_showHitboxes)
        {
            _debugLastType = type;
            _debugLastCenter = center;
            _debugLastSize = box.Size;
            _debugBoxUntil = Time.time + Mathf.Max(box.Duration, 0.05f);
        }
        // VFX는 판정 중심이 아니라 전용 오프셋에서 소환한다. 판정을 옮겨도 이펙트 위치는 유지.
        var vfxPos = (Vector2)transform.position + new Vector2(direction * box.VfxOffset.x, box.VfxOffset.y);
        PlayAttackVfx(type, vfxPos, direction);
        var count = Physics2D.OverlapBox(center, box.Size, 0f, ContactFilter2D.noFilter, _hitBuffer);
        for (var i = 0; i < count; i++)
        {
            var hit = _hitBuffer[i];
            if (hit == null || !hit.CompareTag("Enemy")) continue;
            var health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(_damage);
                EffectManager.Instance?.PlayEnemyHit(hit.transform.position);
                // 사망 이펙트는 각 적의 사망 처리(Dummy/EliteMonsterDeath)가 담당한다. 여기서 중복 재생하지 않는다.
            }
            Debug.Log($"Attack hit {hit.name} for {_damage} damage.", hit);
            _hitBuffer[i] = null;
        }
    }
    private void PlayAttackVfx(PlayerAttackType type, Vector3 position, int direction)
    {
        var instance = EffectManager.Instance;
        if (instance == null) return;
        // 뒤집기는 프리팹의 VFXFlip이 있으면 Spawn 안에서 자동 처리된다.
        switch (type)
        {
            case PlayerAttackType.SideAttack: instance.PlayAttackSideAttack(position, direction); break;
            case PlayerAttackType.Stab: instance.PlayAttackStab(position, direction); break;
            case PlayerAttackType.Horizontal: instance.PlayAttackHorizontal(position, direction); break;
            case PlayerAttackType.Down: instance.PlayAttackDown(position, direction); break;
            case PlayerAttackType.Up: instance.PlayAttackUp(position, direction); break;
        }
    }
    public void CancelAttack() => EndAttack();
    private void EndAttack()
    {
        if (!IsAttacking) return;
        IsAttacking = false;
        OnAttackEnded?.Invoke();
    }
}
