using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(100)]
[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput), typeof(PlayerMove), typeof(PlayerJump))]
public class PlayerChargeJump : MonoBehaviour
{
    private enum Phase { None, Aiming, Launching }

    [Header("투사체 선택")]
    [SerializeField, Min(0.1f)] private float _selectionRadius = 1.8f;
    [SerializeField] private LayerMask _projectileLayers = ~0;
    [SerializeField] private Camera _aimCamera;
    [SerializeField] private ChargeJumpArrow _arrowPrefab;
    [Header("조준과 발사")]
    [SerializeField, Range(0.01f, 1f)] private float _slowMotionScale = 0.1f;
    [SerializeField, Min(0.01f)] private float _playerLaunchSpeed = 18f;
    [SerializeField, Min(0.01f)] private float _projectileLaunchSpeed = 8f;
    [SerializeField, Min(0.02f)] private float _launchDuration = 0.2f;

    private Rigidbody2D _body;
    private PlayerInput _input;
    private PlayerMove _move;
    private PlayerJump _jump;
    private PlayerWall _wall;
    private PlayerDash _dash;
    private PlayerAttack _attack;
    private PlayerHit _hit;
    private Health _health;
    private ChargeJumpAim _aim;
    private ChargeJumpSession _session;
    private IceProjectile _target;
    private Phase _phase;
    private Vector2 _projectileVelocity;
    private float _launchRemaining;

    public bool IsAiming => _phase == Phase.Aiming;
    public bool IsLaunching => _phase == Phase.Launching;
    public bool IsActive => _phase != Phase.None;
    private Vector3 AimOrigin => _aim.Origin;
    private Vector3 ArrowOrigin => _target != null ? _target.Center : AimOrigin;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetTimeOwner() => ChargeJumpSession.ResetTimeOwner();

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInput>();
        _move = GetComponent<PlayerMove>();
        _jump = GetComponent<PlayerJump>();
        _wall = GetComponent<PlayerWall>();
        _dash = GetComponent<PlayerDash>();
        _attack = GetComponent<PlayerAttack>();
        _hit = GetComponent<PlayerHit>();
        _health = GetComponent<Health>();
        _aim = new ChargeJumpAim(transform, GetComponent<Collider2D>());
        _session = new ChargeJumpSession(this, _body, _move, _jump);
    }

    private void OnEnable()
    {
        _input.OnChargeJumpPressed += TryBegin;
        _input.OnChargeJumpReleased += Release;
    }

    private void Start()
    {
        if (_arrowPrefab != null) return;
        Debug.LogError("PlayerChargeJump: Arrow Prefab에 ChargeJumpArrow 프리팹을 연결하세요.", this);
        enabled = false;
    }

    private void OnDisable()
    {
        _input.OnChargeJumpPressed -= TryBegin;
        _input.OnChargeJumpReleased -= Release;
        Stop(true);
        _aim.Dispose();
    }

    private bool ControlsAvailable => _input.isActiveAndEnabled && _move.isActiveAndEnabled
        && _jump.isActiveAndEnabled && _body.simulated && _body.bodyType == RigidbodyType2D.Dynamic
        && (_health == null || !_health.IsDead) && (_hit == null || !_hit.IsHit);

    private void TryBegin()
    {
        if (IsActive || !ControlsAvailable || _arrowPrefab == null || !ChargeJumpSession.CanBegin
            || (_dash != null && _dash.IsDashing) || (_attack != null && _attack.IsAttacking)
            || _move.HasMovementLockOtherThan(_wall)
            || (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()))
        {
            return;
        }

        if (_aimCamera == null) _aimCamera = Camera.main;
        if (_aimCamera == null) return;
        IceProjectile target = _aim.FindNearest(_selectionRadius, _projectileLayers);
        if (target == null || !target.TryBeginCharge(this)) return;

        _target = target;
        _projectileVelocity = target.Velocity;
        _phase = Phase.Aiming;
        _session.Begin(_slowMotionScale);
        _aim.Begin(_arrowPrefab, _aimCamera, ArrowOrigin, _move.FacingDirection);
    }

    private void Update()
    {
        if (!IsActive) return;
        if (!ControlsAvailable || (_move.HasMovementLockOtherThan(this) && (_wall == null || !_wall.IsClinging)))
        {
            Stop(false);
            return;
        }
        if (IsAiming)
        {
            if (_target == null || !_target.isActiveAndEnabled || !_target.IsHeldBy(this)
                || _aimCamera == null || !_session.TimeUnchanged)
            {
                Stop(true);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Escape)) Stop(true);
            else if (!Input.GetMouseButton(1)) Release();
        }
        else
        {
            _launchRemaining -= Time.deltaTime;
            if (_launchRemaining <= 0f) Stop(false);
        }
    }

    private void LateUpdate() { if (IsAiming) _aim.Tick(_aimCamera, ArrowOrigin); }

    private void Release()
    {
        if (!IsAiming) return;
        if (!ControlsAvailable || _target == null || !_target.isActiveAndEnabled || !_target.IsHeldBy(this)
            || _aimCamera == null || !_session.TimeUnchanged)
        {
            Stop(false);
            return;
        }
        _aim.UpdateDirection(_aimCamera, ArrowOrigin);
        Vector3 projectileOrigin = _target.Center;
        if (!_target.ReleaseCharge(this, -_aim.Direction * _projectileLaunchSpeed, true))
        {
            Stop(false);
            return;
        }
        _target = null;
        _session.RestoreTime();
        _aim.End();
        _phase = Phase.Launching;
        _launchRemaining = _launchDuration;
        _session.Launch(_aim.Direction, _playerLaunchSpeed);
        EffectManager.Instance?.PlayChargeJumpLaunch(AimOrigin, _aim.Direction);
        EffectManager.Instance?.PlayChargeJumpProjectileLaunch(projectileOrigin, -_aim.Direction);
    }

    private void FixedUpdate() => _session.FixedTick(IsAiming, IsLaunching);

    private void OnCollisionEnter2D(Collision2D collision) => CheckBlockingCollision(collision);
    private void OnCollisionStay2D(Collision2D collision) => CheckBlockingCollision(collision);
    private void CheckBlockingCollision(Collision2D collision)
    {
        if (IsLaunching && _session.BlocksLaunch(collision, _aim.Direction)) Stop(false);
    }

    public void CancelChargeJump() => Stop(false);

    private void Stop(bool restoreVelocity)
    {
        if (!IsActive) return;
        bool wasAiming = IsAiming;
        bool wasLaunching = IsLaunching;
        if (_target != null) _target.ReleaseCharge(this, _projectileVelocity);
        _target = null;
        _session.RestoreTime();
        _phase = Phase.None;
        _aim.End();
        _session.End(wasAiming, wasLaunching, restoreVelocity);
    }

    private void OnApplicationFocus(bool hasFocus) { if (!hasFocus) Stop(true); }

    private void OnApplicationPause(bool paused) { if (paused) Stop(true); }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        var collider = GetComponent<Collider2D>();
        Gizmos.DrawWireSphere(collider != null ? collider.bounds.center : transform.position, _selectionRadius);
    }
}
