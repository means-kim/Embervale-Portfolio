using UnityEngine;

public class EliteMonsterFacing : MonoBehaviour
{
    [SerializeField] private Transform _bossDirectionRoot;

    private Transform _target;
    private bool _isRightPosition;

    private void Awake()
    {
        _isRightPosition = _bossDirectionRoot.localScale.x < 0f;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            _target = player.transform;
        }
    }

    private void Update()
    {
        if (_target == null) return;

        UpdateFacing();
    }

    public void UpdateFacing()
    {
        float directionX = _target.position.x - transform.position.x;

        if (directionX > 0)
        {
            RightTurn();
        }
        else if (directionX < 0)
        {
            LeftTurn();
        }
    }

    private void RightTurn()
    {
        if (_isRightPosition) return;

        _isRightPosition = true;
        SetDirection(-1f);
    }

    private void LeftTurn()
    {
        if (!_isRightPosition) return;

        _isRightPosition = false;
        SetDirection(1f);
    }

    private void SetDirection(float direction)
    {
        Vector3 scale = _bossDirectionRoot.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;

        _bossDirectionRoot.localScale = scale;
    }
}