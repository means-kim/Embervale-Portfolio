using UnityEngine;

public class EliteMonsterMovement : MonoBehaviour
{
    [Header("몬스터 이동")]
    [SerializeField] private float _moveSpeed = 7f;

    [SerializeField] private float _stopDistance = 1f;

    [Header("이동 가능한 월드 X 범위")]
    [SerializeField] private float _minX = 190f;
    [SerializeField] private float _maxX = 251.5f;

    [Header("플레이어 감지")]
    [SerializeField] private float _detectDistance = 40f;

    [SerializeField] private float _loseDistance = 45f;

    private GameObject _player;
    private Animator _animator;
    private bool _isChasing;

    public bool IsChasing => _isChasing;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (_player == null || !_player.activeInHierarchy)
        {
            _isChasing = false;
            _animator.SetBool("IsWalk", false);
            return;
        }

        Vector2 direction =
            (Vector2)_player.transform.position - (Vector2)transform.position;

        float distance = direction.magnitude;

        UpdateChaseState(distance);

        if (!_isChasing)
        {
            _animator.SetBool("IsWalk", false);
            return;
        }

        Move(direction, distance);
    }

    private void UpdateChaseState(float distance)
    {
        if (!_isChasing)
        {
            if (distance <= _detectDistance)
            {
                _isChasing = true;
            }
        }
        else
        {
            float loseDistance = Mathf.Max(_detectDistance, _loseDistance);

            if (distance > loseDistance)
            {
                _isChasing = false;
            }
        }
    }

    private void Move(Vector2 direction, float distance)
    {
        float horizontalDistance = Mathf.Abs(direction.x);
        bool shouldMove = horizontalDistance > _stopDistance;

        _animator.SetBool("IsWalk", shouldMove);

        if (!shouldMove) return;

        float directionX = Mathf.Sign(direction.x);

        Vector3 nextPosition = transform.position;
        nextPosition.x += directionX * _moveSpeed * Time.deltaTime;
        nextPosition.x = Mathf.Clamp(nextPosition.x, Mathf.Min(_minX, _maxX), Mathf.Max(_minX, _maxX));
        transform.position = nextPosition;
    }

    public void ChargeMovement(float speed)
    {
        _moveSpeed += speed;
    }
}
