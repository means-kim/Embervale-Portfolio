using UnityEngine;
using UnityEngine.EventSystems;
[DefaultExecutionOrder(100)]
[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(PlayerWall))]
[RequireComponent(typeof(PlayerDash))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerHit))]
[RequireComponent(typeof(PlayerGlide))]
[RequireComponent(typeof(Health))]
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
    [Header("사용 조건과 비용")]
    [SerializeField, Tooltip("투사체가 없으면 플레이어 중심에서 조준하고 체력을 소모합니다.")]
    private bool _allowWithoutProjectile = true;
    [SerializeField, Min(0), Tooltip("투사체 없이 발사할 때만 소모. 남은 체력이 이 값 이하면 사용할 수 없습니다.")]
    private int _healthCost = 50;
    [SerializeField, Min(0f), Tooltip("모든 차지 점프의 발사 시점부터 적용되는 게임 시간 기준 쿨타임(초). 취소에는 적용하지 않습니다.")]
    private float _cooldown = 1f;
    private Rigidbody2D _body;
    private PlayerInput _input;
    private PlayerMove _move;
    private PlayerJump _jump;
    private PlayerWall _wall;
    private PlayerDash _dash;
    private PlayerAttack _attack;
    private PlayerHit _hit;
    private Health _health;
    private PlayerGlide _glide;
    private ChargeJumpAim _aim;
    private ChargeJumpSession _session;
    private IceProjectile _target;
    private Phase _phase;
    private Vector2 _projectileVelocity;
    private float _launchRemaining;
    private float _nextUseTime;
    private bool _withoutProjectile;
    public bool IsAiming => _phase == Phase.Aiming;
    public bool IsLaunching => _phase == Phase.Launching;
    public bool IsActive => _phase != Phase.None;
    private Vector3 AimOrigin => _aim.Origin;
    private Vector3 ArrowOrigin
    {
        get
        {
            if (_target != null)
            {
                return _target.Center;
            }
            return AimOrigin;
        }
    }
    private bool CanPayHealth => _healthCost <= 0 || (!_health.IsDead && _health.CurrentHealth > _healthCost);
    private bool AimTargetAvailable
    {
        get
        {
            if (_withoutProjectile)
            {
                return _allowWithoutProjectile && CanPayHealth;
            }
            return _target != null && _target.isActiveAndEnabled && _target.IsHeldBy(this);
        }
    }
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
        _glide = GetComponent<PlayerGlide>();
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
        if (_arrowPrefab != null)
        {
            return;
        }
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
        && !_health.IsDead && !_hit.IsHit;
    private void TryBegin()
    {
        if (IsActive || Time.time < _nextUseTime || !ControlsAvailable || _arrowPrefab == null || !ChargeJumpSession.CanBegin
            || _dash.IsDashing
            || _move.HasMovementLockOtherThan(_wall, _attack)
            || (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()))
        {
            return;
        }
        if (_aimCamera == null)
        {
            _aimCamera = Camera.main;
        }
        if (_aimCamera == null)
        {
            return;
        }
        IceProjectile target = _aim.FindNearest(_selectionRadius, _projectileLayers);
        bool shouldRejectTarget;
        if (target == null)
        {
            shouldRejectTarget = !_allowWithoutProjectile || !CanPayHealth;
        }
        else
        {
            shouldRejectTarget = !target.TryBeginCharge(this);
        }
        if (shouldRejectTarget)
        {
            return;
        }
        _attack.InterruptForMovement();
        _glide.CancelGlide();
        _target = target;
        _withoutProjectile = target == null;
        _projectileVelocity = Vector2.zero;
        if (target != null)
        {
            _projectileVelocity = target.Velocity;
        }
        _phase = Phase.Aiming;
        _session.Begin(_slowMotionScale);
        _aim.Begin(_arrowPrefab, _aimCamera, ArrowOrigin, _move.FacingDirection);
    }
    private void Update()
    {
        if (!IsActive)
        {
            return;
        }
        if (!ControlsAvailable || (_move.HasMovementLockOtherThan(this) && !_wall.IsClinging))
        {
            Stop(false);
            return;
        }
        if (IsAiming)
        {
            if (!AimTargetAvailable || _aimCamera == null || !_session.TimeUnchanged)
            {
                Stop(true);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Stop(true);
            }
            else if (!Input.GetMouseButton(1))
            {
                Release();
            }
        }
        else
        {
            _launchRemaining -= Time.deltaTime;
            if (_launchRemaining <= 0f)
            {
                Stop(false);
            }
        }
    }
    private void LateUpdate() { if (IsAiming) _aim.Tick(_aimCamera, ArrowOrigin); }

    private void Release()
    {
        if (!IsAiming)
        {
            return;
        }
        if (!ControlsAvailable || !AimTargetAvailable || _aimCamera == null || !_session.TimeUnchanged)
        {
            Stop(false);
            return;
        }
        _aim.UpdateDirection(_aimCamera, ArrowOrigin);
        Vector3 projectileOrigin = ArrowOrigin;
        if (!_withoutProjectile && !_target.ReleaseCharge(this, -_aim.Direction * _projectileLaunchSpeed, true))
        {
            Stop(false);
            return;
        }
        _target = null;
        _session.RestoreTime();
        _aim.End();
        _phase = Phase.Launching;
        _launchRemaining = _launchDuration;
        if (_withoutProjectile && _healthCost > 0)
        {
            _health.SpendHealth(_healthCost);
        }
        _nextUseTime = Time.time + Mathf.Max(0f, _cooldown);
        _session.Launch(_aim.Direction, _playerLaunchSpeed);
        _attack.ResetAirAttackCountOnChargeJump();
        EffectManager.Instance?.PlayChargeJumpLaunch(AimOrigin, _aim.Direction);
        if (!_withoutProjectile)
        {
            EffectManager.Instance?.PlayChargeJumpProjectileLaunch(projectileOrigin, -_aim.Direction);
        }
    }

    private void FixedUpdate() => _session.FixedTick(IsAiming, IsLaunching);

    private void OnCollisionEnter2D(Collision2D collision) => CheckBlockingCollision(collision);
    private void OnCollisionStay2D(Collision2D collision) => CheckBlockingCollision(collision);
    private void CheckBlockingCollision(Collision2D collision)
    {
        if (IsLaunching && _session.BlocksLaunch(collision, _aim.Direction))
        {
            Stop(false);
        }
    }

    public void CancelChargeJump() => Stop(false);

    private void Stop(bool restoreVelocity)
    {
        if (!IsActive)
        {
            return;
        }
        bool wasAiming = IsAiming;
        bool wasLaunching = IsLaunching;
        if (_target != null)
        {
            _target.ReleaseCharge(this, _projectileVelocity);
        }
        _target = null;
        _session.RestoreTime();
        _phase = Phase.None;
        _aim.End();
        _session.End(wasAiming, wasLaunching, restoreVelocity);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Stop(true);
        }
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            Stop(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        var collider = GetComponent<Collider2D>();
        Vector3 gizmoCenter = transform.position;
        if (collider != null)
        {
            gizmoCenter = collider.bounds.center;
        }
        Gizmos.DrawWireSphere(gizmoCenter, _selectionRadius);
    }
}
