using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField, Min(0)] private int _maxJumpCount = 2;
    [SerializeField] private List<int> _jumpPowers = new() { 40, 30 };
    [SerializeField, Min(0f)] private float _platformLandTolerance = 0.1f;
    [SerializeField, Min(0f), Tooltip("사선 미세 들뜸 깜빡임 방지")] private float _groundLossGrace = 0.1f;
    public event Action OnJumpStarted;
    public event Action OnLanded;
    public event Action OnGroundLost;
    private Rigidbody2D _rigidbody;
    private Collider2D _playerCollider;
    private PlayerInput _playerInput;
    private PlayerDash _playerDash;
    private PlayerWall _playerWall;
    private PlayerHit _playerHit;
    private PlayerAttack _playerAttack;
    private Health _health;
    private readonly HashSet<Collider2D> _groundContacts = new();
    private readonly HashSet<Collider2D> _platformContacts = new();
    private readonly List<Collider2D> _ignoredPlatforms = new();
    private float _dropTimer;
    private float _ungroundedTimer = -1f;
    private float _lastGroundedTime = -100f;
    private float _bufferedJumpTime = -100f;
    private float _dropThroughDuration = 0.4f;
    private float _coyoteTime = 0.1f;
    private float _jumpBufferTime = 0.1f;
    private bool _isGrounded;
    private bool _isJumping;
    private int _jumpCount;
    public bool IsGrounded => _isGrounded;
    public bool IsOnPlatform => _isGrounded && _platformContacts.Count > 0;
    public bool IsJumping => _isJumping;
    public int CurrentJumpCount => _jumpCount;
    public bool HasRemainingJumps => _jumpCount < _maxJumpCount;
    public float JumpSpeed => GetJumpPower(0);
    private void Awake()
    {
        PlayerRig rig = PlayerRig.Resolve(gameObject);
        _rigidbody = rig.Body;
        _playerCollider = rig.PlayerCollider;
        _playerInput = rig.Input;
        _playerDash = rig.Dash;
        _playerWall = rig.Wall;
        _playerHit = rig.Hit;
        _playerAttack = rig.Attack;
        _health = rig.Health;
    }
    private void OnEnable()
    {
        _playerInput.OnJumpInput += HandleJump;
        _playerInput.OnDropInput += HandleDrop;
    }
    private void OnDisable()
    {
        _playerInput.OnJumpInput -= HandleJump;
        _playerInput.OnDropInput -= HandleDrop;
        RestoreIgnoredPlatforms();
    }
    private void Update()
    {
        if (_dropTimer > 0f)
        {
            _dropTimer -= Time.deltaTime;
            if (_dropTimer <= 0f)
            {
                RestoreIgnoredPlatforms();
            }
        }
        if (_ungroundedTimer >= 0f)
        {
            _ungroundedTimer -= Time.deltaTime;
            if (_ungroundedTimer < 0f && _groundContacts.Count == 0)
            {
                SetGrounded(false);
            }
        }
        if (!_isGrounded && !_isJumping && _jumpCount == 0 && (Time.time - _lastGroundedTime) > Mathf.Max(0f, _coyoteTime))
        {
            _jumpCount = 1;
        }
    }
    private void HandleJump()
    {
        if (_playerWall.TryHandleJumpInput())
        {
            return;
        }
        _bufferedJumpTime = Time.time;
        TryPerformJump();
    }
    public void RefreshJumpsFromWall()
    {
        if (_isGrounded)
        {
            return;
        }
        _jumpCount = 0;
        _isJumping = true;
        _ungroundedTimer = -1f;
        _bufferedJumpTime = -100f;
    }
    public bool IsIgnoringPlatform(Collider2D platform) => _ignoredPlatforms.Contains(platform);
    public void ClearJumpBuffer() => _bufferedJumpTime = -100f;

    public void BeginExternalLaunch()
    {
        _groundContacts.Clear();
        _platformContacts.Clear();
        _ungroundedTimer = -1f;
        ClearJumpBuffer();
        _isGrounded = false;
        _isJumping = true;
        _jumpCount = Mathf.Max(1, _jumpCount);
        OnJumpStarted?.Invoke();
    }
    public void ResetForRespawn()
    {
        RestoreIgnoredPlatforms();
        _groundContacts.Clear();
        _platformContacts.Clear();
        _dropTimer = 0f;
        _ungroundedTimer = -1f;
        _lastGroundedTime = -100f;
        _bufferedJumpTime = -100f;
        _jumpCount = 0;
        _isGrounded = false;
        _isJumping = false;
    }
    private bool TryPerformJump()
    {
        if (!TryConsumeJump())
        {
            return false;
        }
        bool isDoubleJump = _jumpCount > 1;
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, GetJumpPower(_jumpCount - 1));
        if (isDoubleJump)
        {
            EffectManager.Instance?.PlayDoubleJump(transform.position);
        }
        else
        {
            EffectManager.Instance?.PlayJump(transform.position);
        }
        return true;
    }
    public bool TryConsumeJump()
    {
        if (_health.IsDead
            || _playerDash.IsDashing
            || _playerHit.IsHit
            || _jumpCount >= _maxJumpCount)
        {
            return false;
        }
        _ungroundedTimer = -1f;
        _groundContacts.Clear();
        _platformContacts.Clear();
        _isGrounded = false;
        _isJumping = true;
        _jumpCount++;
        _bufferedJumpTime = -100f;
        _playerAttack.ResetAirAttackCountOnJump();
        OnJumpStarted?.Invoke();
        return true;
    }
    private int GetJumpPower(int index)
    {
        if (_jumpPowers == null || _jumpPowers.Count == 0)
        {
            return 0;
        }
        return _jumpPowers[Mathf.Clamp(index, 0, _jumpPowers.Count - 1)];
    }
    private void HandleDrop()
    {
        if (_health.IsDead || !_isGrounded || _playerDash.IsDashing || _playerCollider == null || _platformContacts.Count == 0)
        {
            return;
        }
        foreach (var platform in _platformContacts)
        {
            if (platform == null)
            {
                continue;
            }
            Physics2D.IgnoreCollision(_playerCollider, platform, true);
            _groundContacts.Remove(platform);
            _ignoredPlatforms.Add(platform);
        }
        _platformContacts.Clear();
        _dropTimer = _dropThroughDuration;
        _ungroundedTimer = -1f;
        _isJumping = true;
        SetGrounded(_groundContacts.Count > 0);
    }
    private void RestoreIgnoredPlatforms()
    {
        foreach (var platform in _ignoredPlatforms)
        {
            if (_playerCollider == null || platform == null)
            {
                continue;
            }
            Physics2D.IgnoreCollision(_playerCollider, platform, false);
        }
        _ignoredPlatforms.Clear();
    }
    private void OnCollisionEnter2D(Collision2D collision) => TryLand(collision);
    private void OnCollisionStay2D(Collision2D collision) => TryLand(collision);
    private void OnCollisionExit2D(Collision2D collision) => RemoveGroundContact(collision.collider);
    private void RemoveGroundContact(Collider2D otherCollider)
    {
        if (otherCollider == null || !_groundContacts.Remove(otherCollider))
        {
            return;
        }
        _platformContacts.Remove(otherCollider);
        if (_groundContacts.Count > 0)
        {
            _ungroundedTimer = -1f;
        }
        else if (_isGrounded)
        {
            _ungroundedTimer = _groundLossGrace;
        }
    }
    private void TryLand(Collision2D collision)
    {
        if (collision.collider == null)
        {
            return;
        }
        var platform = collision.collider.CompareTag("Platform");
        if ((!platform && !collision.collider.CompareTag("Ground")) || (platform && (_playerCollider == null || IsIgnoringPlatform(collision.collider))))
        {
            RemoveGroundContact(collision.collider);
            return;
        }
        int contactCount = collision.contactCount;
        for (int i = 0; i < contactCount; i++)
        {
            var contact = collision.GetContact(i);
            if (contact.normal.y <= 0.5f || Vector2.Dot(_rigidbody.linearVelocity, contact.normal) > 0.01f)
            {
                continue;
            }
            if (platform && contact.point.y > _playerCollider.bounds.center.y + _platformLandTolerance)
            {
                continue;
            }
            _ungroundedTimer = -1f;
            _groundContacts.Add(collision.collider);
            if (platform)
            {
                _platformContacts.Add(collision.collider);
            }
            SetGrounded(true);
            return;
        }
        RemoveGroundContact(collision.collider);
    }
    private void SetGrounded(bool grounded)
    {
        if (_isGrounded == grounded)
        {
            return;
        }
        _isGrounded = grounded;
        if (!grounded)
        {
            _lastGroundedTime = Time.time;
            OnGroundLost?.Invoke();
            return;
        }
        _jumpCount = 0;
        _isJumping = false;
        OnLanded?.Invoke();
        if (_bufferedJumpTime > 0f && (Time.time - _bufferedJumpTime) <= Mathf.Max(0f, _jumpBufferTime))
        {
            TryPerformJump();
        }
    }
}
