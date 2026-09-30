using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerJump))]
[RequireComponent(typeof(PlayerMove), typeof(PlayerDash))]
public class PlayerWall : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _climbSpeed = 3f;
    [SerializeField, Min(0f)] private float _slideSpeed = 1.5f;
    [SerializeField, Min(0f)] private float _wallJumpAwaySpeed = 8f;
    [SerializeField, Min(0f)] private float _wallJumpLockDuration = 0.2f;
    private const int MaxStamina = 100;
    private const float StaminaTick = 0.1f;
    private const int ClingDrain = 1;
    private const int ClimbDrain = 2;
    private const int WallJumpCost = 25;
    public bool IsClinging { get; private set; }
    public bool IsClimbing { get; private set; }
    public int Stamina => _stamina;
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;
    private PlayerJump _playerJump;
    private PlayerMove _playerMove;
    private PlayerDash _playerDash;
    private Vector2 _moveInput;
    private readonly Dictionary<Collider2D, float> _wallContacts = new();
    private int _stamina = MaxStamina;
    private float _staminaTimer;
    private float _clingDirection;
    private bool _touchingWall;
    private float _regrabUntil;
    private float _moveLockUntil;
    private bool _exhausted;
    private bool _moveLocked;
    // 착지 후 벽 점프를 한 번이라도 썼는지. 첫 벽 점프만 스태미나를 소모한다.
    private bool _wallJumpUsed;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _playerJump = GetComponent<PlayerJump>();
        _playerMove = GetComponent<PlayerMove>();
        _playerDash = GetComponent<PlayerDash>();
        EnsureFrictionlessCollider();
    }
    private void OnEnable()
    {
        _playerInput.OnMoveInput += HandleMove;
        _playerInput.OnWallJumpInput += HandleWallJump;
        _playerJump.OnLanded += HandleLanded;
    }
    private void OnDisable()
    {
        _playerInput.OnMoveInput -= HandleMove;
        _playerInput.OnWallJumpInput -= HandleWallJump;
        _playerJump.OnLanded -= HandleLanded;
        if (_moveLocked)
        {
            _moveLocked = false;
            _playerMove.SetMovementLocked(false);
        }
    }
    private void EnsureFrictionlessCollider()
    {
        // 기본 마찰(0.4)이 있으면 탈진 후에도 A/D로 벽을 누르는 동안 마찰력이 몸을 붙잡는다.
        // 런타임 전용 마찰 0 소재로 교체해 탈진 시 자유 낙하가 되게 한다. 에셋 파일은 건드리지 않는다.
        var playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null) return;
        var material = playerCollider.sharedMaterial;
        if (material != null && material.friction <= 0f && material.bounciness <= 0f) return;
        playerCollider.sharedMaterial = new PhysicsMaterial2D("PlayerWallFrictionless")
        {
            friction = 0f,
            bounciness = 0f
        };
    }
    private void HandleMove(Vector2 direction) => _moveInput = direction;
    private void Update()
    {
        if (_moveLocked && Time.time >= _moveLockUntil)
        {
            _moveLocked = false;
            if (!_playerDash.IsDashing) _playerMove.SetMovementLocked(false);
        }
        var pressingToward = 0f;
        if (Mathf.Abs(_moveInput.x) > 0.01f) pressingToward = Mathf.Sign(_moveInput.x);
        var canGrab = Time.time >= _regrabUntil && !_exhausted;
        // 물리적 접촉(벽+방향키+공중). 스태미나와 별개로, 닿아 있는 동안은 코드가 낙하를 제어한다.
        // 마찰력만으로는 탈진 후에도 몸이 벽에 붙어 버리기 때문이다.
        _touchingWall = pressingToward != 0f && _wallContacts.ContainsValue(pressingToward) && !_playerJump.IsGrounded;
        IsClinging = _touchingWall && canGrab;
        IsClimbing = IsClinging && Mathf.Abs(_moveInput.y) > 0.01f;
        if (IsClinging) _clingDirection = pressingToward;
        if (IsClinging)
        {
            _staminaTimer += Time.deltaTime;
            while (_staminaTimer >= StaminaTick && _stamina > 0)
            {
                _staminaTimer -= StaminaTick;
                _stamina = Mathf.Max(0, _stamina - (IsClimbing ? ClimbDrain : ClingDrain));
            }
            if (_stamina <= 0)
            {
                _exhausted = true;
                IsClinging = false;
                IsClimbing = false;
            }
        }
        else
        {
            _staminaTimer = 0f;
        }
    }
    private void HandleWallJump()
    {
        if (!IsClinging) return;
        // 벽 점프도 최대 점프 횟수를 공유한다. 일반 점프와 합쳐 최대 횟수까지만 가능하다.
        if (_playerJump.CurrentJumpCount >= _playerJump.MaxJumpCount) return;
        bool freeJump = _wallJumpUsed;
        if (!freeJump && _stamina < WallJumpCost) return;
        if (!_playerJump.TryConsumeJump()) return;
        // 첫 번째 벽 점프만 벽 체력을 소모하고, 이후부터는 소모하지 않는다.
        if (!freeJump)
        {
            _stamina -= WallJumpCost;
            if (_stamina <= 0) _exhausted = true;
            _wallJumpUsed = true;
        }
        _regrabUntil = Time.time + _wallJumpLockDuration;
        _moveLockUntil = Time.time + _wallJumpLockDuration;
        _moveLocked = true;
        _playerMove.SetMovementLocked(true);
        IsClinging = false;
        IsClimbing = false;
        _rigidbody.linearVelocity = new Vector2(-_clingDirection * _wallJumpAwaySpeed, _playerJump.JumpSpeed);
    }
    private void HandleLanded()
    {
        _stamina = MaxStamina;
        _staminaTimer = 0f;
        _exhausted = false;
        _wallJumpUsed = false;
    }
    private void FixedUpdate()
    {
        if (IsClimbing)
        {
            var velocity = _rigidbody.linearVelocity;
            velocity.y = _moveInput.y * _climbSpeed;
            _rigidbody.linearVelocity = velocity;
        }
        else if (!_exhausted && _touchingWall && _rigidbody.linearVelocity.y < 0f)
        {
            // 스태미나가 남아 있을 때만 접촉 중 낙하를 코드가 강제한다.
            // 탈진 상태에서는 붙잡지 않고 자유 낙하시킨다.
            // 상승 중(벽점프 직후 등)은 건드리지 않는다.
            var velocity = _rigidbody.linearVelocity;
            velocity.y = -_slideSpeed;
            _rigidbody.linearVelocity = velocity;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) => TrackWall(collision);
    private void OnCollisionStay2D(Collision2D collision) => TrackWall(collision);
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider != null) _wallContacts.Remove(collision.collider);
    }
    private void TrackWall(Collision2D collision)
    {
        if (collision.collider == null || !collision.collider.CompareTag("Ground")) return;
        int contactCount = collision.contactCount;
        for (int i = 0; i < contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            // 옆면이면 벽으로 취급한다. 윗면은 착지 판정이 처리한다.
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                // normal은 벽에서 플레이어 쪽을 향하므로 달라붙으려면 반대 방향을 눌러야 한다.
                _wallContacts[collision.collider] = -Mathf.Sign(contact.normal.x);
                return;
            }
        }
    }
}
