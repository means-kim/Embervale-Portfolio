using System;
using UnityEngine;
public class PlayerInput : MonoBehaviour
{
    public event Action<Vector2> OnMoveInput;
    public event Action OnJumpInput;
    public event Action OnDashInput;
    public event Action OnAttackInput;
    private float _horizontal;
    private Vector2 _moveDirection = Vector2.zero;
    private void Update()
    {
        CheckHorizontalInput();
        CheckJumpInput();
        CheckDashInput();
        CheckAttackInput();
    }
    private void CheckHorizontalInput()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _moveDirection = new Vector2(_horizontal, 0f);
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
}
