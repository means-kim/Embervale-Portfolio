using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerJump))]
[RequireComponent(typeof(PlayerMove))]
public class PlayerWall : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _climbSpeed = 3f;
    [SerializeField, Min(0f)] private float _slideSpeed = 1.5f;
    public bool IsClinging { get; private set; }
    public bool IsClimbing { get; private set; }
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;
    private PlayerJump _playerJump;
    private Vector2 _moveInput;
    private readonly Dictionary<Collider2D, float> _wallContacts = new();
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _playerJump = GetComponent<PlayerJump>();
    }
    private void OnEnable() => _playerInput.OnMoveInput += HandleMove;
    private void OnDisable() => _playerInput.OnMoveInput -= HandleMove;
    private void HandleMove(Vector2 direction) => _moveInput = direction;
    private void Update()
    {
        var pressingToward = 0f;
        if (Mathf.Abs(_moveInput.x) > 0.01f) pressingToward = Mathf.Sign(_moveInput.x);
        var touching = pressingToward != 0f && _wallContacts.ContainsValue(pressingToward);
        IsClinging = touching && !_playerJump.IsGrounded;
        IsClimbing = IsClinging && Mathf.Abs(_moveInput.y) > 0.01f;
    }
    private void FixedUpdate()
    {
        if (IsClimbing)
        {
            var velocity = _rigidbody.linearVelocity;
            velocity.y = _moveInput.y * _climbSpeed;
            _rigidbody.linearVelocity = velocity;
        }
        else if (IsClinging)
        {
            var velocity = _rigidbody.linearVelocity;
            velocity.y = Mathf.Max(velocity.y, -_slideSpeed);
            _rigidbody.linearVelocity = velocity;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) => TrackWall(collision);
    private void OnCollisionStay2D(Collision2D collision) => TrackWall(collision);
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider != null) _wallContacts.Remove(collision.collider);
    }
    private void TrackWall(Collision2D collision)
    {
        if (collision.collider == null || !collision.collider.CompareTag("Wall")) return;
        int contactCount = collision.contactCount;
        for (int i = 0; i < contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                // normal은 벽에서 플레이어 쪽을 향하므로 달라붙으려면 반대 방향을 눌러야 한다.
                _wallContacts[collision.collider] = -Mathf.Sign(contact.normal.x);
                return;
            }
        }
    }
}
