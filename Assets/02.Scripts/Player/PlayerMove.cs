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
    private void HandleMove(Vector2 dir)
    {
        _currentDirection = dir;
    }
    private void FixedUpdate()
    {
        transform.position += _currentDirection * (_moveSpeed * Time.fixedDeltaTime);
    }
}
