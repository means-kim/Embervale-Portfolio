using System;
using UnityEngine;

public class AirBossIcicleLifeTimer : MonoBehaviour
{
    [Header("고드름 생명주기")]
    [SerializeField] private float _icicleLifeTime = 5f;

    private AirBossAttack _attack;
    private PolygonCollider2D _collider;
    private Rigidbody2D _rigidbody;

    private float _elapsed;
    private bool _isRunning = false;

    private void Awake()
    {
        _attack = GetComponentInParent<AirBossAttack>();

        if (_attack != null)
        {
            _collider = _attack.GetComponent<PolygonCollider2D>();
            _rigidbody = _attack.GetComponent<Rigidbody2D>();
        }
    }

    private void OnEnable()
    {
        _elapsed = 0f;
        _isRunning = false;

        if (_attack == null || _collider == null || _rigidbody == null)
        {
            Debug.Log("고드름에 컴포넌트를 연결하세요");

            if (_attack != null)
            {
                _attack.Release();
            }

            return;
        }

        ResetVelocity();

        _collider.enabled = true;
        _isRunning = true;
    }

    private void Update()
    {
        if (!_isRunning) return;

        _elapsed += Time.deltaTime;

        if (_elapsed >= _icicleLifeTime)
        {
            Release();
        }
    }

    private void OnDisable()
    {
        _elapsed = 0f;
        _isRunning = false;

        if (_collider != null)
        {
            _collider.enabled = false;
        }

        ResetVelocity();
    }

    private void Release()
    {
        _isRunning = false;

        if (_collider != null)
        {
            _collider.enabled = false;
        }

        ResetVelocity();

        if (_attack != null)
        {
            _attack.Release();
        }
    }

    private void ResetVelocity()
    {
        if (_rigidbody == null) return;

        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0;
    }
}