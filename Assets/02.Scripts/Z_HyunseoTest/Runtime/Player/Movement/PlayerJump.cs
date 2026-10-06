using System;
using System.Collections.Generic;
using UnityEngine;
namespace HyunseoTest
{
    [RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerDash))]
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField, Min(0)] private int _maxJumpCount = 2;
        [SerializeField] private List<int> _jumpPowers = new() { 7, 7, 7, 7, 7 };
        [SerializeField, Min(0f)] private float _dropThroughDuration = 0.4f;
        [SerializeField, Min(0f)] private float _platformLandTolerance = 0.1f;
        [SerializeField, Min(0f), Tooltip("사선 미세 들뜸 깜빡임 방지")] private float _groundLossGrace = 0.1f;
        [SerializeField, Min(0f), Tooltip("코요테 타임")] private float _coyoteTime = 0.1f;
        [SerializeField, Min(0f), Tooltip("점프 버퍼")] private float _jumpBufferTime = 0.15f;
        public event Action OnJumpStarted;
        public event Action OnLanded;
        public event Action OnGroundLost;
        private Rigidbody2D _rigidbody;
        private Collider2D _playerCollider;
        private PlayerInput _playerInput;
        private PlayerDash _playerDash;
        private PlayerWall _playerWall;
        private PlayerHit _playerHit;
        private readonly HashSet<Collider2D> _groundContacts = new();
        private readonly HashSet<Collider2D> _platformContacts = new();
        private readonly List<Collider2D> _ignoredPlatforms = new();
        private float _dropTimer;
        private float _ungroundedTimer = -1f;
        private float _lastGroundedTime = -100f;
        private float _bufferedJumpTime = -100f;
        private bool _isGrounded;
        private bool _isJumping;
        private int _jumpCount;
        public bool IsGrounded => _isGrounded;
        public bool IsOnPlatform => _isGrounded && _platformContacts.Count > 0;
        public bool IsJumping => _isJumping;
        public int MaxJumpCount => _maxJumpCount;
        public IReadOnlyList<int> JumpPowers => _jumpPowers;
        public int CurrentJumpCount => _jumpCount;
        public float JumpSpeed => GetJumpPower(0);
        public float CoyoteTime => _coyoteTime;
        public float JumpBufferTime => _jumpBufferTime;
        public bool IsCoyoteActive => !_isGrounded && !_isJumping && _jumpCount == 0 && _coyoteTime > 0f && (Time.time - _lastGroundedTime) <= _coyoteTime;
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerCollider = GetComponent<Collider2D>();
            _playerInput = GetComponent<PlayerInput>();
            _playerDash = GetComponent<PlayerDash>();
            _playerWall = GetComponent<PlayerWall>();
            _playerHit = GetComponent<PlayerHit>();
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
                if (_dropTimer <= 0f) RestoreIgnoredPlatforms();
            }
            if (_ungroundedTimer >= 0f)
            {
                _ungroundedTimer -= Time.deltaTime;
                if (_ungroundedTimer < 0f && _groundContacts.Count == 0) SetGrounded(false);
            }
            if (!_isGrounded && !_isJumping && _jumpCount == 0 && (Time.time - _lastGroundedTime) > Mathf.Max(0f, _coyoteTime)) _jumpCount = 1;
        }
        private void HandleJump()
        {
            _bufferedJumpTime = Time.time;
            if (_playerWall != null && _playerWall.IsClinging) return;
            TryPerformJump();
        }
        private bool TryPerformJump()
        {
            if (!TryConsumeJump()) return false;
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, GetJumpPower(_jumpCount - 1));
            return true;
        }
        public void RefreshJumpsFromWall() => _jumpCount = 0;
        public bool TryConsumeJump()
        {
            var health = GetComponent<global::Health>();
            if (health != null && health.IsDead) return false;
            if (_playerDash.IsDashing) return false;
            if (_playerHit != null && _playerHit.IsHit) return false;
            if (_maxJumpCount <= 0 || _jumpCount >= _maxJumpCount) return false;
            _ungroundedTimer = -1f;
            if (_isGrounded)
            {
                _groundContacts.Clear();
                _platformContacts.Clear();
                _isGrounded = false;
            }
            _isJumping = true;
            _jumpCount++;
            _bufferedJumpTime = -100f;
            OnJumpStarted?.Invoke();
            return true;
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
            _ungroundedTimer = -1f;
            SetGrounded(_groundContacts.Count > 0);
        }
        public void ResetForRespawn()
        {
            RestoreIgnoredPlatforms();
            _groundContacts.Clear();
            _platformContacts.Clear();
            _dropTimer = 0f;
            _ungroundedTimer = -1f;
            _lastGroundedTime = float.NegativeInfinity;
            _bufferedJumpTime = float.NegativeInfinity;
            _isGrounded = false;
            _isJumping = false;
            _jumpCount = 0;
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
                if (_groundContacts.Count > 0)
                {
                    _ungroundedTimer = -1f;
                    return;
                }
                if (_isGrounded) _ungroundedTimer = _groundLossGrace;
            }
        }
        private void TryLand(Collision2D collision)
        {
            if (collision.collider == null) return;
            var platform = collision.collider.CompareTag("Platform");
            if (!platform && !collision.collider.CompareTag("Ground")) return;
            if (_rigidbody.linearVelocity.y > 0.01f) return;
            if (platform && (_playerCollider == null || _playerCollider.bounds.min.y < collision.collider.bounds.max.y - _platformLandTolerance)) return;
            int contactCount = collision.contactCount;
            for (int i = 0; i < contactCount; i++)
            {
                ContactPoint2D contact = collision.GetContact(i);
                if (contact.normal.y > 0.5f)
                {
                    _ungroundedTimer = -1f;
                    if (_groundContacts.Add(collision.collider))
                    {
                        if (platform) _platformContacts.Add(collision.collider);
                    }
                    SetGrounded(true);
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
                _lastGroundedTime = Time.time;
                OnGroundLost?.Invoke();
                return;
            }
            _jumpCount = 0;
            _isJumping = false;
            OnLanded?.Invoke();
            if (_bufferedJumpTime > 0f && (Time.time - _bufferedJumpTime) <= Mathf.Max(0f, _jumpBufferTime)) TryPerformJump();
        }
    }

}
