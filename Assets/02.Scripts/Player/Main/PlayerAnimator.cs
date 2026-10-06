using UnityEngine;
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _jumpStartDuration = 0.13f;
    [SerializeField, Min(0f)] private float _apexVelocityRange = 1.5f;
    [SerializeField, Min(0f), Tooltip("미세 들뜸 깜빡임 방지")] private float _apexMinAirTime = 0.15f;
    private Animator _animator;
    private string _currentClip;
    private int _jumpPhase = 2;
    private float _jumpStartTime;
    private float _jumpEnterTime = -100f;
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
                PlayClip("Dash");
                break;
            case PlayerState.Attack:
                PlayClip(GetComponent<PlayerAttack>().CurrentAttackClip);
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
    public void PlayLand() => PlayPhaseClip("Land");
    public void Tick(PlayerState state, bool landingLocked, float verticalVelocity)
    {
        if (landingLocked) return;
        switch (state)
        {
            case PlayerState.Jump:
                UpdateJumpPhase(verticalVelocity);
                break;
            case PlayerState.Dash:
                PlayPhaseClip("Dash");
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
            if (Time.time - _jumpEnterTime < _apexMinAirTime) return;
            PlayPhaseClip("JumpApex");
        }
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
