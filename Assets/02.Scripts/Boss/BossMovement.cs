using UnityEngine;

public class BossMovement : MonoBehaviour
{
    private float _horizontal;
    private float _vertical;
    private float _moveSpeed = 3f;

    private GameObject _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (_player == null) return;

        Move();
    }

    private void Move()
    {
        Vector3 direction = _player.transform.position - transform.position;
        direction.Normalize();

        transform.position += direction * _moveSpeed * Time.deltaTime;
    }
}