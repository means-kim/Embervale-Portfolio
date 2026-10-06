using UnityEngine;

public class AirBossAttackBezierMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _curveAmount = 3f;

    private Vector2 _p0;
    private Vector2 _p1;
    private Vector2 _p2;

    private float _t;
    private bool _isMoving = false;

    private Vector2 _targetPosition;
    private bool _isPrepared;

    private void Update()
    {
        if (!_isMoving) return;

        Move();
    }

    public void StartMove(Vector2 targetPosition)
    {
        _p0 = transform.position;
        _p2 = targetPosition;

        _p1 = (_p0 + _p2) / 2f + Vector2.up * _curveAmount;

        _t = 0f;
        _isMoving = true;
    }

    private void Move()
    {
        // 현재 위치에서의 베지어 곡선 진행 방향
        Vector2 derivative = 2f * (1f - _t) * (_p1 - _p0) + 2f * _t * (_p2 - _p1);

        float curveSpeed = derivative.magnitude;

        if (curveSpeed > 0f)
        {
            _t += (_moveSpeed / curveSpeed) * Time.deltaTime;
        }

        _t = Mathf.Clamp01(_t);

        Vector2 start = Vector2.Lerp(_p0, _p1, _t);
        Vector2 end = Vector2.Lerp(_p1, _p2, _t);

        transform.position = Vector2.Lerp(start, end, _t);

        if (_t >= 1f)
        {
            transform.position = _p2;
            _isMoving = false;
        }
    }

    public void PrepareMove(Vector2 targetPosition)
    {
        _targetPosition = targetPosition;
        _isPrepared = true;
        _isMoving = false;
        _t = 0f;
    }

    public void StartPreparedMove()
    {
        if (!_isPrepared) return;

        _isPrepared = false;
        StartMove(_targetPosition);
    }

    private void OnDisable()
    {
        _isMoving = false;
        _isPrepared = false;
        _t = 0f;
    }
}