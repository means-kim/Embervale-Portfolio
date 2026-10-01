using UnityEngine;
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _jumpStartDuration = 0.13f;
    [SerializeField, Min(0f)] private float _apexVelocityRange = 1.5f;
    [SerializeField, Min(0f)] private float _dashStartDuration = 0.075f;
    [SerializeField, Min(0f)] private float _dashEndDuration = 0.13f;
    private Animator _animator;
    private string _currentClip;
    private int _jumpPhase = 2;
    private float _jumpStartTime;
    private float _dashStartTime;
    private void Awake() => _animator = GetComponent<Animator>();
    public void PlayState(PlayerState state, bool isDoubleJump)
    {
        switch (state)
        {
            case PlayerState.Idle:
                PlayClip("Idle");
                break;
            case PlayerState.Run:
                PlayClip("Run");
                break;
            case PlayerState.Jump:
                PlayClip(isDoubleJump ? "DoubleJump" : "JumpStart");
                break;
            case PlayerState.Dash:
                PlayClip("DashStart");
                break;
            case PlayerState.Attack:
                PlayClip("SideAttack");
                break;
            case PlayerState.Death:
                PlayClip("Death");
                break;
            case PlayerState.Hit:
                PlayClip("Hit");
                break;
            case PlayerState.WallCling:
                PlayClip("WallCling");
                break;
            case PlayerState.WallClimb:
                PlayClip("WallClimb");
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(state));
        }
    }
    public void BeginJump(bool isDoubleJump)
    {
        PlayPhaseClip(isDoubleJump ? "DoubleJump" : "JumpStart");
        _jumpPhase = 0;
    }
    public void ResetJumpPhase() => _jumpPhase = 2;
    public void BeginDash() => _dashStartTime = Time.time;
    public void PlayLand() => PlayPhaseClip("Land");
    public void PlayAttackClip(string clipName) => PlayPhaseClip(clipName);
    public void Tick(PlayerState state, bool landingLocked, float verticalVelocity, float dashDuration)
    {
        if (landingLocked) return;
        switch (state)
        {
            case PlayerState.Jump:
                UpdateJumpPhase(verticalVelocity);
                break;
            case PlayerState.Dash:
                UpdateDashPhase(dashDuration);
                break;
        }
    }
    private void UpdateJumpPhase(float verticalVelocity)
    {
        switch (_jumpPhase)
        {
            case 0:
                _jumpStartTime = Time.time;
                _jumpPhase = 1;
                return;
            case 1:
                if (Time.time - _jumpStartTime < _jumpStartDuration) return;
                _jumpPhase = 2;
                break;
        }
        if (verticalVelocity > _apexVelocityRange) PlayPhaseClip("JumpRise");
        else if (verticalVelocity < -_apexVelocityRange) PlayPhaseClip("Fall");
        else PlayPhaseClip("JumpApex");
    }
    private void UpdateDashPhase(float dashDuration)
    {
        var elapsed = Time.time - _dashStartTime;
        if (elapsed < _dashStartDuration) PlayPhaseClip("DashStart");
        else if (elapsed >= Mathf.Max(_dashStartDuration, dashDuration - _dashEndDuration)) PlayPhaseClip("DashEnd");
        else PlayPhaseClip("DashLoop");
    }
    private void PlayClip(string clipName)
    {
        _currentClip = clipName;
        _animator.Play(clipName);
    }
    private void PlayPhaseClip(string clipName)
    {
        if (_currentClip == clipName) return;
        _currentClip = clipName;
        _animator.Play(clipName);
    }
}
