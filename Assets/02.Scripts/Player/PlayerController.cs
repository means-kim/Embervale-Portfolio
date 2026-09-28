using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private PlayerJump _playerJump;
    [SerializeField] private PlayerDash _playerDash;
    [SerializeField] private PlayerAttack _playerAttack;

    public PlayerState CurrentState => _currentState;

    private PlayerState _currentState = PlayerState.Idle;
    private Vector2 _moveInput;

    private void Reset()
    {
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _playerMove = GetComponent<PlayerMove>();
        _playerJump = GetComponent<PlayerJump>();
        _playerDash = GetComponent<PlayerDash>();
        _playerAttack = GetComponent<PlayerAttack>();
    }

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        if (_playerInput == null)
        {
            _playerInput = GetComponent<PlayerInput>();
        }
        if (_playerMove == null)
        {
            _playerMove = GetComponent<PlayerMove>();
        }
        if (_playerJump == null)
        {
            _playerJump = GetComponent<PlayerJump>();
        }
        if (_playerDash == null)
        {
            _playerDash = GetComponent<PlayerDash>();
        }
        if (_playerAttack == null)
        {
            _playerAttack = GetComponent<PlayerAttack>();
        }

        if (_animator == null)
        {
            Debug.LogError("PlayerController requires an Animator on the same GameObject.", this);
            enabled = false;
            return;
        }
        if (_playerInput == null)
        {
            Debug.LogError("PlayerController requires a PlayerInput component on the same GameObject.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (_playerInput != null)
        {
            _playerInput.OnMoveInput += HandleMoveInput;
        }
        if (_playerJump != null)
        {
            _playerJump.OnJumpStarted += HandleJumpStarted;
            _playerJump.OnLanded += HandleLanded;
        }
        if (_playerDash != null)
        {
            _playerDash.OnDashStarted += HandleDashStarted;
            _playerDash.OnDashEnded += HandleDashEnded;
        }
        if (_playerAttack != null)
        {
            _playerAttack.OnAttackStarted += HandleAttackStarted;
            _playerAttack.OnAttackEnded += HandleAttackEnded;
        }
    }

    private void OnDisable()
    {
        if (_playerInput != null)
        {
            _playerInput.OnMoveInput -= HandleMoveInput;
        }
        if (_playerJump != null)
        {
            _playerJump.OnJumpStarted -= HandleJumpStarted;
            _playerJump.OnLanded -= HandleLanded;
        }
        if (_playerDash != null)
        {
            _playerDash.OnDashStarted -= HandleDashStarted;
            _playerDash.OnDashEnded -= HandleDashEnded;
        }
        if (_playerAttack != null)
        {
            _playerAttack.OnAttackStarted -= HandleAttackStarted;
            _playerAttack.OnAttackEnded -= HandleAttackEnded;
        }
    }

    private void Start()
    {
        PlayCurrent();
    }

    private void Update()
    {
        EvaluateState();
    }

    public void ChangeState(PlayerState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;
        PlayCurrent();
    }

    private void PlayCurrent()
    {
        if (_animator == null) return;

        switch (_currentState)
        {
            case PlayerState.Idle:
                _animator.Play("Idle");
                break;
            case PlayerState.Run:
                _animator.Play("Run");
                break;
            case PlayerState.Jump:
                _animator.Play("JumpStart");
                break;
            case PlayerState.Dash:
                _animator.Play("DashStart");
                break;
            case PlayerState.Attack:
                _animator.Play("Attack");
                break;
            case PlayerState.Death:
                _animator.Play("Death");
                break;
        }
    }

    private void EvaluateState()
    {
        // Death의 종료 조건은 미정이므로 자동 전이에서 제외하고 외부 호출로만 빠져나간다.
        // Attack은 공격이 진행 중일 때만 유지하고, 종료된 뒤에는 일반 평가로 넘어간다.
        if (_currentState == PlayerState.Death)
        {
            return;
        }
        if (_currentState == PlayerState.Attack && _playerAttack != null && _playerAttack.IsAttacking)
        {
            return;
        }

        if (_playerDash != null && _playerDash.IsDashing)
        {
            ChangeState(PlayerState.Dash);
            return;
        }

        if (_playerJump != null && _playerJump.IsJumping)
        {
            ChangeState(PlayerState.Jump);
            return;
        }

        if (_playerJump != null && !_playerJump.IsGrounded)
        {
            // 발판 끝에서 걸어 떨어진 경우 등: 지상 상태나 공격 종료 후 공중으로 이어질 때 Jump로 전환.
            // 시작 직후 착지 전(Idle + 미접지)에는 Idle을 유지해 깜빡임을 막는다.
            bool stateCheck = _currentState == PlayerState.Run || _currentState == PlayerState.Jump
                              || _currentState == PlayerState.Dash || _currentState == PlayerState.Attack;
            if (stateCheck)
            {
                ChangeState(PlayerState.Jump);
            }
            return;
        }

        bool moving = _playerMove != null
            ? _playerMove.IsMoving
            : Mathf.Abs(_moveInput.x) > 0.01f;
        ChangeState(moving ? PlayerState.Run : PlayerState.Idle);
    }

    private void HandleMoveInput(Vector2 direction)
    {
        _moveInput = direction;
    }

    private void HandleJumpStarted()
    {
        ChangeState(PlayerState.Jump);
    }

    private void HandleLanded()
    {
        // 공중 공격 중 착지하면 공격을 캔슬하고 지상 상태로 복귀한다.
        if (_currentState == PlayerState.Attack && _playerAttack != null)
        {
            _playerAttack.CancelAttack();
            return;
        }
        EvaluateState();
    }

    private void HandleDashStarted()
    {
        ChangeState(PlayerState.Dash);
    }

    private void HandleDashEnded()
    {
        EvaluateState();
    }

    private void HandleAttackStarted()
    {
        ChangeState(PlayerState.Attack);
    }

    private void HandleAttackEnded()
    {
        EvaluateState();
    }
}