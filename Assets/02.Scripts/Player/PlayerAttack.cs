using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerMove))]
[RequireComponent(typeof(PlayerDash))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField, Min(0)] private int _damage = 10;
    [SerializeField, Min(0.05f)] private float _attackDuration = 0.38f;
    [SerializeField, Min(0f)] private Vector2 _attackSize = new Vector2(1.4f, 1.2f);
    [SerializeField, Min(0f)] private float _attackForwardOffset = 0.8f;
    [SerializeField, Min(0f)] private float _attackUpwardOffset = 0.8f;
    [SerializeField] private GameObject _effectPrefab;
    private readonly Collider2D[] _hitBuffer = new Collider2D[16];
    private PlayerInput _playerInput;
    private PlayerMove _playerMove;
    private PlayerDash _playerDash;
    public event Action OnAttackStarted;
    public event Action OnAttackEnded;
    public bool IsAttacking { get; private set; }
    private Rigidbody2D _rigidbody;
    private float _attackTimeRemaining;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _playerMove = GetComponent<PlayerMove>();
        _playerDash = GetComponent<PlayerDash>();
    }
    private void OnEnable() => _playerInput.OnAttackInput += HandleAttack;
    private void OnDisable()
    {
        _playerInput.OnAttackInput -= HandleAttack;
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
        _attackTimeRemaining = _attackDuration;
        DoHit(direction);
        OnAttackStarted?.Invoke();
    }
    private void DoHit(int direction)
    {
        var center = (Vector2)transform.position + new Vector2(direction * _attackForwardOffset, _attackUpwardOffset);
        var count = Physics2D.OverlapBox(center, _attackSize, 0f, ContactFilter2D.noFilter, _hitBuffer);
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
