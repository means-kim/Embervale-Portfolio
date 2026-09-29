using UnityEngine;
[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMove), typeof(PlayerJump), typeof(PlayerDash))]
[RequireComponent(typeof(PlayerAttack), typeof(PlayerWall))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _jumpStartDuration = 0.13f;
    [SerializeField, Min(0f)] private float _apexVelocityRange = 1.5f;
    [SerializeField, Min(0f)] private float _dashStartDuration = 0.075f;
    [SerializeField, Min(0f)] private float _dashEndDuration = 0.13f;
    [SerializeField, Min(0f)] private float _landDuration = 0.17f;
    public PlayerState CurrentState => _currentState;
    private PlayerState _currentState = PlayerState.Idle;
    private Vector2 _moveInput;
    private Rigidbody2D _rigidbody;
    private int _jumpPhase = 2;
    private float _jumpStartTime;
    private float _dashStartTime;
    private float _landUntil;
    private string _currentClip;
    private Animator _animator;
    private PlayerInput _playerInput;
    private PlayerMove _playerMove;
    private PlayerJump _playerJump;
    private PlayerDash _playerDash;
    private PlayerAttack _playerAttack;
    private PlayerWall _playerWall;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _playerMove = GetComponent<PlayerMove>();
        _playerJump = GetComponent<PlayerJump>();
        _playerDash = GetComponent<PlayerDash>();
        _playerAttack = GetComponent<PlayerAttack>();
        _playerWall = GetComponent<PlayerWall>();
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
    private void Start() => PlayCurrent();
    private void Update()
    {
        EvaluateState();
        var landing = Time.time < _landUntil;
        if (_currentState == PlayerState.Jump && !landing) UpdateJumpPhase();
        if (_currentState == PlayerState.Dash && !landing) UpdateDashPhase();
    }
    public void ChangeState(PlayerState newState)
    {
        if (_currentState == newState) return;
        if (_currentState == PlayerState.Jump) _jumpPhase = 2;
        _currentState = newState;
        PlayCurrent();
    }
    private void PlayStateClip(string clipName)
    {
        _currentClip = clipName;
        _animator.Play(clipName);
    }
    private void PlayPhaseClip(string clipName)
    {
        if (_currentClip == clipName) return;
        _currentClip = clipName;
        _animator.Play(clipName);
    }
    private void UpdateDashPhase()
    {
        var dashDuration = _playerDash.DashDuration;
        var elapsed = Time.time - _dashStartTime;
        if (elapsed < _dashStartDuration) PlayPhaseClip("DashStart");
        else if (elapsed >= Mathf.Max(_dashStartDuration, dashDuration - _dashEndDuration)) PlayPhaseClip("DashEnd");
        else PlayPhaseClip("DashLoop");
    }
    private void UpdateJumpPhase()
    {
        if (_rigidbody == null) return;
        if (_jumpPhase == 0)
        {
            PlayPhaseClip("JumpStart");
            _jumpStartTime = Time.time;
            _jumpPhase = 1;
            return;
        }
        if (_jumpPhase == 1)
        {
            if (Time.time - _jumpStartTime < _jumpStartDuration) return;
            _jumpPhase = 2;
        }
        var verticalVelocity = _rigidbody.linearVelocity.y;
        if (verticalVelocity > _apexVelocityRange) PlayPhaseClip("JumpRise");
        else if (verticalVelocity < -_apexVelocityRange) PlayPhaseClip("Fall");
        else PlayPhaseClip("JumpApex");
    }

    private void PlayCurrent()
    {
        switch (_currentState)
        {
            case PlayerState.Idle:
                PlayStateClip("Idle");
                break;
            case PlayerState.Run:
                PlayStateClip("Run");
                break;
            case PlayerState.Jump:
                PlayStateClip("JumpStart");
                break;
            case PlayerState.Dash:
                PlayStateClip("DashStart");
                break;
            case PlayerState.Attack:
                PlayStateClip("Attack");
                break;
            case PlayerState.Death:
                PlayStateClip("Death");
                break;
            case PlayerState.WallCling:
                PlayStateClip("WallCling");
                break;
            case PlayerState.WallClimb:
                PlayStateClip("WallClimb");
                break;
        }
    }
    private void EvaluateState()
    {
        if (_currentState == PlayerState.Death) return;
        if (_currentState == PlayerState.Attack && _playerAttack.IsAttacking) return;
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
            var stateCheck = _currentState == PlayerState.Run || _currentState == PlayerState.Jump || _currentState == PlayerState.Dash || _currentState == PlayerState.Attack || _currentState == PlayerState.WallCling || _currentState == PlayerState.WallClimb;
            if (stateCheck) ChangeState(PlayerState.Jump);
            return;
        }
        var moving = _playerMove != null ? _playerMove.IsMoving : Mathf.Abs(_moveInput.x) > 0.01f;
        ChangeState(moving ? PlayerState.Run : PlayerState.Idle);
    }
    private void HandleMoveInput(Vector2 direction) => _moveInput = direction;
    private void HandleJumpStarted()
    {
        _landUntil = 0f;
        ChangeState(PlayerState.Jump);
        _jumpPhase = 0;
    }
    private void HandleLanded()
    {
        if (_playerDash.IsDashing)
        {
            EvaluateState();
            return;
        }
        _landUntil = Time.time + _landDuration;
        if (_currentState == PlayerState.Attack) _playerAttack.CancelAttack();
        PlayPhaseClip("Land");
        EvaluateState();
    }
    private void HandleGroundLost()
    {
        // 드롭스루 등으로 가만히 서 있다가 공중이 되면 Jump로 전환한다.
        if (_currentState == PlayerState.Idle || _currentState == PlayerState.Run) ChangeState(PlayerState.Jump);
    }
    private void HandleDashStarted()
    {
        _landUntil = 0f;
        ChangeState(PlayerState.Dash);
        _dashStartTime = Time.time;
    }
    private void HandleDashEnded() => EvaluateState();
    private void HandleAttackStarted()
    {
        _landUntil = 0f;
        ChangeState(PlayerState.Attack);
    }
    private void HandleAttackEnded() => EvaluateState();
}