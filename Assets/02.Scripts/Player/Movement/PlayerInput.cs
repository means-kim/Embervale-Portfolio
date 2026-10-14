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
    public event Action OnChargeJumpPressed;
    public event Action OnChargeJumpReleased;
    private PlayerChargeJump _chargeJump;
    private PlayerGlide _glide;
    private PlayerAttack _attack;
    private void Awake()
    {
        PlayerRig rig = PlayerRig.Resolve(gameObject);
        _chargeJump = rig.ChargeJump;
        _glide = rig.Glide;
        _attack = rig.Attack;
    }
    public Vector2 GetDashDirection(int facingDirection) => ResolveDashDirection(
        Input.GetKey(KeyCode.W),
        Input.GetKey(KeyCode.A),
        Input.GetKey(KeyCode.S),
        Input.GetKey(KeyCode.D),
        facingDirection);
    public static Vector2 ResolveDashDirection(bool up, bool left, bool down, bool right, int facingDirection)
    {
        float x = 0f;
        if (right)
        {
            x += 1f;
        }
        if (left)
        {
            x -= 1f;
        }

        float y = 0f;
        if (up)
        {
            y += 1f;
        }
        if (down)
        {
            y -= 1f;
        }

        var direction = new Vector2(x, y);
        if (direction.sqrMagnitude > 0f)
        {
            return direction.normalized;
        }

        if (facingDirection < 0)
        {
            return Vector2.left;
        }

        return Vector2.right;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            OnChargeJumpPressed?.Invoke();
        }
        if (Input.GetMouseButtonUp(1))
        {
            OnChargeJumpReleased?.Invoke();
        }
        if (_chargeJump.IsActive)
        {
            OnMoveInput?.Invoke(Vector2.zero);
            return;
        }
        OnMoveInput?.Invoke(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            _glide.TryToggleGlide();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            OnDashInput?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnDropInput?.Invoke();
        }
        CheckAttackInput();
        CheckClickVfx();
    }
    private void CheckAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _attack.RequestCombo(GetAttack(KeyCode.Alpha1), GetAttack(KeyCode.Alpha3),
                GetAttack(KeyCode.Alpha2));
        }
        foreach (var binding in _attackBindings)
        {
            if (binding == null || binding.attack == null || binding.key == KeyCode.None)
            {
                continue;
            }
            if (binding.key == KeyCode.Alpha1 || binding.key == KeyCode.Alpha2 || binding.key == KeyCode.Alpha3)
            {
                continue;
            }
            if (!Input.GetKeyDown(binding.key))
            {
                continue;
            }
            OnAttackInput?.Invoke(binding.attack);
            break;
        }
    }
    private PlayerAttackDataSO GetAttack(KeyCode key)
    {
        foreach (var binding in _attackBindings)
        {
            if (binding != null && binding.key == key)
            {
                return binding.attack;
            }
        }
        return null;
    }
    private void CheckClickVfx()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }
        var myCamera = Camera.main;
        if (myCamera == null)
        {
            return;
        }
        Vector3 worldPosition = myCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f;
        EffectManager.Instance?.PlayClick(worldPosition);
    }
}
