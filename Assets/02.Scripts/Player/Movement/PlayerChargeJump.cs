using System.Collections.Generic;
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

    private static PlayerChargeJump _timeOwner;
    private readonly List<Collider2D> _nearby = new();
    private Rigidbody2D _body;
    private Collider2D _playerCollider;
    private PlayerInput _input;
    private PlayerMove _move;
    private PlayerJump _jump;
    private PlayerWall _wall;
    private PlayerDash _dash;
    private PlayerAttack _attack;
    private PlayerHit _hit;
    private Health _health;
    private ChargeJumpArrow _arrow;
    private IceProjectile _target;
    private Phase _phase;
    private Vector2 _aimDirection;
    private Vector2 _launchVelocity;
    private Vector2 _previousVelocity;
    private Vector2 _projectileVelocity;
    private RigidbodyConstraints2D _previousConstraints;
    private float _previousGravity;
    private float _previousTimeScale;
    private float _previousFixedDeltaTime;
    private float _appliedTimeScale;
    private float _appliedFixedDeltaTime;
    private float _launchRemaining;

    public bool IsAiming => _phase == Phase.Aiming;
    public bool IsLaunching => _phase == Phase.Launching;
    public bool IsActive => _phase != Phase.None;
    private Vector3 AimOrigin => _playerCollider != null ? _playerCollider.bounds.center : transform.position;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetTimeOwner() => _timeOwner = null;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _playerCollider = GetComponent<Collider2D>();
        _input = GetComponent<PlayerInput>();
        _move = GetComponent<PlayerMove>();
        _jump = GetComponent<PlayerJump>();
        _wall = GetComponent<PlayerWall>();
        _dash = GetComponent<PlayerDash>();
        _attack = GetComponent<PlayerAttack>();
        _hit = GetComponent<PlayerHit>();
        _health = GetComponent<Health>();
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
        if (_arrow != null) Destroy(_arrow.gameObject);
        _arrow = null;
    }

    private bool ControlsAvailable => _input.isActiveAndEnabled && _move.isActiveAndEnabled
        && _jump.isActiveAndEnabled && _body.simulated && _body.bodyType == RigidbodyType2D.Dynamic
        && (_health == null || !_health.IsDead) && (_hit == null || !_hit.IsHit);

    private void TryBegin()
    {
        if (IsActive || !ControlsAvailable || _arrowPrefab == null || _timeOwner != null || Time.timeScale <= 0f
            || (_dash != null && _dash.IsDashing) || (_attack != null && _attack.IsAttacking)
            || _move.HasMovementLockOtherThan(_wall)
            || (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()))
        {
            return;
        }

        if (_aimCamera == null) _aimCamera = Camera.main;
        if (_aimCamera == null) return;
        IceProjectile target = FindNearestProjectile();
        if (target == null || !target.TryBeginCharge(this)) return;

        _target = target;
        _projectileVelocity = target.Velocity;
        _previousVelocity = _body.linearVelocity;
        _previousGravity = _body.gravityScale;
        _previousConstraints = _body.constraints;
        _previousTimeScale = Time.timeScale;
        _previousFixedDeltaTime = Time.fixedDeltaTime;
        _appliedTimeScale = _previousTimeScale * _slowMotionScale;
        _appliedFixedDeltaTime = _previousFixedDeltaTime * _slowMotionScale;
        _timeOwner = this;
        _phase = Phase.Aiming;
        _move.SetMovementLocked(this, true);
        _jump.ClearJumpBuffer();
        _body.gravityScale = 0f;
        _body.constraints = RigidbodyConstraints2D.FreezeAll;
        _body.linearVelocity = Vector2.zero;
        Time.timeScale = _appliedTimeScale;
        Time.fixedDeltaTime = _appliedFixedDeltaTime;
        _aimDirection = Vector2.right * _move.FacingDirection;
        if (_arrow == null) _arrow = Instantiate(_arrowPrefab);
        UpdateAim();
        _arrow.Show(AimOrigin, _aimDirection);
    }

    private IceProjectile FindNearestProjectile()
    {
        var filter = new ContactFilter2D { useTriggers = true };
        filter.SetLayerMask(_projectileLayers);
        _nearby.Clear();
        Physics2D.OverlapCircle(AimOrigin, _selectionRadius, filter, _nearby);
        IceProjectile nearest = null;
        float bestDistance = float.PositiveInfinity;
        foreach (var collider in _nearby)
        {
            if (collider == null) continue;
            var projectile = collider.GetComponentInParent<IceProjectile>();
            if (projectile == null || !projectile.CanBeCharged) continue;
            float distance = ((Vector2)AimOrigin - collider.ClosestPoint(AimOrigin)).sqrMagnitude;
            if (distance >= bestDistance) continue;
            bestDistance = distance;
            nearest = projectile;
        }
        return nearest;
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
                || _aimCamera == null || !Mathf.Approximately(Time.timeScale, _appliedTimeScale))
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

    private void LateUpdate()
    {
        if (!IsAiming || _arrow == null) return;
        UpdateAim();
        _arrow.Show(AimOrigin, _aimDirection);
    }

    private void UpdateAim()
    {
        var plane = new Plane(Vector3.forward, AimOrigin);
        Ray ray = _aimCamera.ScreenPointToRay(Input.mousePosition);
        if (!plane.Raycast(ray, out float distance)) return;
        Vector2 direction = ray.GetPoint(distance) - AimOrigin;
        if (direction.sqrMagnitude > 0.0001f) _aimDirection = direction.normalized;
    }

    private void Release()
    {
        if (!IsAiming) return;
        if (!ControlsAvailable || _target == null || !_target.isActiveAndEnabled || !_target.IsHeldBy(this)
            || _aimCamera == null || !Mathf.Approximately(Time.timeScale, _appliedTimeScale))
        {
            Stop(false);
            return;
        }
        UpdateAim();
        if (!_target.ReleaseCharge(this, -_aimDirection * _projectileLaunchSpeed, true))
        {
            Stop(false);
            return;
        }
        _target = null;
        RestoreTime();
        _arrow.Hide();
        _body.constraints = _previousConstraints;
        _phase = Phase.Launching;
        _launchRemaining = _launchDuration;
        _launchVelocity = _aimDirection * _playerLaunchSpeed;
        _body.linearVelocity = _launchVelocity;
        _jump.BeginExternalLaunch();
    }

    private void FixedUpdate()
    {
        if (IsAiming)
        {
            _body.linearVelocity = Vector2.zero;
        }
        else if (IsLaunching)
        {
            _body.linearVelocity = _launchVelocity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) => CheckBlockingCollision(collision);
    private void OnCollisionStay2D(Collision2D collision) => CheckBlockingCollision(collision);
    private void CheckBlockingCollision(Collision2D collision)
    {
        if (!IsLaunching || collision.collider == null || (!collision.collider.CompareTag("Ground") && !collision.collider.CompareTag("Platform")))
        {
            return;
        }
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (Vector2.Dot(_aimDirection, collision.GetContact(i).normal) >= -0.25f) continue;
            Stop(false);
            return;
        }
    }

    public void CancelChargeJump() => Stop(false);

    private void Stop(bool restoreVelocity)
    {
        if (!IsActive) return;
        bool wasAiming = IsAiming;
        if (_target != null) _target.ReleaseCharge(this, _projectileVelocity);
        _target = null;
        RestoreTime();
        _phase = Phase.None;
        if (_arrow != null) _arrow.Hide();
        if (wasAiming && _body.constraints == RigidbodyConstraints2D.FreezeAll)
            _body.constraints = _previousConstraints;
        if (Mathf.Approximately(_body.gravityScale, 0f)) _body.gravityScale = _previousGravity;
        _move.SetMovementLocked(this, false);
        if (wasAiming && restoreVelocity) _body.linearVelocity = _previousVelocity;
    }

    private void RestoreTime()
    {
        if (_timeOwner != this) return;
        HitImpactFeedback.Instance?.ReplaceResumeTimeScale(_appliedTimeScale, _previousTimeScale);
        // 다른 일시정지 시스템이 덮어쓴 시간은 해제하지 않습니다.
        if (Mathf.Approximately(Time.timeScale, _appliedTimeScale)) Time.timeScale = _previousTimeScale;
        if (Mathf.Approximately(Time.fixedDeltaTime, _appliedFixedDeltaTime)) Time.fixedDeltaTime = _previousFixedDeltaTime;
        _timeOwner = null;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) Stop(true);
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) Stop(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        var collider = GetComponent<Collider2D>();
        Gizmos.DrawWireSphere(collider != null ? collider.bounds.center : transform.position, _selectionRadius);
    }
}
