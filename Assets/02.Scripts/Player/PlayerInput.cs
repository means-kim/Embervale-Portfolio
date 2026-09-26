using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public event Action<Vector2> OnMoveInput;
    public event Action OnJumpInput;
    public event Action OnDashInput;

    private float _horizontal;
    private Vector2 _moveDirection = Vector2.zero;

    private void Update()
    {
        CheckHorizontalInput();
        CheckJumpInput();
        CheckDashInput();
    }

    private void CheckHorizontalInput()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _moveDirection = new Vector2(_horizontal, 0f);
        OnMoveInput?.Invoke(_moveDirection);
    }

    private void CheckJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnJumpInput?.Invoke();
            //Debug.Log("점프 입력 감지");
        }
    }

    private void CheckDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            OnDashInput?.Invoke();
        }
    }
}
