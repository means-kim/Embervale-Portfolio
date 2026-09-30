using System;
using UnityEngine;

public enum PlayerAttackType
{
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
    [SerializeField, Min(0.05f)] private float _attackDuration = 0.38f;
    [SerializeField, Min(0f)] private Vector2 _attackSize = new Vector2(1.4f, 1.2f);
    [SerializeField, Min(0f)] private float _attackForwardOffset = 0.8f;
    [SerializeField, Min(0f)] private float _attackUpwardOffset = 0.8f;
    [Header("타입별 판정 (1: 찌르기, 3: 내리치기, 4: 올려치기. 2는 위 기본값 사용)")]
    [SerializeField] private Vector2 _stabSize = new Vector2(2.2f, 0.8f);
    [SerializeField] private Vector2 _stabOffset = new Vector2(1.1f, 0.8f);
    [SerializeField] private Vector2 _downSize = new Vector2(1.2f, 1.6f);
    [SerializeField] private Vector2 _downOffset = new Vector2(0.2f, -1.0f);
    [SerializeField] private Vector2 _upSize = new Vector2(1.2f, 1.8f);
    [SerializeField] private Vector2 _upOffset = new Vector2(0.2f, 1.2f);
    [Header("타입별 지속시간 (클립 길이에 맞춤)")]
    [SerializeField, Min(0.05f)] private float _stabDuration = 0.67f;
    [SerializeField, Min(0.05f)] private float _horizontalDuration = 1.17f;
    [SerializeField, Min(0.05f)] private float _downDuration = 1.17f;
    [SerializeField, Min(0.05f)] private float _upDuration = 1.17f;
    [SerializeField] private GameObject _effectPrefab;
    private readonly Collider2D[] _hitBuffer = new Collider2D[16];
    private PlayerInput _playerInput;
    private PlayerMove _playerMove;
    private PlayerDash _playerDash;
    public event Action OnAttackStarted;
    public event Action OnAttackEnded;
    public bool IsAttacking { get; private set; }
    public string CurrentAttackClip { get; private set; } = "Attack";
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
        CurrentAttackClip = "Attack";
        _attackTimeRemaining = _attackDuration;
        DoHit(direction, _attackSize, new Vector2(_attackForwardOffset, _attackUpwardOffset));
        OnAttackStarted?.Invoke();
    }
    private void HandleTypeAttack(PlayerAttackType type)
    {
        if (IsAttacking || _playerDash.IsDashing) return;
        var direction = _playerMove.FacingDirection;
        _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
        IsAttacking = true;
        CurrentAttackClip = type + "Attack";
        _attackTimeRemaining = GetAttackDuration(type);
        DoHit(direction, GetAttackSize(type), GetAttackOffset(type));
        OnAttackStarted?.Invoke();
    }
    private Vector2 GetAttackSize(PlayerAttackType type)
    {
        switch (type)
        {
            case PlayerAttackType.Stab: return _stabSize;
            case PlayerAttackType.Down: return _downSize;
            case PlayerAttackType.Up: return _upSize;
            default: return _attackSize;
        }
    }
    private Vector2 GetAttackOffset(PlayerAttackType type)
    {
        switch (type)
        {
            case PlayerAttackType.Stab: return _stabOffset;
            case PlayerAttackType.Down: return _downOffset;
            case PlayerAttackType.Up: return _upOffset;
            default: return new Vector2(_attackForwardOffset, _attackUpwardOffset);
        }
    }
    private float GetAttackDuration(PlayerAttackType type)
    {
        switch (type)
        {
            case PlayerAttackType.Stab: return _stabDuration;
            case PlayerAttackType.Down: return _downDuration;
            case PlayerAttackType.Up: return _upDuration;
            default: return _horizontalDuration;
        }
    }
    private void DoHit(int direction, Vector2 size, Vector2 offset)
    {
        var center = (Vector2)transform.position + new Vector2(direction * offset.x, offset.y);
        var count = Physics2D.OverlapBox(center, size, 0f, ContactFilter2D.noFilter, _hitBuffer);
        for (var i = 0; i < count; i++)
        {
            var hit = _hitBuffer[i];
            if (hit == null || !hit.CompareTag("Enemy")) continue;
            var health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(_damage);
                EffectManager.Instance?.PlayEnemyHit(hit.transform.position);
                if (health.IsDead) EffectManager.Instance?.PlayEnemyDeath(hit.transform.position);
            }
            Debug.Log($"Attack hit {hit.name} for {_damage} damage.", hit);
            _hitBuffer[i] = null;
        }
    }
    public void CancelAttack() => EndAttack();
    private void EndAttack()
    {
        if (!IsAttacking) return;
        IsAttacking = false;
        OnAttackEnded?.Invoke();
    }
    private void OnDrawGizmosSelected()
    {
        var direction = transform.localScale.x < 0f ? -1 : 1;
        Vector2 center = (Vector2)transform.position + new Vector2(direction * _attackForwardOffset, _attackUpwardOffset);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, _attackSize);
    }
}
