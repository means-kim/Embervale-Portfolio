using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _dashSpeed = 10f;
    [SerializeField, Min(0.05f)] private float _dashDuration = 0.3f;
    [SerializeField, Min(0f)] private float _dashCooldown = 0.15f;
    [SerializeField, Min(0), Tooltip("한 번 공중에 뜬 뒤 착지하기 전까지 허용할 일반 대시 횟수. 0이면 공중 대시를 막습니다. 차지 점프는 별도입니다.")]
    private int _maxAirDashCount = 1;
    public event Action OnDashStarted;
    public event Action OnDashEnded;
    public bool IsDashing { get; private set; }
    private PlayerInput _playerInput;
    private PlayerMove _playerMove;
    private PlayerWall _playerWall;
    private PlayerHit _playerHit;
    private PlayerAttack _playerAttack;
    private PlayerJump _playerJump;
    private PlayerAnimator _playerAnimator;
    private PlayerGlide _glide;
    private Health _health;
    private Rigidbody2D _rigidbody;
    private float _dashTimeRemaining;
    private float _cooldownTimeRemaining;
    private float _originalGravityScale;
    private Vector2 _dashDirection = Vector2.right;
    private bool _airDashVisual;
    private int _airDashCount;
    private void Awake()
    {
        PlayerRig rig = PlayerRig.Resolve(gameObject);
        _rigidbody = rig.Body;
        _playerInput = rig.Input;
        _playerMove = rig.Move;
        _playerWall = rig.Wall;
        _playerHit = rig.Hit;
        _playerAttack = rig.Attack;
        _playerJump = rig.Jump;
        _playerAnimator = rig.PlayerAnimator;
        _glide = rig.Glide;
        _health = rig.Health;
    }
    private void OnEnable()
    {
        _playerInput.OnDashInput += HandleDash;
        _playerJump.OnLanded += ResetAirDashCount;
        if (_playerJump.IsGrounded)
        {
            ResetAirDashCount();
        }
    }
    private void OnDisable()
    {
        _playerInput.OnDashInput -= HandleDash;
        _playerJump.OnLanded -= ResetAirDashCount;
        EndDash();
    }
    private void Update()
    {
        if (_cooldownTimeRemaining > 0f)
        {
            _cooldownTimeRemaining = Mathf.Max(0f, _cooldownTimeRemaining - Time.deltaTime);
        }
        if (!IsDashing)
        {
            return;
        }
        _dashTimeRemaining -= Time.deltaTime;
        if (_dashTimeRemaining <= 0f)
        {
            EndDash();
        }
    }
    private void FixedUpdate()
    {
        if (!IsDashing)
        {
            return;
        }
        _rigidbody.linearVelocity = _dashDirection * _dashSpeed;
    }
    private void HandleDash()
    {
        if (IsDashing || _cooldownTimeRemaining > 0f || _playerWall.IsClinging)
        {
            return;
        }
        if (_health.IsDead)
        {
            return;
        }
        if (_playerHit.IsHit)
        {
            return;
        }
        bool airborne = !_playerJump.IsGrounded;
        if (airborne && _airDashCount >= Mathf.Max(0, _maxAirDashCount))
        {
            return;
        }
        _glide.CancelGlide();
        _playerAttack.InterruptForDash();
        _airDashVisual = airborne;
        if (airborne)
        {
            _airDashCount++;
        }
        if (_airDashVisual)
        {
            _dashDirection = _playerInput.GetDashDirection(_playerMove.FacingDirection);
        }
        else
        {
            _dashDirection = Vector2.right * _playerMove.FacingDirection;
        }
        _playerMove.SetFacingDirection(_dashDirection.x);
        _originalGravityScale = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 0f;
        _playerMove.SetMovementLocked(this, true);
        _rigidbody.linearVelocity = _dashDirection * _dashSpeed;
        IsDashing = true;
        if (airborne)
        {
            _playerAttack.ResetAirAttackCountOnAirDash();
        }
        _dashTimeRemaining = _dashDuration;
        _cooldownTimeRemaining = _dashDuration + _dashCooldown;
        if (_airDashVisual)
        {
            EffectManager.Instance?.PlayDash(transform.position, _dashDirection);
        }
        else
        {
            EffectManager.Instance?.PlayDash(transform.position, _playerMove.FacingDirection);
        }
        OnDashStarted?.Invoke();
        if (_airDashVisual)
        {
            _playerAnimator.PlayDirectionalDash(_dashDirection);
        }
    }
    public void CancelDash() => EndDash();
    public void ResetForRespawn()
    {
        EndDash();
        _dashTimeRemaining = 0f;
        _cooldownTimeRemaining = 0f;
        ResetAirDashCount();
    }
    private void ResetAirDashCount() => _airDashCount = 0;
    private void EndDash()
    {
        if (!IsDashing)
        {
            return;
        }
        IsDashing = false;
        _rigidbody.gravityScale = _originalGravityScale;
        _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
        _playerMove.SetMovementLocked(this, false);
        OnDashEnded?.Invoke();
        if (_airDashVisual)
        {
            _playerAnimator.EndDirectionalDash();
        }
        _airDashVisual = false;
    }
}
