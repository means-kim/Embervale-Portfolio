using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _jumpSpeed = 7f;

    public event Action OnJumpStarted;
    public event Action OnLanded;

    private Rigidbody2D _rigidbody;
    private Vector2 _moveInput;
    private bool _isGrounded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _playerInput.OnMoveInput += HandleMove;
        _playerInput.OnJumpInput += HandleJump;
    }

    private void OnDisable()
    {
        _playerInput.OnMoveInput -= HandleMove;
        _playerInput.OnJumpInput -= HandleJump;
    }

    private void HandleMove(Vector2 direction)
    {
        _moveInput = direction;
    }

    private void HandleJump()
    {
        if (!_isGrounded)
        {
            return;
        }
        _isGrounded = false;
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpSpeed);
        OnJumpStarted?.Invoke();
    }
    private void FixedUpdate()
    {
        if (_moveInput.x != 0f)
        {
            transform.localScale = new Vector3(Mathf.Sign(_moveInput.x), 1f, 1f);
        }
        _rigidbody.linearVelocity = new Vector2(_moveInput.x * _moveSpeed, _rigidbody.linearVelocity.y);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryLand(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        TryLand(collision);
    }
    private void TryLand(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
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
                bool wasGrounded = _isGrounded;
                _isGrounded = true;
                if (!wasGrounded)
                {
                    OnLanded?.Invoke();
                }
                return;
            }
        }
    }
}