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
    public event Action<PlayerAttackType> OnAttackTypeInput;
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
        CheckClickVfx();
        CheckAttackTypeInput();
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
        if (Input.GetKeyDown(KeyCode.Space)) OnJumpInput?.Invoke();
    }
    private void CheckDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) OnDashInput?.Invoke();
    }
    private void CheckAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) OnAttackInput?.Invoke();
    }
    private void CheckDropInput()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) OnDropInput?.Invoke();
    }
    private void CheckWallJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) OnWallJumpInput?.Invoke();
    }
    private void CheckClickVfx()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        var camera = Camera.main;
        if (camera == null) return;
        Vector3 worldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f;
        EffectManager.Instance?.PlayClick(worldPosition);
    }
    private void CheckAttackTypeInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) OnAttackTypeInput?.Invoke(PlayerAttackType.Stab);
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) OnAttackTypeInput?.Invoke(PlayerAttackType.Horizontal);
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) OnAttackTypeInput?.Invoke(PlayerAttackType.Down);
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) OnAttackTypeInput?.Invoke(PlayerAttackType.Up);
    }
}
