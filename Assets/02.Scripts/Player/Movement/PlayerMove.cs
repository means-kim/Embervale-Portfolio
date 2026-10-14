using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _moveSpeed = 5f;
    private Rigidbody2D _rigidbody;
    private Vector2 _moveInput;
    private readonly HashSet<Object> _movementLocks = new();
    public bool MovementLocked => (_health != null && _health.IsDead) || _movementLocks.Count > 0;
    public bool HasMovementLockOtherThan(Object source) => HasMovementLockOtherThan(source, null);
    public bool HasMovementLockOtherThan(Object source, Object additionalSource)
    {
        foreach (var owner in _movementLocks)
        {
            if (owner != source && owner != additionalSource)
            {
                return true;
            }
        }
        return false;
    }
    private int _facingDirection = 1;
    private PlayerInput _playerInput;
    private PlayerGlide _glide;
    private Health _health;
    public int FacingDirection => _facingDirection;
    public bool IsMoving => Mathf.Abs(_rigidbody.linearVelocity.x) > 0.01f;
    public bool HasMoveInput => Mathf.Abs(_moveInput.x) > 0.01f;
    private void Awake()
    {
        PlayerRig rig = PlayerRig.Resolve(gameObject);
        _rigidbody = rig.Body;
        _playerInput = rig.Input;
        _glide = rig.Glide;
        _health = rig.Health;
        if (transform.localScale.x < 0f)
        {
            _facingDirection = -1;
        }
        else
        {
            _facingDirection = 1;
        }
    }
    private void OnEnable() => _playerInput.OnMoveInput += HandleMove;
    private void OnDisable() => _playerInput.OnMoveInput -= HandleMove;
    private void HandleMove(Vector2 direction)
    {
        if (MovementLocked)
        {
            _moveInput = Vector2.zero;
            return;
        }
        _moveInput = direction;
        if (Mathf.Abs(_moveInput.x) <= 0.01f)
        {
            return;
        }
        SetFacingDirection(_moveInput.x);
    }
    public void SetFacingDirection(float horizontal)
    {
        if (Mathf.Abs(horizontal) <= 0.01f)
        {
            return;
        }
        if (horizontal > 0f)
        {
            _facingDirection = 1;
        }
        else
        {
            _facingDirection = -1;
        }
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * _facingDirection;
        transform.localScale = scale;
    }
    private void FixedUpdate()
    {
        if (_health.IsDead)
        {
            _moveInput = Vector2.zero;
            _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
            return;
        }
        if (MovementLocked)
        {
            return;
        }
        Vector2 velocity = _rigidbody.linearVelocity;
        velocity.x = _moveInput.x * _moveSpeed * _glide.HorizontalSpeedMultiplier;
        _rigidbody.linearVelocity = velocity;
    }
    public void ResetForRespawn() => _moveInput = Vector2.zero;
    public void SetMovementLocked(Object source, bool locked)
    {
        if (source == null)
        {
            return;
        }
        if (locked)
        {
            _movementLocks.Add(source);
        }
        else
        {
            _movementLocks.Remove(source);
        }
        if (locked)
        {
            _moveInput = Vector2.zero;
            _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
        }
    }
}
