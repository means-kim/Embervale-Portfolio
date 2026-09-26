using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    private Animator _animator;
    private Vector3 _currentDirection;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _playerInput.OnMoveInput += UpdateAnimation;
    }
    private void OnDisable()
    {
        _playerInput.OnMoveInput -= UpdateAnimation;
    }
    private void UpdateAnimation(Vector2 direction)
    {
        //_animator.SetFloat("MoveX", direction.x);
        _animator.SetBool("IsMoving", direction.x != 0);
    }
}
