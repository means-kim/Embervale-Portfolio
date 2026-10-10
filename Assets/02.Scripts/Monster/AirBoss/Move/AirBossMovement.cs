using UnityEngine;

public class AirBossMovement : MonoBehaviour
{
    [Header("월드 좌표 범위(씬 별 설정 필요)")]
    [SerializeField] private Vector2 _maxPosition = new Vector2(30f, 10f);

    [SerializeField] private Vector2 _minPosition = new Vector2(30f, -5f);

    [SerializeField] private float _speed = 5f;

    private Vector2 _direction;

    public Vector2 Direction => _direction;

    public void StartMove()
    {
        RandomSelect();
    }

    public void Move()
    {
        Vector3 nextPosition = transform.position + (Vector3)(_direction * _speed * Time.deltaTime);

        // 경계에서는 방향과 애니메이션을 유지하고 위치만 제한한다.
        nextPosition.x = Mathf.Clamp(nextPosition.x, _minPosition.x, _maxPosition.x);
        nextPosition.y = Mathf.Clamp(nextPosition.y, _minPosition.y, _maxPosition.y);

        transform.position = nextPosition;
    }

    public void StopMove()
    {
        _direction = Vector2.zero;
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
        Vector2 position = transform.position;

        // 바깥쪽 방향의 가중치만 제외하고 기존 비율(9:9:3:2)은 유지한다.
        int leftWeight = position.x > _minPosition.x ? 9 : 0;
        int rightWeight = position.x < _maxPosition.x ? 9 : 0;
        int upWeight = position.y < _maxPosition.y ? 9 : 0;
        int downWeight = position.y > _minPosition.y ? 9 : 0;
        int totalWeight = leftWeight + rightWeight + upWeight + downWeight;

        if (totalWeight == 0)
        {
            StopMove();
            return;
        }

        int number = Random.Range(0, totalWeight);

        if (number < leftWeight)
        {
            LeftMove();
        }
        else if (number < leftWeight + rightWeight)
        {
            RightMove();
        }
        else if (number < leftWeight + rightWeight + upWeight)
        {
            UpMove();
        }
        else
        {
            DownMove();
        }
    }
}