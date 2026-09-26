using UnityEngine;
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private float _moveSpeed = 1f;

    private Vector3 _currentDirection;

    private void OnEnable()
    {
        _playerInput.OnMoveInput += HandleMove;
    }
    private void OnDisable()
    {
        _playerInput.OnMoveInput -= HandleMove;
    }
    private void HandleMove(Vector2 direction)
    {
        _currentDirection = direction;
    }
    private void FixedUpdate()
    {
        if (_currentDirection.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(_currentDirection.x), 1, 1);
        }
        transform.position += _currentDirection * (_moveSpeed * Time.fixedDeltaTime);
    }
}
