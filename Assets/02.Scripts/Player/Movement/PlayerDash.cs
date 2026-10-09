using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerMove))]
[RequireComponent(typeof(PlayerWall))]
public class PlayerDash : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _dashSpeed = 10f;
    [SerializeField, Min(0.05f)] private float _dashDuration = 0.3f;
    [SerializeField, Min(0f)] private float _dashCooldown = 0.15f;
    public event Action OnDashStarted;
    public event Action OnDashEnded;
    public bool IsDashing { get; private set; }
    public float DashDuration => _dashDuration;
    private PlayerInput _playerInput;
    private PlayerMove _playerMove;
    private PlayerWall _playerWall;
    private PlayerHit _playerHit;
    private PlayerAttack _playerAttack;
    private PlayerJump _playerJump;
    private PlayerAnimator _playerAnimator;
    private Rigidbody2D _rigidbody;
    private float _dashTimeRemaining;
    private float _cooldownTimeRemaining;
    private float _originalGravityScale;
    private Vector2 _dashDirection = Vector2.right;
    private bool _airDashVisual;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _playerMove = GetComponent<PlayerMove>();
        _playerWall = GetComponent<PlayerWall>();
        _playerHit = GetComponent<PlayerHit>();
        _playerAttack = GetComponent<PlayerAttack>();
        _playerJump = GetComponent<PlayerJump>();
        _playerAnimator = GetComponent<PlayerAnimator>();
    }
    private void OnEnable() => _playerInput.OnDashInput += HandleDash;
    private void OnDisable()
    {
        _playerInput.OnDashInput -= HandleDash;
        EndDash();
    }
    private void Update()
    {
        if (_cooldownTimeRemaining > 0f) _cooldownTimeRemaining = Mathf.Max(0f, _cooldownTimeRemaining - Time.deltaTime);
        if (!IsDashing) return;
        _dashTimeRemaining -= Time.deltaTime;
        if (_dashTimeRemaining <= 0f) EndDash();
    }
    private void FixedUpdate()
    {
        if (!IsDashing) return;
        _rigidbody.linearVelocity = _dashDirection * _dashSpeed;
    }
    private void HandleDash()
    {
        if (IsDashing || _cooldownTimeRemaining > 0f || _playerWall.IsClinging) return;
        var health = GetComponent<Health>();
        if (health != null && health.IsDead) return;
        if (_playerHit != null && _playerHit.IsHit) return;
        _playerAttack?.InterruptForDash();
        _airDashVisual = _playerJump != null && !_playerJump.IsGrounded;
        _dashDirection = _airDashVisual ? _playerInput.GetDashDirection(_playerMove.FacingDirection)
            : Vector2.right * _playerMove.FacingDirection;
        _playerMove.SetFacingDirection(_dashDirection.x);
        _originalGravityScale = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 0f;
        _playerMove?.SetMovementLocked(this, true);
        _rigidbody.linearVelocity = _dashDirection * _dashSpeed;
        IsDashing = true;
        _dashTimeRemaining = _dashDuration;
        _cooldownTimeRemaining = _dashDuration + _dashCooldown;
        if (_airDashVisual) EffectManager.Instance?.PlayDash(transform.position, _dashDirection);
        else EffectManager.Instance?.PlayDash(transform.position, _playerMove.FacingDirection);
        OnDashStarted?.Invoke();
        if (_airDashVisual) _playerAnimator?.PlayDirectionalDash(_dashDirection);
    }
    public void CancelDash() => EndDash();
    public void ResetForRespawn()
    {
        EndDash();
        _dashTimeRemaining = 0f;
        _cooldownTimeRemaining = 0f;
    }
    private void EndDash()
    {
        if (!IsDashing) return;
        IsDashing = false;
        _rigidbody.gravityScale = _originalGravityScale;
        _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
        _playerMove?.SetMovementLocked(this, false);
        OnDashEnded?.Invoke();
        if (_airDashVisual) _playerAnimator?.EndDirectionalDash();
        _airDashVisual = false;
    }
}
