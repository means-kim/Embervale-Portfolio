using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerMove), typeof(PlayerJump), typeof(PlayerDash))]
[RequireComponent(typeof(PlayerAttack), typeof(PlayerWall), typeof(PlayerHit))]
[RequireComponent(typeof(PlayerGlide))]
public class PlayerController : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;
    private Vector2 _moveInput;
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;
    private PlayerMove _playerMove;
    private PlayerJump _playerJump;
    private PlayerDash _playerDash;
    private PlayerAttack _playerAttack;
    private PlayerWall _playerWall;
    private PlayerHit _playerHit;
    private PlayerAnimator _playerAnimator;
    private PlayerChargeJump _chargeJump;
    private PlayerGlide _glide;
    private Health _health;
    public bool CanAttack => isActiveAndEnabled && CurrentState != PlayerState.Death && (_health == null || !_health.IsDead) && !_playerHit.IsHit && !_playerDash.IsDashing && !_playerAttack.IsAttacking
        && (_chargeJump == null || !_chargeJump.IsActive);
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
        _chargeJump = GetComponent<PlayerChargeJump>();
        _glide = GetComponent<PlayerGlide>();
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
        var verticalVelocity = _rigidbody.linearVelocity.y;
        _playerAnimator.Tick(CurrentState, false, verticalVelocity);
    }
    public void ResetForRespawn()
    {
        _moveInput = Vector2.zero;
        CurrentState = PlayerState.Idle;
        _playerAnimator.ResetJumpPhase();
        _playerAnimator.PlayState(CurrentState, false);
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
        if (_playerDash.IsDashing)
        {
            ChangeState(PlayerState.Dash);
            return;
        }
        if (_playerWall.IsClinging)
        {
            ChangeState(PlayerState.WallCling);
            return;
        }
        if (_playerJump.IsJumping)
        {
            ChangeState(_glide != null && _glide.IsGliding ? PlayerState.Glide : PlayerState.Jump);
            return;
        }
        if (!_playerJump.IsGrounded)
        {
            var stateCheck = CurrentState == PlayerState.Run || CurrentState == PlayerState.Jump || CurrentState == PlayerState.Dash || CurrentState == PlayerState.Attack || CurrentState == PlayerState.WallCling;
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
        _playerAttack.CancelAttack();
        _playerAnimator.ResetJumpPhase();
        EvaluateState();
    }
    private void HandleGroundLost()
    {
        if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Run) ChangeState(PlayerState.Jump);
    }
    private void HandleDashStarted()
    {
        ChangeState(PlayerState.Dash);
    }
    private void HandleDashEnded() => EvaluateState();
    private void HandleAttackStarted()
    {
        ChangeState(PlayerState.Attack);
    }
    private void HandleAttackEnded() => EvaluateState();
}
