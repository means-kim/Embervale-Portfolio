using System;
using System.Collections.Generic;
using UnityEngine;
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
    private PlayerMove _playerMove;
    private PlayerDash _playerDash;
    public event Action OnAttackStarted;
    public event Action OnAttackEnded;
    public bool IsAttacking { get; private set; }
    public string CurrentAttackClip { get; private set; } = "LeftHorizontalAttack";
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
        CurrentAttackClip = string.IsNullOrEmpty(so.animClipName) ? "LeftHorizontalAttack" : so.animClipName;
        _attackTimeRemaining = so.hitbox.Duration;
        if (so.projectilePrefab != null)
            FireProjectile(so, direction);
        else
            SpawnMeleeHitbox(so, direction);
        OnAttackStarted?.Invoke();
    }
    private void FireProjectile(PlayerAttackDataSO so, int direction)
    {
        var spawnPos = (Vector2)transform.position + new Vector2(direction * so.hitbox.Offset.x, so.hitbox.Offset.y);
        SpawnVfx(so.vfxPrefab, spawnPos, direction);
        var spawned = Instantiate(so.projectilePrefab, spawnPos, Quaternion.identity);
        var projectile = spawned.GetComponent<IPlayerProjectile>();
        if (projectile == null)
        {
            Debug.LogError($"투사체 프리팹에 IPlayerProjectile이 없음. 근접 히트박스로 대체한다. ({so.name})", spawned);
            Destroy(spawned);
            SpawnMeleeHitbox(so, direction);
            return;
        }
        projectile.Launch(so.damage, direction);
        if (_showHitboxes)
        {
            _debugLastType = so.attackType;
            _debugLastCenter = spawnPos + new Vector2(direction * projectile.MaxDistance * 0.5f, 0f);
            _debugLastSize = new Vector2(projectile.MaxDistance, so.hitbox.Size.y);
            _debugBoxUntil = Time.time + Mathf.Max(so.hitbox.Duration, 0.05f);
        }
    }
    private void SpawnMeleeHitbox(PlayerAttackDataSO so, int direction)
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
        SpawnVfx(so.vfxPrefab, center, direction);
        var hitboxObject = new GameObject($"MeleeHitbox_{so.attackType}");
        hitboxObject.transform.position = center;
        var melee = hitboxObject.AddComponent<PlayerMeleeHitbox>();
        melee.Setup(so.damage, box.Size, box.Duration);
    }
    private static GameObject SpawnVfx(GameObject prefab, Vector3 position, int direction)
    {
        if (prefab == null) return null;
        GameObject spawned = Instantiate(prefab, position, Quaternion.identity);
        if (direction < 0)
            spawned.GetComponent<VFXFlip>()?.Apply(direction);
        return spawned;
    }
    public bool DebugDrawEnabled => _showHitboxes;
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
