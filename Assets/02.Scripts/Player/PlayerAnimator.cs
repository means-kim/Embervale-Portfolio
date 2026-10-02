using UnityEngine;
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _jumpStartDuration = 0.13f;
    [SerializeField, Min(0f)] private float _apexVelocityRange = 1.5f;
    [SerializeField, Min(0f), Tooltip("Jump 진입 후 이 시간이 지나야 JumpApex를 재생한다. 사선 미세 들뜸 깜빡임 방지용.")] private float _apexMinAirTime = 0.15f;
    [SerializeField, Min(0f)] private float _dashStartDuration = 0.075f;
    [SerializeField, Min(0f)] private float _dashEndDuration = 0.13f;
    private Animator _animator;
    private string _currentClip;
    private int _jumpPhase = 2;
    private float _jumpStartTime;
    private float _jumpEnterTime = -100f;
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
                _jumpEnterTime = Time.time;
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
        _jumpEnterTime = Time.time;
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
        if (verticalVelocity > _apexVelocityRange)
        {
            PlayPhaseClip("JumpRise");
        }
        else if (verticalVelocity < -_apexVelocityRange)
        {
            PlayPhaseClip("Fall");
        }
        else
        {
            // 공중 진입 직후의 미세한 속도 0 구간(사선 들뜸 등)에서는 Apex로 바로 넘기지 않고
            // 기존 클립(JumpStart 등)을 유지한다. Rise/Fall은 즉시 허용.
            if (Time.time - _jumpEnterTime < _apexMinAirTime) return;
            PlayPhaseClip("JumpApex");
        }
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
