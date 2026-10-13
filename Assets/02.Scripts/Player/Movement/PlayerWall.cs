using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerJump))]
[RequireComponent(typeof(PlayerMove))]
public class PlayerWall : MonoBehaviour
{
    private enum WallState { None, Cling, Slide }
    [SerializeField, Min(0f)] private float _slideSpeed = 1.5f;
    [SerializeField, Min(0f)] private float _wallJumpAwaySpeed = 8f;
    [SerializeField, Min(0f)] private float _wallJumpLockDuration = 0.2f;
    [SerializeField, Min(0f)] private float _wallProbeDistance = 0.05f;
    [SerializeField, Range(0.5f, 1f)] private float _minimumWallNormal = 0.5f;
    [SerializeField] private bool _chargeOnlyFirstWallJump = true;

    public bool IsClinging => isActiveAndEnabled && _state != WallState.None;
    public int Stamina { get; private set; }
    private Rigidbody2D _body;
    private PlayerInput _input;
    private PlayerJump _jump;
    private PlayerMove _move;
    private PlayerDash _dash;
    private PlayerHit _hit;
    private Health _health;
    private PlayerWallSensor _sensor;
    private Func<Collider2D, bool> _isIgnoredPlatform;
    private Vector2 _moveInput;
    private WallState _state;
    private int _wallDirection;
    private float _wallJumpUntil;
    private float _staminaTimer;
    private int _maxStamina = 100;
    private float _staminaTick = 0.1f;
    private int _clingDrain = 1;
    private int _wallJumpCost = 25;
    private bool _moveLocked;
    private bool _wallJumpUsed;
    private Collider2D _collider;
    private PhysicsMaterial2D _originalMaterial;
    private PhysicsMaterial2D _runtimeMaterial;
    private int JumpCost
    {
        get
        {
            if (_chargeOnlyFirstWallJump && _wallJumpUsed)
            {
                return 0;
            }
            return _wallJumpCost;
        }
    }
    private bool IsJumpEnabled => _jump.isActiveAndEnabled;
    private bool IsAlive => _health == null || !_health.IsDead;
    private bool IsStable => _hit == null || !_hit.IsHit;
    private bool IsNotDashing => _dash == null || !_dash.IsDashing;
    private bool HasNoExternalLock => !_move.HasMovementLockOtherThan(this);
    private bool CanUseWall => IsJumpEnabled && IsAlive && IsStable && IsNotDashing && HasNoExternalLock;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInput>();
        _jump = GetComponent<PlayerJump>();
        _move = GetComponent<PlayerMove>();
        _dash = GetComponent<PlayerDash>();
        _hit = GetComponent<PlayerHit>();
        _health = GetComponent<Health>();
        _sensor = new PlayerWallSensor(_body);
        _isIgnoredPlatform = _jump.IsIgnoringPlatform;
        Stamina = _maxStamina;
        _collider = GetComponent<Collider2D>();
        if (_collider == null) return;
        _originalMaterial = _collider.sharedMaterial;
        if (_originalMaterial != null
            && _originalMaterial.friction <= 0f
            && _originalMaterial.bounciness <= 0f)
        {
            return;
        }
        _runtimeMaterial = new PhysicsMaterial2D("PlayerWallFrictionless") { friction = 0f, bounciness = 0f };
        _collider.sharedMaterial = _runtimeMaterial;
    }
    private void OnEnable()
    {
        _input.OnMoveInput += HandleMove;
        _jump.OnLanded += HandleLanded;
        if (_health == null) return;
        _health.OnHit += ReleaseWall;
        _health.OnDie += ReleaseWall;
    }
    private void OnDisable()
    {
        _input.OnMoveInput -= HandleMove;
        _jump.OnLanded -= HandleLanded;
        if (_health != null)
        {
            _health.OnHit -= ReleaseWall;
            _health.OnDie -= ReleaseWall;
        }
        _moveInput = Vector2.zero;
        _staminaTimer = 0f;
        ReleaseWall();
    }
    private void OnDestroy()
    {
        if (_runtimeMaterial == null) return;
        if (_collider != null && _collider.sharedMaterial == _runtimeMaterial)
        {
            _collider.sharedMaterial = _originalMaterial;
        }
        if (Application.isPlaying) Destroy(_runtimeMaterial);
        else DestroyImmediate(_runtimeMaterial);
    }
    private void HandleMove(Vector2 input)
    {
        _moveInput = input;
        RefreshWallState();
    }
    private void Update()
    {
        RefreshWallState();
        if (!IsClinging) { _staminaTimer = 0f; return; }
        _staminaTimer += Time.deltaTime;
        float interval = Mathf.Max(0.01f, _staminaTick);
        while (_staminaTimer >= interval && Stamina > 0)
        {
            _staminaTimer -= interval;
            Stamina = Mathf.Max(0, Stamina - _clingDrain);
        }
        if (Stamina == 0) RefreshWallState();
    }
    private void RefreshWallState()
    {
        int direction = 0;
        if (Mathf.Abs(_moveInput.x) > 0.01f)
        {
            if (_moveInput.x > 0f)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
        }
        bool attached =
            CanUseWall
            && !_jump.IsGrounded
            && Stamina > 0
            && Time.time >= _wallJumpUntil
            && _sensor.HasWall(direction, _wallProbeDistance, _minimumWallNormal, _isIgnoredPlatform);
        if (attached)
        {
            _wallDirection = direction;
            if (!IsClinging) _jump.RefreshJumpsFromWall();
        }
        WallState nextState = WallState.None;
        if (attached)
        {
            if (_body.linearVelocity.y < 0f)
            {
                nextState = WallState.Slide;
            }
            else
            {
                nextState = WallState.Cling;
            }
        }
        SetState(nextState);
        SetMovementLock(IsClinging || Time.time < _wallJumpUntil);
    }
    public bool TryHandleJumpInput()
    {
        if (!isActiveAndEnabled) return false;
        RefreshWallState();
        if (!IsClinging) return false;
        if (Stamina < JumpCost || !_jump.TryConsumeJump()) return true;
        Stamina -= JumpCost;
        _wallJumpUsed = true;
        _staminaTimer = 0f;
        _wallJumpUntil = Time.time + _wallJumpLockDuration;
        SetState(WallState.None);
        SetMovementLock(Time.time < _wallJumpUntil);
        _body.linearVelocity = new Vector2(-_wallDirection * _wallJumpAwaySpeed, _jump.JumpSpeed);
        return true;
    }
    public void ResetForRespawn()
    {
        _moveInput = Vector2.zero;
        Stamina = _maxStamina;
        _staminaTimer = 0f;
        _wallJumpUsed = false;
        ReleaseWall();
    }
    private void HandleLanded()
    {
        Stamina = _maxStamina;
        _staminaTimer = 0f;
        _wallJumpUsed = false;
        ReleaseWall();
    }
    private void ReleaseWall()
    {
        SetState(WallState.None);
        _wallJumpUntil = 0f;
        SetMovementLock(false);
    }
    private void FixedUpdate()
    {
        RefreshWallState();
        if (!IsClinging) return;
        Vector2 velocity = _body.linearVelocity;
        velocity.x = 0f; // Keep the player beside one-way platform sides.
        if (velocity.y < 0f) velocity.y = -_slideSpeed;
        _body.linearVelocity = velocity;
    }
    private void SetState(WallState state)
    {
        if (_state == state) return;
        _state = state;
        var effects = EffectManager.Instance;
        if (effects == null) return;
        if (state == WallState.Slide) effects.PlayWallSlide(transform.position);
    }
    private void SetMovementLock(bool locked)
    {
        if (_moveLocked == locked) return;
        _moveLocked = locked;
        _move.SetMovementLocked(this, locked);
    }
}
