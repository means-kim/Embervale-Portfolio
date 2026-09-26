using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField, Min(0f)] private float _moveSpeed = 5f;

    private Rigidbody2D _rigidbody;
    private Vector2 _moveInput;
    private bool _movementLocked;
    private int _facingDirection = 1;

    public int FacingDirection => _facingDirection;
    public bool IsMoving => _rigidbody != null && Mathf.Abs(_rigidbody.linearVelocity.x) > 0.01f;

    private void Reset()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        if (_playerInput == null)
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        _facingDirection = transform.localScale.x < 0f ? -1 : 1;

        if (_playerInput == null)
        {
            Debug.LogError("PlayerMove requires a PlayerInput component on the same GameObject.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (_playerInput != null)
        {
            _playerInput.OnMoveInput += HandleMove;
        }
    }

    private void OnDisable()
    {
        if (_playerInput != null)
        {
            _playerInput.OnMoveInput -= HandleMove;
        }
    }

    private void HandleMove(Vector2 direction)
    {
        _moveInput = direction;
    }

    private void FixedUpdate()
    {
        if (_movementLocked || _rigidbody == null)
        {
            return;
        }

        if (Mathf.Abs(_moveInput.x) > 0.01f)
        {
            _facingDirection = _moveInput.x > 0f ? 1 : -1;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * _facingDirection;
            transform.localScale = scale;
        }

        Vector2 velocity = _rigidbody.linearVelocity;
        velocity.x = _moveInput.x * _moveSpeed;
        _rigidbody.linearVelocity = velocity;
    }

    public void SetMovementLocked(bool locked)
    {
        _movementLocked = locked;
    }
}
