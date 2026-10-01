using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAttackType
{
    SideAttack,
    Stab,
    Horizontal,
    Down,
    Up,
}

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerMove))]
[RequireComponent(typeof(PlayerDash))]
public class PlayerAttack : MonoBehaviour
{
    [Header("공격 데이터 (SO를 연결하면 바로 사용. 콤보는 SO 나열로 확장)")]
    [SerializeField] private List<PlayerAttackDataSO> _attacks = new();
    [Header("디버그 (플레이 중 발동 판정 표시)")]
    [SerializeField] private bool _showHitboxes;
    private PlayerAttackType _debugLastType;
    private Vector2 _debugLastCenter;
    private Vector2 _debugLastSize;
    private float _debugBoxUntil;
    private readonly Collider2D[] _hitBuffer = new Collider2D[16];
    private PlayerMove _playerMove;
    private PlayerDash _playerDash;
    public event Action OnAttackStarted;
    public event Action OnAttackEnded;
    public bool IsAttacking { get; private set; }
    public string CurrentAttackClip { get; private set; } = "SideAttack";
    public IReadOnlyList<PlayerAttackDataSO> Attacks => _attacks;
    private Rigidbody2D _rigidbody;
    private float _attackTimeRemaining;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerMove = GetComponent<PlayerMove>();
        _playerDash = GetComponent<PlayerDash>();
    }
    private void OnDisable()
    {
        EndAttack();
    }
    private void OnValidate()
    {
        var types = new HashSet<PlayerAttackType>();
        var keys = new HashSet<KeyCode>();
        foreach (var so in _attacks)
        {
            if (so == null) continue;
            if (!types.Add(so.attackType))
                Debug.LogWarning($"공격 타입 중복: {so.attackType} ({so.name})", so);
            foreach (var key in new[] { so.triggerKey, so.altTriggerKey })
            {
                if (key == KeyCode.None) continue;
                if (!keys.Add(key)) Debug.LogWarning($"발동 키 중복: {key} ({so.name})", so);
            }
        }
    }
    private void Update()
    {
        if (IsAttacking)
        {
            _attackTimeRemaining -= Time.deltaTime;
            if (_attackTimeRemaining <= 0f) EndAttack();
            return;
        }
        if (_playerDash.IsDashing) return;
        // 발동 키는 SO가 소유한다. 목록 순서대로 확인하고 먼저 눌린 1종만 발동.
        foreach (var so in _attacks)
        {
            if (so == null) continue;
            if (!Input.GetKeyDown(so.triggerKey) && !(so.altTriggerKey != KeyCode.None && Input.GetKeyDown(so.altTriggerKey))) continue;
            Execute(so);
            break;
        }
    }
    private void Execute(PlayerAttackDataSO so)
    {
        var direction = _playerMove.FacingDirection;
        _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
        IsAttacking = true;
        CurrentAttackClip = string.IsNullOrEmpty(so.animClipName) ? "SideAttack" : so.animClipName;
        _attackTimeRemaining = so.hitbox.Duration;
        if (so.projectilePrefab != null)
            FireProjectile(so, direction);
        else
            DoHit(so, direction);
        OnAttackStarted?.Invoke();
    }
    /// <summary>
    /// 투사체 공격. 즉발 박스 대신 프리팹을 날린다. 애니·발동락·머즐 VFX 흐름은 그대로 쓴다.
    /// SO의 Duration(공격 경직)과 VfxOffset(발사 위치)은 그대로 사용하고 Size/Offset 판정은 쓰지 않는다.
    /// </summary>
    private void FireProjectile(PlayerAttackDataSO so, int direction)
    {
        var spawnPos = (Vector2)transform.position + new Vector2(direction * so.hitbox.VfxOffset.x, so.hitbox.VfxOffset.y);
        SpawnVfx(so.vfxPrefab, spawnPos, direction);
        var spawned = Instantiate(so.projectilePrefab, spawnPos, Quaternion.identity);
        var projectile = spawned.GetComponent<StabProjectile>();
        if (projectile == null)
        {
            Debug.LogError($"투사체 프리팹에 StabProjectile이 없음. 즉발 판정으로 대체한다. ({so.name})", spawned);
            Destroy(spawned);
            DoHit(so, direction);
            return;
        }
        projectile.Launch(so.damage, direction);
        if (_showHitboxes)
        {
            // 디버그는 박스 대신 투사체 비행 경로로 표시한다.
            _debugLastType = so.attackType;
            _debugLastCenter = spawnPos + new Vector2(direction * projectile.MaxDistance * 0.5f, 0f);
            _debugLastSize = new Vector2(projectile.MaxDistance, so.hitbox.Size.y);
            _debugBoxUntil = Time.time + Mathf.Max(so.hitbox.Duration, 0.05f);
        }
    }
    private void DoHit(PlayerAttackDataSO so, int direction)
    {
        var box = so.hitbox;
        var center = (Vector2)transform.position + new Vector2(direction * box.Offset.x, box.Offset.y);
        if (_showHitboxes)
        {
            _debugLastType = so.attackType;
            _debugLastCenter = center;
            _debugLastSize = box.Size;
            _debugBoxUntil = Time.time + Mathf.Max(box.Duration, 0.05f);
        }
        // VFX는 판정 중심이 아니라 전용 오프셋에서 소환한다. 판정을 옮겨도 이펙트 위치는 유지.
        var vfxPos = (Vector2)transform.position + new Vector2(direction * box.VfxOffset.x, box.VfxOffset.y);
        SpawnVfx(so.vfxPrefab, vfxPos, direction);
        var count = Physics2D.OverlapBox(center, box.Size, 0f, ContactFilter2D.noFilter, _hitBuffer);
        for (var i = 0; i < count; i++)
        {
            var hit = _hitBuffer[i];
            if (hit == null || !hit.CompareTag("Enemy")) continue;
            var health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(so.damage);
                EffectManager.Instance?.PlayEnemyHit(hit.transform.position);
                // 사망 이펙트는 각 적의 사망 처리(Dummy/EliteMonsterDeath)가 담당한다. 여기서 중복 재생하지 않는다.
            }
            Debug.Log($"Attack hit {hit.name} for {so.damage} damage.", hit);
            _hitBuffer[i] = null;
        }
    }
    /// <summary>VFX 프리팹 소환. VFXFlip이 붙은 프리팹만 왼쪽 소환 시 뒤집는다.</summary>
    private static GameObject SpawnVfx(GameObject prefab, Vector3 position, int direction)
    {
        if (prefab == null) return null;
        GameObject spawned = Instantiate(prefab, position, Quaternion.identity);
        if (direction < 0)
            spawned.GetComponent<VFXFlip>()?.Apply(direction);
        return spawned;
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
    public void CancelAttack() => EndAttack();
    private void EndAttack()
    {
        if (!IsAttacking) return;
        IsAttacking = false;
        OnAttackEnded?.Invoke();
    }
}
