using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerDash))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField, Min(0)] private int _maxJumpCount = 2;
    [SerializeField] private List<int> _jumpPowers = new() { 7, 7, 7, 7, 7 };
    [SerializeField, Min(0f)] private float _dropThroughDuration = 0.4f;
    [SerializeField, Min(0f)] private float _platformLandTolerance = 0.1f;
    public event Action OnJumpStarted;
    public event Action OnLanded;
    public event Action OnGroundLost;
    private Rigidbody2D _rigidbody;
    private Collider2D _playerCollider;
    private PlayerInput _playerInput;
    private PlayerDash _playerDash;
    private readonly HashSet<Collider2D> _groundContacts = new();
    private readonly HashSet<Collider2D> _platformContacts = new();
    private readonly List<Collider2D> _ignoredPlatforms = new();
    private float _dropTimer;
    private bool _isGrounded;
    private bool _isJumping;
    private int _jumpCount;
    public bool IsGrounded => _isGrounded;
    public bool IsJumping => _isJumping;
    public int MaxJumpCount => _maxJumpCount;
    public IReadOnlyList<int> JumpPowers => _jumpPowers;
    public int CurrentJumpCount => _jumpCount;
    public float JumpSpeed => GetJumpPower(0);
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerCollider = GetComponent<Collider2D>();
        _playerInput = GetComponent<PlayerInput>();
        _playerDash = GetComponent<PlayerDash>();
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
        if (_dropTimer <= 0f) return;
        _dropTimer -= Time.deltaTime;
        if (_dropTimer > 0f) return;
        RestoreIgnoredPlatforms();
    }
    private void HandleJump()
    {
        if (_playerDash.IsDashing) return;
        if (_maxJumpCount <= 0 || _jumpCount >= _maxJumpCount) return;
        if (_isGrounded)
        {
            _groundContacts.Clear();
            _platformContacts.Clear();
            _isGrounded = false;
        }
        _isJumping = true;
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, GetJumpPower(_jumpCount));
        _jumpCount++;
        OnJumpStarted?.Invoke();
    }
    private int GetJumpPower(int index)
    {
        if (_jumpPowers == null || _jumpPowers.Count == 0) return 0;
        return _jumpPowers[Mathf.Clamp(index, 0, _jumpPowers.Count - 1)];
    }
    private void HandleDrop()
    {
        if (!_isGrounded || _playerDash.IsDashing || _playerCollider == null || _platformContacts.Count == 0) return;
        foreach (var platform in _platformContacts)
        {
            if (platform == null) continue;
            Physics2D.IgnoreCollision(_playerCollider, platform, true);
            _groundContacts.Remove(platform);
            _ignoredPlatforms.Add(platform);
        }
        _platformContacts.Clear();
        _dropTimer = _dropThroughDuration;
        SetGrounded(_groundContacts.Count > 0);
    }
    private void RestoreIgnoredPlatforms()
    {
        if (_playerCollider == null)
        {
            _ignoredPlatforms.Clear();
            return;
        }
        foreach (var platform in _ignoredPlatforms)
        {
            if (platform == null) continue;
            Physics2D.IgnoreCollision(_playerCollider, platform, false);
        }
        _ignoredPlatforms.Clear();
    }
    private void OnCollisionEnter2D(Collision2D collision) => TryLand(collision);
    private void OnCollisionStay2D(Collision2D collision) => TryLand(collision);
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider != null && _groundContacts.Remove(collision.collider))
        {
            _platformContacts.Remove(collision.collider);
            SetGrounded(_groundContacts.Count > 0);
        }
    }
    private void TryLand(Collision2D collision)
    {
        if (collision.collider == null) return;
        var platform = collision.collider.CompareTag("Platform");
        if (!platform && !collision.collider.CompareTag("Ground")) return;
        if (_rigidbody.linearVelocity.y > 0.01f) return;
        // 원웨이 플랫폼은 발이 윗면 위에 있을 때만 착지한다. 아래에서 통과 중이면 무시한다.
        if (platform && (_playerCollider == null || _playerCollider.bounds.min.y < collision.collider.bounds.max.y - _platformLandTolerance)) return;
        int contactCount = collision.contactCount;
        for (int i = 0; i < contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            if (contact.normal.y > 0.5f)
            {
                if (_groundContacts.Add(collision.collider))
                {
                    if (platform) _platformContacts.Add(collision.collider);
                    SetGrounded(true);
                }
                return;
            }
        }
    }
    private void SetGrounded(bool grounded)
    {
        if (_isGrounded == grounded) return;
        _isGrounded = grounded;
        if (!grounded)
        {
            OnGroundLost?.Invoke();
            return;
        }
        _jumpCount = 0;
        _isJumping = false;
        OnLanded?.Invoke();
    }
}