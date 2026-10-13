using UnityEngine;

public class SlimeBMovement : MonoBehaviour
{
    [Header("현재 위치에서 X 범위")]
    [SerializeField] private float _leftOffsetX = 3f;

    [SerializeField] private float _rightOffsetX = 3f;

    [Header("슬라임 이동 속도")]
    [SerializeField] private float _speed = 1f;

    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;

    private float _leftX;
    private float _rightX;
    private bool _isLeft = true;
    private bool _isMoving = false;
    private bool _initialized = false;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize()
    {
        if (_initialized) return;

        float startX = _rigidbody2D.position.x;
        _leftX = startX - _leftOffsetX;
        _rightX = startX + _rightOffsetX;

        _initialized = true;
    }

    public void StartMove()
    {
        _isMoving = true;

        // 원본은 왼쪽을 바라본다.
        _spriteRenderer.flipX = !_isLeft;
    }

    public void StopMove()
    {
        _isMoving = false;

        // 중력과 낙하는 유지하고 수평 이동만 정지
        _rigidbody2D.linearVelocity = new Vector2(0f, _rigidbody2D.linearVelocity.y);
    }

    public bool Move()
    {
        if (!_initialized || !_isMoving)
        {
            return false;
        }

        float direction = _isLeft ? -1 : 1;
        float targetX = _isLeft ? _leftX : _rightX;

        float remaining = (targetX - _rigidbody2D.position.x) * direction;

        if (remaining <= 0.01f)
        {
            StopMove();
            _isLeft = !_isLeft;
            return true;
        }

        float speed = Mathf.Min(_speed, remaining / Time.fixedDeltaTime);

        _rigidbody2D.linearVelocity = new Vector2(direction * speed, _rigidbody2D.linearVelocity.y);

        return false;
    }

    private void OnDisable()
    {
        if (_rigidbody2D != null)
        {
            StopMove();
        }
    }
}