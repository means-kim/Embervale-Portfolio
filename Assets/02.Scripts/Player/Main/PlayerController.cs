using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerMove), typeof(PlayerJump), typeof(PlayerDash))]
[RequireComponent(typeof(PlayerAttack), typeof(PlayerWall), typeof(PlayerHit))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _landDuration = 0.17f;
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;
    private Vector2 _moveInput;
    private Rigidbody2D _rigidbody;
    private float _landUntil;
    private PlayerInput _playerInput;
    private PlayerMove _playerMove;
    private PlayerJump _playerJump;
    private PlayerDash _playerDash;
    private PlayerAttack _playerAttack;
    private PlayerWall _playerWall;
    private PlayerHit _playerHit;
    private PlayerAnimator _playerAnimator;
    private Health _health;
    public bool CanAttack => isActiveAndEnabled && CurrentState != PlayerState.Death &&
        (_health == null || !_health.IsDead) && !_playerHit.IsHit &&
        !_playerDash.IsDashing && !_playerAttack.IsAttacking;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();
        _playerInput = GetComponent<PlayerInput>();
        _playerMove = GetComponent<PlayerMove>();
        _playerJump = GetComponent<PlayerJump>();
        _playerDash = GetComponent<PlayerDash>();
        _playerAttack = GetComponent<PlayerAttack>();
        _playerWall = GetComponent<PlayerWall>();
        _playerHit = GetComponent<PlayerHit>();
        _playerAnimator = GetComponent<PlayerAnimator>();
        if (_playerAnimator == null) _playerAnimator = gameObject.AddComponent<PlayerAnimator>();
    }
    private void OnEnable()
    {
        _playerInput.OnMoveInput += HandleMoveInput;
        _playerInput.OnAttackInput += HandleAttackInput;
        _playerJump.OnJumpStarted += HandleJumpStarted;
        _playerJump.OnLanded += HandleLanded;
        _playerJump.OnGroundLost += HandleGroundLost;
        _playerDash.OnDashStarted += HandleDashStarted;
        _playerDash.OnDashEnded += HandleDashEnded;
        _playerAttack.OnAttackStarted += HandleAttackStarted;
        _playerAttack.OnAttackEnded += HandleAttackEnded;
    }
    private void OnDisable()
    {
        _playerInput.OnMoveInput -= HandleMoveInput;
        _playerInput.OnAttackInput -= HandleAttackInput;
        _playerJump.OnJumpStarted -= HandleJumpStarted;
        _playerJump.OnLanded -= HandleLanded;
        _playerJump.OnGroundLost -= HandleGroundLost;
        _playerDash.OnDashStarted -= HandleDashStarted;
        _playerDash.OnDashEnded -= HandleDashEnded;
        _playerAttack.OnAttackStarted -= HandleAttackStarted;
        _playerAttack.OnAttackEnded -= HandleAttackEnded;
    }
    private bool IsDoubleJump => _playerJump != null && _playerJump.CurrentJumpCount > 1;
    private void Start() => _playerAnimator.PlayState(CurrentState, IsDoubleJump);
    private void Update()
    {
        EvaluateState();
        var landing = Time.time < _landUntil;
        var verticalVelocity = _rigidbody.linearVelocity.y;
        var dashDuration = _playerDash.DashDuration;
        _playerAnimator.Tick(CurrentState, landing, verticalVelocity, dashDuration);
    }
    public void ChangeState(PlayerState newState)
    {
        if (CurrentState == PlayerState.Death && newState != PlayerState.Death) return;
        if (CurrentState == newState) return;
        if (CurrentState == PlayerState.Jump) _playerAnimator.ResetJumpPhase();
        CurrentState = newState;
        _playerAnimator.PlayState(CurrentState, IsDoubleJump);
    }
    private void EvaluateState()
    {
        if (CurrentState == PlayerState.Death) return;
        if (_playerHit != null && _playerHit.IsHit)
        {
            ChangeState(PlayerState.Hit);
            return;
        }
        if (_playerAttack.IsAttacking) { ChangeState(PlayerState.Attack); return; }
        if (Time.time < _landUntil) return;
        if (_playerDash.IsDashing)
        {
            ChangeState(PlayerState.Dash);
            return;
        }
        if (_playerWall.IsClimbing)
        {
            ChangeState(PlayerState.WallClimb);
            return;
        }
        if (_playerWall.IsClinging)
        {
            ChangeState(PlayerState.WallCling);
            return;
        }
        if (_playerJump.IsJumping)
        {
            ChangeState(PlayerState.Jump);
            return;
        }
        if (!_playerJump.IsGrounded)
        {
            var stateCheck = CurrentState == PlayerState.Run || CurrentState == PlayerState.Jump || CurrentState == PlayerState.Dash || CurrentState == PlayerState.Attack || CurrentState == PlayerState.WallCling || CurrentState == PlayerState.WallClimb;
            if (stateCheck) ChangeState(PlayerState.Jump);
            return;
        }
        var moving = _playerMove != null ? _playerMove.HasMoveInput : Mathf.Abs(_moveInput.x) > 0.01f;
        ChangeState(moving ? PlayerState.Run : PlayerState.Idle);
    }
    private void HandleMoveInput(Vector2 direction) => _moveInput = direction;
    private void HandleAttackInput(PlayerAttackDataSO definition)
    {
        if (CanAttack) _playerAttack.TryStartAttack(definition);
    }
    private void HandleJumpStarted()
    {
        _playerAttack.CancelAttack();
        _landUntil = 0f;
        ChangeState(PlayerState.Jump);
        _playerAnimator.BeginJump(IsDoubleJump);
    }
    private void HandleLanded()
    {
        if (CurrentState == PlayerState.Death || _playerHit.IsHit) return;
        if (_playerDash.IsDashing)
        {
            EvaluateState();
            return;
        }
        _landUntil = Time.time + _landDuration;
        _playerAttack.CancelAttack();
        _playerAnimator.PlayLand();
        EvaluateState();
    }
    private void HandleGroundLost()
    {
        if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Run) ChangeState(PlayerState.Jump);
    }
    private void HandleDashStarted()
    {
        _landUntil = 0f;
        ChangeState(PlayerState.Dash);
        _playerAnimator.BeginDash();
    }
    private void HandleDashEnded() => EvaluateState();
    private void HandleAttackStarted()
    {
        _landUntil = 0f;
        ChangeState(PlayerState.Attack);
    }
    private void HandleAttackEnded() => EvaluateState();
}
