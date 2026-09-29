using System;
using UnityEngine;
public class PlayerInput : MonoBehaviour
{
    public event Action<Vector2> OnMoveInput;
    public event Action OnJumpInput;
    public event Action OnDashInput;
    public event Action OnAttackInput;
    public event Action OnDropInput;
    public event Action OnWallJumpInput;
    private float _horizontal;
    private float _vertical;
    private Vector2 _moveDirection = Vector2.zero;
    private void Update()
    {
        CheckHorizontalInput();
        CheckJumpInput();
        CheckDashInput();
        CheckAttackInput();
        CheckDropInput();
        CheckWallJumpInput();
    }
    private void CheckHorizontalInput()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
        _moveDirection = new Vector2(_horizontal, _vertical);
        OnMoveInput?.Invoke(_moveDirection);
    }
    private void CheckJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) OnJumpInput?.Invoke();
    }
    private void CheckDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) OnDashInput?.Invoke();
    }
    private void CheckAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0)) OnAttackInput?.Invoke();
    }
    private void CheckDropInput()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) OnDropInput?.Invoke();
    }
    private void CheckWallJumpInput()
    {
        // 벽점프는 스페이스바 전용. W·↑는 등반 입력으로만 쓴다.
        if (Input.GetKeyDown(KeyCode.Space)) OnWallJumpInput?.Invoke();
    }
}
