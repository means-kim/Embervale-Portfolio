using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PlayerDash _playerDash;
    [SerializeField, Min(0f)] private float _jumpSpeed = 7f;

    public event Action OnJumpStarted;
    public event Action OnLanded;

    private Rigidbody2D _rigidbody;
    private readonly HashSet<Collider2D> _groundContacts = new HashSet<Collider2D>();
    private bool _isGrounded;
    private bool _isJumping;

    public bool IsGrounded => _isGrounded;
    public bool IsJumping => _isJumping;

    private void Reset()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerDash = GetComponent<PlayerDash>();
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        if (_playerInput == null)
        {
            _playerInput = GetComponent<PlayerInput>();
        }
        if (_playerDash == null)
        {
            _playerDash = GetComponent<PlayerDash>();
        }

        if (_playerInput == null)
        {
            Debug.LogError("PlayerJump requires a PlayerInput component on the same GameObject.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (_playerInput != null)
        {
            _playerInput.OnJumpInput += HandleJump;
        }
    }

    private void OnDisable()
    {
        if (_playerInput != null)
        {
            _playerInput.OnJumpInput -= HandleJump;
        }
    }

    private void HandleJump()
    {
        if (!_isGrounded || (_playerDash != null && _playerDash.IsDashing))
        {
            return;
        }

        _groundContacts.Clear();
        _isGrounded = false;
        _isJumping = true;
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpSpeed);
        OnJumpStarted?.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryLand(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        TryLand(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider != null && _groundContacts.Remove(collision.collider))
        {
            SetGrounded(_groundContacts.Count > 0);
        }
    }

    private void TryLand(Collision2D collision)
    {
        if (collision.collider == null || !collision.collider.CompareTag("Ground"))
        {
            return;
        }

        if (_rigidbody.linearVelocity.y > 0.01f)
        {
            return;
        }

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                if (_groundContacts.Add(collision.collider))
                {
                    SetGrounded(true);
                }
                return;
            }
        }
    }

    private void SetGrounded(bool grounded)
    {
        if (_isGrounded == grounded)
        {
            return;
        }

        _isGrounded = grounded;
        if (grounded)
        {
            _isJumping = false;
            OnLanded?.Invoke();
        }
    }
}
