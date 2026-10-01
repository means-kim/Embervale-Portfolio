using UnityEngine;

public class AirBossMovement : MonoBehaviour
{
    [Header("월드 좌표 범위(씬 별 설정 필요)")]
    [SerializeField] private Vector2 _maxPosition = new Vector2(-10f, -5f);

    [SerializeField] private Vector2 _minPosition = new Vector2(10f, 10f);

    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private float _moveInterval = 3f;

    private Vector2 _direction;
    private float _timer;
    private bool _isMoving = false;
    private int _number = 0;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (!_isMoving)
        {
            if (_timer >= _moveInterval)
            {
                RandomSelect();
                _isMoving = true;
            }

            return;
        }

        if (_timer >= _moveDuration)
        {
            _isMoving = false;
            _timer = 0f;
            return;
        }

        Move();
    }

    private void Move()
    {
        Vector3 nextPosition = transform.position + (Vector3)(_direction * _speed * Time.deltaTime);

        if ((_direction.x < 0f && nextPosition.x <= _minPosition.x) ||
            (_direction.x > 0f && nextPosition.x >= _maxPosition.x) ||
            (_direction.y < 0f && nextPosition.y <= _minPosition.y) ||
            (_direction.y > 0f && nextPosition.y >= _maxPosition.y))
        {
            _direction = -_direction;
        }

        nextPosition.x = Mathf.Clamp(nextPosition.x, _minPosition.x, _maxPosition.x);
        nextPosition.y = Mathf.Clamp(nextPosition.y, _minPosition.y, _maxPosition.y);

        transform.position = nextPosition;
    }

    private void LeftMove()
    {
        _direction = Vector2.left;
    }

    private void RightMove()
    {
        _direction = Vector2.right;
    }

    private void UpMove()
    {
        _direction = Vector2.up;
    }

    private void DownMove()
    {
        _direction = Vector2.down;
    }

    private void RandomSelect()
    {
        _number = Random.Range(0, 23);

        if (_number < 9)
        {
            LeftMove();
        }
        else if (_number < 18)
        {
            RightMove();
        }
        else if (_number < 21)
        {
            UpMove();
        }
        else
        {
            DownMove();
        }

        _timer = 0f;
    }
}