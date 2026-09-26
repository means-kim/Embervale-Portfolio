using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private PlayerJump _playerJump;
    [SerializeField] private PlayerDash _playerDash;

    private Animator _animator;

    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
    private static readonly int IsDashingHash = Animator.StringToHash("IsDashing");

    private void Reset()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerJump = GetComponent<PlayerJump>();
        _playerDash = GetComponent<PlayerDash>();
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_playerMove == null)
        {
            _playerMove = GetComponent<PlayerMove>();
        }
        if (_playerJump == null)
        {
            _playerJump = GetComponent<PlayerJump>();
        }
        if (_playerDash == null)
        {
            _playerDash = GetComponent<PlayerDash>();
        }

        if (_playerMove == null || _playerJump == null || _playerDash == null)
        {
            Debug.LogError("PlayerAnimation requires PlayerMove, PlayerJump, and PlayerDash on the same GameObject.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (_animator == null || _playerMove == null || _playerJump == null || _playerDash == null)
        {
            return;
        }

        _animator.SetBool(IsMovingHash, _playerMove.IsMoving);
        _animator.SetBool(IsJumpingHash, _playerJump.IsJumping);
        _animator.SetBool(IsDashingHash, _playerDash.IsDashing);
    }
}
