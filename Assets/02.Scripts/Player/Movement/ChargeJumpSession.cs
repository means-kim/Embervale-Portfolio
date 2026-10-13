using UnityEngine;

internal sealed class ChargeJumpSession
{
    private static PlayerChargeJump _timeOwner;
    private readonly PlayerChargeJump _owner;
    private readonly Rigidbody2D _body;
    private readonly PlayerMove _move;
    private readonly PlayerJump _jump;
    private readonly PlayerAnimator _animator;
    private readonly PlayerDashGhost _dashGhost;
    private Vector2 _previousVelocity;
    private Vector2 _launchVelocity;
    private RigidbodyConstraints2D _previousConstraints;
    private float _previousGravity;
    private float _previousTimeScale;
    private float _previousFixedDeltaTime;
    private float _appliedTimeScale;
    private float _appliedFixedDeltaTime;

    public static bool CanBegin => _timeOwner == null && Time.timeScale > 0f && (HitImpactFeedback.Instance == null || !HitImpactFeedback.Instance.IsStopping);
    public bool TimeUnchanged => Mathf.Approximately(Time.timeScale, _appliedTimeScale);
    public static void ResetTimeOwner() => _timeOwner = null;

    public ChargeJumpSession(PlayerChargeJump owner, Rigidbody2D body, PlayerMove move, PlayerJump jump)
    {
        _owner = owner;
        _body = body;
        _move = move;
        _jump = jump;
        _animator = owner.GetComponent<PlayerAnimator>();
        _dashGhost = owner.GetComponent<PlayerDashGhost>();
    }

    public void Begin(float slowMotionScale)
    {
        _previousVelocity = _body.linearVelocity;
        _previousGravity = _body.gravityScale;
        _previousConstraints = _body.constraints;
        _previousTimeScale = Time.timeScale;
        _previousFixedDeltaTime = Time.fixedDeltaTime;
        _appliedTimeScale = _previousTimeScale * slowMotionScale;
        _appliedFixedDeltaTime = _previousFixedDeltaTime * slowMotionScale;
        _timeOwner = _owner;
        _move.SetMovementLocked(_owner, true);
        _jump.ClearJumpBuffer();
        _body.gravityScale = 0f;
        _body.constraints = RigidbodyConstraints2D.FreezeAll;
        _body.linearVelocity = Vector2.zero;
        Time.timeScale = _appliedTimeScale;
        Time.fixedDeltaTime = _appliedFixedDeltaTime;
        _appliedTimeScale = Time.timeScale;
        _appliedFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void Launch(Vector2 direction, float speed)
    {
        _body.constraints = _previousConstraints;
        _launchVelocity = direction * speed;
        _body.linearVelocity = _launchVelocity;
        _move.SetFacingDirection(direction.x);
        _jump.BeginExternalLaunch();
        _animator.PlayChargeJumpLaunch(direction);
        _dashGhost.BeginChargeJumpLaunch();
    }

    public void FixedTick(bool aiming, bool launching)
    {
        if (aiming)
        {
            _body.linearVelocity = Vector2.zero;
        }
        else if (launching)
        {
            _body.linearVelocity = _launchVelocity;
        }
    }

    public bool BlocksLaunch(Collision2D collision, Vector2 direction)
    {
        if (collision.collider == null || (!collision.collider.CompareTag("Ground") && !collision.collider.CompareTag("Platform")))
        {
            return false;
        }
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (Vector2.Dot(direction, collision.GetContact(i).normal) >= -0.25f)
            {
                continue;
            }
            return true;
        }
        return false;
    }

    public void End(bool wasAiming, bool wasLaunching, bool restoreVelocity)
    {
        if (wasLaunching)
        {
            _animator.EndChargeJumpLaunch();
            _dashGhost.EndChargeJumpLaunch();
        }
        if (wasAiming && _body.constraints == RigidbodyConstraints2D.FreezeAll)
        {
            _body.constraints = _previousConstraints;
        }
        if (Mathf.Approximately(_body.gravityScale, 0f))
        {
            _body.gravityScale = _previousGravity;
        }
        _move.SetMovementLocked(_owner, false);
        if (wasAiming && restoreVelocity)
        {
            _body.linearVelocity = _previousVelocity;
        }
    }

    public void RestoreTime()
    {
        if (_timeOwner != _owner)
        {
            return;
        }
        HitImpactFeedback.Instance?.ReplaceResumeTimeScale(_appliedTimeScale, _previousTimeScale);
        if (Mathf.Approximately(Time.timeScale, _appliedTimeScale))
        {
            Time.timeScale = _previousTimeScale;
        }
        if (Mathf.Approximately(Time.fixedDeltaTime, _appliedFixedDeltaTime))
        {
            Time.fixedDeltaTime = _previousFixedDeltaTime;
        }
        _timeOwner = null;
    }
}
