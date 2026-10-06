using System;
using UnityEngine;
public class PlayerInput : MonoBehaviour
{
    [Serializable]
    public class AttackBinding
    {
        public KeyCode key;
        public PlayerAttackDataSO attack;
    }
    [SerializeField] private AttackBinding[] _attackBindings = Array.Empty<AttackBinding>();
    public AttackBinding[] AttackBindings => _attackBindings;
    public event Action<PlayerAttackDataSO> OnAttackInput;
    public event Action<Vector2> OnMoveInput;
    public event Action OnJumpInput;
    public event Action OnDashInput;
    public event Action OnDropInput;
    private void Update()
    {
        OnMoveInput?.Invoke(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        if (Input.GetKeyDown(KeyCode.Space)) OnJumpInput?.Invoke();
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) OnDashInput?.Invoke();
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) OnDropInput?.Invoke();
        CheckAttackInput();
        CheckClickVfx();
    }
    private void CheckAttackInput()
    {
        foreach (var binding in _attackBindings)
        {
            if (binding == null || binding.attack == null || binding.key == KeyCode.None) continue;
            if (!Input.GetKeyDown(binding.key)) continue;
            OnAttackInput?.Invoke(binding.attack);
            break;
        }
    }
    private void CheckClickVfx()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        var myCamera = Camera.main;
        if (myCamera == null) return;
        Vector3 worldPosition = myCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f;
        EffectManager.Instance?.PlayClick(worldPosition);
    }
}
