using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;
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
    public bool CanAttack => isActiveAndEnabled
        && CurrentState != PlayerState.Death
        && !_health.IsDead
        && !_playerHit.IsHit
        && !_playerDash.IsDashing
        && !_playerAttack.IsAttacking
        && !_chargeJump.IsActive;
    private void Awake()
    {
        PlayerRig rig = PlayerRig.Resolve(gameObject);
        _rigidbody = rig.Body;
        _health = rig.Health;
        _playerInput = rig.Input;
        _playerMove = rig.Move;
        _playerJump = rig.Jump;
        _playerDash = rig.Dash;
        _playerAttack = rig.Attack;
        _playerWall = rig.Wall;
        _playerHit = rig.Hit;
        _playerAnimator = rig.PlayerAnimator;
        _chargeJump = rig.ChargeJump;
        _glide = rig.Glide;
    }
    private void OnEnable()
    {
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
        _playerInput.OnAttackInput -= HandleAttackInput;
        _playerJump.OnJumpStarted -= HandleJumpStarted;
        _playerJump.OnLanded -= HandleLanded;
        _playerJump.OnGroundLost -= HandleGroundLost;
        _playerDash.OnDashStarted -= HandleDashStarted;
        _playerDash.OnDashEnded -= HandleDashEnded;
        _playerAttack.OnAttackStarted -= HandleAttackStarted;
        _playerAttack.OnAttackEnded -= HandleAttackEnded;
    }
    private bool IsDoubleJump => _playerJump.CurrentJumpCount > 1;
    private void Start() => _playerAnimator.PlayState(CurrentState, IsDoubleJump);
    private void Update()
    {
        EvaluateState();
        var verticalVelocity = _rigidbody.linearVelocity.y;
        _playerAnimator.Tick(CurrentState, false, verticalVelocity);
    }
    public void ResetForRespawn()
    {
        CurrentState = PlayerState.Idle;
        _playerAnimator.ResetJumpPhase();
        _playerAnimator.PlayState(CurrentState, false);
    }
    public void ChangeState(PlayerState newState)
    {
        if (CurrentState == PlayerState.Death && newState != PlayerState.Death)
        {
            return;
        }
        if (CurrentState == newState)
        {
            return;
        }
        if (CurrentState == PlayerState.Jump)
        {
            _playerAnimator.ResetJumpPhase();
        }
        CurrentState = newState;
        _playerAnimator.PlayState(CurrentState, IsDoubleJump);
    }
    private void EvaluateState()
    {
        if (CurrentState == PlayerState.Death)
        {
            return;
        }
        if (_playerHit.IsHit)
        {
            ChangeState(PlayerState.Hit);
            return;
        }
        if (_playerAttack.IsAttacking)
        {
            ChangeState(PlayerState.Attack);
            return;
        }
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
            if (_glide.IsGliding)
            {
                ChangeState(PlayerState.Glide);
            }
            else
            {
                ChangeState(PlayerState.Jump);
            }
            return;
        }
        if (!_playerJump.IsGrounded)
        {
            if (_glide.IsGliding)
            {
                ChangeState(PlayerState.Glide);
                return;
            }
            var stateCheck = CurrentState == PlayerState.Run || CurrentState == PlayerState.Jump || CurrentState == PlayerState.Dash || CurrentState == PlayerState.Attack || CurrentState == PlayerState.WallCling || CurrentState == PlayerState.Glide;
            if (stateCheck)
            {
                ChangeState(PlayerState.Jump);
            }
            return;
        }
        bool moving = _playerMove.HasMoveInput;
        if (moving)
        {
            ChangeState(PlayerState.Run);
        }
        else
        {
            ChangeState(PlayerState.Idle);
        }
    }
    private void HandleAttackInput(PlayerAttackDataSO definition)
    {
        if (CanAttack)
        {
            _playerAttack.TryStartAttack(definition);
        }
    }
    private void HandleJumpStarted()
    {
        _playerAttack.CancelAttack();
        ChangeState(PlayerState.Jump);
        _playerAnimator.BeginJump(IsDoubleJump);
    }
    private void HandleLanded()
    {
        if (CurrentState == PlayerState.Death || _playerHit.IsHit)
        {
            return;
        }
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
        if (CurrentState == PlayerState.Idle || CurrentState == PlayerState.Run)
        {
            ChangeState(PlayerState.Jump);
        }
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
