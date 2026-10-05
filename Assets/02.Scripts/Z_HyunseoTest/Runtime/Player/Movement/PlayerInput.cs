using System;
using UnityEngine;
namespace HyunseoTest
{
    public class PlayerInput : MonoBehaviour
    {
        [Serializable]
        public class AttackBinding
        {
            public KeyCode key;
            public PlayerAttackDataSO attack;
        }
        [SerializeField] private AttackBinding[] _attackBindings = Array.Empty<AttackBinding>();
        [SerializeField] private PlayerAttackDataSO _upwardAttack;
        public AttackBinding[] AttackBindings => _attackBindings;
        public event Action<PlayerAttackDataSO> OnAttackInput;
        public event Action<Vector2> OnMoveInput;
        public event Action OnJumpInput;
        public event Action OnDashInput;
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
            CheckDropInput();
            CheckWallJumpInput();
            CheckAttackInput();
        }
        private void CheckAttackInput()
        {
            if (Input.GetMouseButtonDown(0))
                RequestPrimaryAttack(Input.GetKey(KeyCode.W));
            foreach (var binding in _attackBindings)
            {
                if (binding == null || binding.attack == null || binding.key == KeyCode.None) continue;
                if (binding.key == KeyCode.Alpha1 || binding.key == KeyCode.Alpha2 || binding.key == KeyCode.Alpha3) continue;
                if (!Input.GetKeyDown(binding.key)) continue;
                OnAttackInput?.Invoke(binding.attack);
                break;
            }
        }
        private void RequestPrimaryAttack(bool upwardHeld)
        {
            if (upwardHeld)
            {
                if (_upwardAttack != null) OnAttackInput?.Invoke(_upwardAttack);
                return;
            }
            GetComponent<PlayerAttack>().RequestCombo(GetAttack(KeyCode.Alpha1), GetAttack(KeyCode.Alpha2));
        }
        private PlayerAttackDataSO GetAttack(KeyCode key)
        {
            foreach (var binding in _attackBindings)
                if (binding != null && binding.key == key) return binding.attack;
            return null;
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
        private void CheckDropInput()
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) OnDropInput?.Invoke();
        }
        private void CheckWallJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space)) OnWallJumpInput?.Invoke();
        }
    }

}
