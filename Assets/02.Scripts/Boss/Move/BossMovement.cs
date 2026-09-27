using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _stopDistance = 1f;

    private GameObject _player;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        if (_player == null)
        {
            Debug.Log($"{_player} : Palyer Tag가 붙은 오브젝트가 없습니다.");
        }
    }

    private void Update()
    {
        if (_player == null) return;

        Move();
    }

    private void Move()
    {
        Vector3 direction = _player.transform.position - transform.position;

        float distance = direction.magnitude;

        if (distance > _stopDistance)
        {
            direction.Normalize();

            _animator.SetBool("IsWalk", true);

            transform.position += direction * _moveSpeed * Time.deltaTime;
        }
        else
        {
            _animator.SetBool("IsWalk", false);
        }
    }

    public void ChargeMovement(float speed)
    {
        _moveSpeed += speed;
    }
}