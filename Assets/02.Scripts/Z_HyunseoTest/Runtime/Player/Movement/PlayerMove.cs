using UnityEngine;
using System.Collections.Generic;
namespace HyunseoTest
{
    [RequireComponent(typeof(Rigidbody2D), (typeof(PlayerInput)))]
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _moveSpeed = 5f;
        private Rigidbody2D _rigidbody;
        private Vector2 _moveInput;
        private readonly HashSet<Object> _movementLocks = new();
        public bool MovementLocked => _movementLocks.Count > 0;
        private int _facingDirection = 1;
        private PlayerInput _playerInput;
        public int FacingDirection => _facingDirection;
        public bool IsMoving => Mathf.Abs(_rigidbody.linearVelocity.x) > 0.01f;
        public bool HasMoveInput => Mathf.Abs(_moveInput.x) > 0.01f;
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();
            _facingDirection = transform.localScale.x < 0f ? -1 : 1;
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
            if (!MovementLocked && !(GetComponent<PlayerAttack>()?.IsAttacking ?? false) && Mathf.Abs(_moveInput.x) > 0.01f)
            {
                _facingDirection = _moveInput.x > 0f ? 1 : -1;
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * _facingDirection;
                transform.localScale = scale;
            }
        }
        private void FixedUpdate()
        {
            if (MovementLocked) return;
            Vector2 velocity = _rigidbody.linearVelocity;
            velocity.x = _moveInput.x * _moveSpeed;
            _rigidbody.linearVelocity = velocity;
        }
        public void ResetForRespawn() => _moveInput = Vector2.zero;

        public void SetMovementLocked(Object source, bool locked)
        {
            if (source == null) return;
            if (locked) _movementLocks.Add(source);
            else _movementLocks.Remove(source);
            if (locked)
            {
                _moveInput = Vector2.zero;
                _rigidbody.linearVelocity = new Vector2(0f, _rigidbody.linearVelocity.y);
            }
        }
    }

}
