using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerMove))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private PlayerDash _playerDash;
    [SerializeField, Min(0)] private int _damage = 10;
    [SerializeField, Min(0.05f)] private float _attackDuration = 0.38f;
    [SerializeField, Min(0f)] private Vector2 _attackSize = new Vector2(1.4f, 1.2f);
    [SerializeField, Min(0f)] private float _attackForwardOffset = 0.8f;

    public event Action OnAttackStarted;
    public event Action OnAttackEnded;
    public bool IsAttacking { get; private set; }

    private Rigidbody2D _rigidbody;
    private float _attackTimeRemaining;

    private void Reset()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerMove = GetComponent<PlayerMove>();
        _playerDash = GetComponent<PlayerDash>();
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        if (_playerInput == null)
        {
            _playerInput = GetComponent<PlayerInput>();
        }
        if (_playerMove == null)
        {
            _playerMove = GetComponent<PlayerMove>();
        }
        if (_playerDash == null)
        {
            _playerDash = GetComponent<PlayerDash>();
        }
    }

    private void OnEnable()
    {
        if (_playerInput != null)
        {
            _playerInput.OnAttackInput += HandleAttack;
        }
    }

    private void OnDisable()
    {
        if (_playerInput != null)
        {
            _playerInput.OnAttackInput -= HandleAttack;
        }
        EndAttack();
    }

    private void Update()
    {
        if (IsAttacking)
        {
            _attackTimeRemaining -= Time.deltaTime;
            if (_attackTimeRemaining <= 0f)
            {
                EndAttack();
            }
        }
    }

    private void HandleAttack()
    {
        if (IsAttacking || _rigidbody == null)
        {
            return;
        }
        if (_playerDash != null && _playerDash.IsDashing)
        {
            return;
        }

        int direction = _playerMove != null
            ? _playerMove.FacingDirection
            : (transform.localScale.x < 0f ? -1 : 1);

        // 절충안: 발동 순간 수평 속도만 0으로 하고 이동 자체는 잠그지 않는다.
        // 수직 속도는 유지해서 공중 공격 중 낙하가 이어진다.
        _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);

        IsAttacking = true;
        _attackTimeRemaining = _attackDuration;
        DoHit(direction);
        OnAttackStarted?.Invoke();
    }

    private void DoHit(int direction)
    {
        Vector2 center = (Vector2)transform.position + new Vector2(direction * _attackForwardOffset, 0f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, _attackSize, 0f);
        foreach (Collider2D hit in hits)
        {
            if (hit == null || !hit.CompareTag("Enemy"))
            {
                continue;
            }

            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(_damage);
            }
            Debug.Log($"Attack hit {hit.name} for {_damage} damage.", hit);
        }
    }

    public void CancelAttack()
    {
        EndAttack();
    }

    private void EndAttack()
    {
        if (!IsAttacking)
        {
            return;
        }

        IsAttacking = false;
        OnAttackEnded?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        int direction = transform.localScale.x < 0f ? -1 : 1;
        Vector2 center = (Vector2)transform.position + new Vector2(direction * _attackForwardOffset, 0f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, _attackSize);
    }
}
