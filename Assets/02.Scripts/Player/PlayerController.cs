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
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
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
        if (CurrentState == newState) return;
        if (CurrentState == PlayerState.Jump) _playerAnimator.ResetJumpPhase();
        CurrentState = newState;
        _playerAnimator.PlayState(CurrentState, IsDoubleJump);
    }
    private void EvaluateState()
    {
        if (CurrentState == PlayerState.Death) return;
        // 피격 모션을 다른 상태보다 우선 재생한다.
        if (_playerHit != null && _playerHit.IsHit)
        {
            ChangeState(PlayerState.Hit);
            return;
        }
        if (CurrentState == PlayerState.Attack && _playerAttack.IsAttacking) return;
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
        var moving = _playerMove.IsMoving || Mathf.Abs(_moveInput.x) > 0.01f;
        ChangeState(moving ? PlayerState.Run : PlayerState.Idle);
    }
    private void HandleMoveInput(Vector2 direction) => _moveInput = direction;
    private void HandleJumpStarted()
    {
        _landUntil = 0f;
        ChangeState(PlayerState.Jump);
        _playerAnimator.BeginJump(IsDoubleJump);
    }
    private void HandleLanded()
    {
        if (_playerDash.IsDashing)
        {
            EvaluateState();
            return;
        }
        _landUntil = Time.time + _landDuration;
        if (CurrentState == PlayerState.Attack) _playerAttack.CancelAttack();
        _playerAnimator.PlayLand();
        EvaluateState();
    }
    private void HandleGroundLost()
    {
        // 드롭스루 등으로 가만히 서 있다가 공중이 되면 Jump로 전환한다.
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
        _playerAnimator.PlayAttackClip(_playerAttack.CurrentAttackClip);
    }
    private void HandleAttackEnded() => EvaluateState();
}