using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(50)]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGlide : MonoBehaviour
{
    [SerializeField, Min(1f)] private float _horizontalSpeedMultiplier = 1.5f;
    [SerializeField, Min(0.1f)] private float _maxFallSpeed = 5f;
    public bool IsGliding { get; private set; }
    public float HorizontalSpeedMultiplier
    {
        get
        {
            if (IsGliding)
            {
                return _horizontalSpeedMultiplier;
            }
            return 1f;
        }
    }
    private PlayerJump _jump;
    private PlayerMove _move;
    private PlayerDash _dash;
    private PlayerWall _wall;
    private PlayerHit _hit;
    private PlayerAttack _attack;
    private PlayerChargeJump _charge;
    private PlayerInput _input;
    private Health _health;
    private Rigidbody2D _body;
    private PlayerWallSensor _sensor;

    private void Awake()
    {
        PlayerRig rig = PlayerRig.Resolve(gameObject);
        _jump = rig.Jump;
        _move = rig.Move;
        _dash = rig.Dash;
        _wall = rig.Wall;
        _hit = rig.Hit;
        _attack = rig.Attack;
        _charge = rig.ChargeJump;
        _input = rig.Input;
        _health = rig.Health;
        _body = rig.Body;
        _sensor = new PlayerWallSensor(_body);
    }
    private void OnEnable()
    {
        _jump.OnJumpStarted += HandleJump;
        _jump.OnLanded += ResetForRespawn;
        _health.OnHit += CancelGlide;
        _health.OnDie += ResetForRespawn;
        GameSettings.GlideChanged += CancelGlide;
    }
    private void OnDisable()
    {
        _jump.OnJumpStarted -= HandleJump;
        _jump.OnLanded -= ResetForRespawn;
        _health.OnHit -= CancelGlide;
        _health.OnDie -= ResetForRespawn;
        GameSettings.GlideChanged -= CancelGlide;
        ResetForRespawn();
    }
    private void HandleJump()
    {
        CancelGlide();
    }
    private bool AreComponentsDisabled => !_input.isActiveAndEnabled || !_jump.isActiveAndEnabled || !_move.isActiveAndEnabled;
    private bool IsPhysicsNotReady => !_body.simulated || _body.bodyType != RigidbodyType2D.Dynamic;
    private bool IsGroundedOrLocked => _jump.IsGrounded || _move.MovementLocked;
    private bool IsDead => _health.IsDead;
    private bool IsInterrupted => _hit.IsHit || _dash.IsDashing;
    private bool IsOccupied => _wall.IsClinging || _attack.IsAttacking || _charge.IsActive;
    private bool Blocked => AreComponentsDisabled || IsPhysicsNotReady || IsGroundedOrLocked || IsDead || IsInterrupted || IsOccupied;
    private bool TouchingWall =>
        _sensor.HasWall(-1, 0.02f, 0.5f, _jump.IsIgnoringPlatform)
        || _sensor.HasWall(1, 0.02f, 0.5f, _jump.IsIgnoringPlatform);

    public bool TryToggleGlide()
    {
        if (!isActiveAndEnabled)
        {
            return false;
        }
        if (IsGliding)
        {
            CancelGlide();
            _jump.ClearJumpBuffer();
            return true;
        }
        return TryStartGlide();
    }

    public void HandleGlideInput(bool pressed, bool held)
    {
        if (GameSettings.GlideMode == GlideInputMode.Hold)
        {
            if (!held) CancelGlide();
            else if (pressed) TryStartGlide();
        }
        else if (pressed)
        {
            TryToggleGlide();
        }
    }

    private bool TryStartGlide()
    {
        if (!isActiveAndEnabled) return false;
        if (IsGliding) return true;
        if (_jump.IsGrounded)
        {
            return false;
        }
        if (_wall.IsClinging)
        {
            return false;
        }
        _jump.ClearJumpBuffer();
        if (!Blocked && !TouchingWall)
        {
            IsGliding = true;
            LimitFallSpeed();
            return true;
        }
        return false;
    }

    public bool TryHandleJumpInput() => TryToggleGlide();
    private void Update()
    {
        if (IsGliding && (Blocked || TouchingWall))
        {
            CancelGlide();
        }
    }
    private void FixedUpdate()
    {
        if (!IsGliding)
        {
            return;
        }
        if (Blocked || TouchingWall)
        {
            CancelGlide();
            return;
        }
        LimitFallSpeed();
    }
    private void LimitFallSpeed()
    {
        Vector2 velocity = _body.linearVelocity;
        float gravityStep = Physics2D.gravity.y * _body.gravityScale * Time.fixedDeltaTime;
        velocity.y = Mathf.Max(velocity.y, -_maxFallSpeed - Mathf.Min(0f, gravityStep));
        _body.linearVelocity = velocity;
    }
    private void OnCollisionEnter2D(Collision2D collision) => HandleContact(collision);
    private void OnCollisionStay2D(Collision2D collision) => HandleContact(collision);
    private void HandleContact(Collision2D collision)
    {
        if (!IsGliding || _jump.IsIgnoringPlatform(collision.collider))
        {
            return;
        }
        if (!collision.collider.CompareTag("Ground") && !collision.collider.CompareTag("Platform"))
        {
            return;
        }
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            if (normal.y > 0.5f || Mathf.Abs(normal.x) > 0.5f)
            {
                CancelGlide();
                return;
            }
        }
    }
    public void CancelGlide() => IsGliding = false;

    public void ResetForRespawn()
    {
        CancelGlide();
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused)
        {
            CancelGlide();
        }
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            CancelGlide();
        }
    }
}
