using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PlayerMove _playerMove;

    private Animator _animator;
    private bool _isMoving = false;
    private bool _isJumping = false;

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        if (_playerMove == null)
        {
            _playerMove = GetComponent<PlayerMove>();
        }
    }

    private void OnEnable()
    {
        _playerInput.OnMoveInput += UpdateAnimation;
        _playerMove.OnJumpStarted += JumpAnimation;
        _playerMove.OnLanded += LandAnimation;
    }
    private void OnDisable()
    {
        _playerInput.OnMoveInput -= UpdateAnimation;
        _playerMove.OnJumpStarted -= JumpAnimation;
        _playerMove.OnLanded -= LandAnimation;
    }

    private void JumpAnimation()
    {
        _isJumping = true;
    }
    private void LandAnimation()
    {
        _isJumping = false;
    }

    private void UpdateAnimation(Vector2 direction)
    {
        _isMoving = direction.x != 0f;
        _animator.SetBool("IsMoving", _isMoving);
        _animator.SetBool("IsJumping", _isJumping);
    }
}