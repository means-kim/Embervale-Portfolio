using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public event Action<Vector2> OnMoveInput;
    private float _horizontal;
    private float _vertical;
    Vector2 _moveDirection = Vector2.zero;

    private void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        //_vertical = Input.GetAxisRaw("Vertical");
        _moveDirection = new Vector2(_horizontal, transform.position.y).normalized;
        OnMoveInput?.Invoke(_moveDirection);
    }
}
