using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerDash))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _jumpSpeed = 7f;
    public event Action OnJumpStarted;
    public event Action OnLanded;
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;
    private PlayerDash _playerDash;
    private readonly HashSet<Collider2D> _groundContacts = new();
    private bool _isGrounded;
    private bool _isJumping;
    public bool IsGrounded => _isGrounded;
    public bool IsJumping => _isJumping;
    public float JumpSpeed => _jumpSpeed;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _playerDash = GetComponent<PlayerDash>();
    }
    private void OnEnable() => _playerInput.OnJumpInput += HandleJump;
    private void OnDisable() => _playerInput.OnJumpInput -= HandleJump;
    private void HandleJump()
    {
        if (!_isGrounded || _playerDash.IsDashing) return;
        _groundContacts.Clear();
        _isGrounded = false;
        _isJumping = true;
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpSpeed);
        OnJumpStarted?.Invoke();
    }
    private void OnCollisionEnter2D(Collision2D collision) => TryLand(collision);
    private void OnCollisionStay2D(Collision2D collision) => TryLand(collision);
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider != null && _groundContacts.Remove(collision.collider)) SetGrounded(_groundContacts.Count > 0);
    }
    private void TryLand(Collision2D collision)
    {
        if (collision.collider == null || !collision.collider.CompareTag("Ground")) return;
        if (_rigidbody.linearVelocity.y > 0.01f) return;
        int contactCount = collision.contactCount;
        for (int i = 0; i < contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            if (contact.normal.y > 0.5f)
            {
                if (_groundContacts.Add(collision.collider)) SetGrounded(true);
                return;
            }
        }
    }
    private void SetGrounded(bool grounded)
    {
        if (_isGrounded == grounded) return;
        _isGrounded = grounded;
        if (!grounded) return;
        _isJumping = false;
        OnLanded?.Invoke();
    }
}