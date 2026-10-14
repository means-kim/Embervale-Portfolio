using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerJumpPhaseAnimator))]
[RequireComponent(typeof(PlayerChargeJumpVisual))]
public class PlayerAnimator : MonoBehaviour
{
    [FormerlySerializedAs("_jumpStartDuration")]
    [SerializeField, Min(0f), Tooltip("공중 점프 회전 표시 시간")] private float _doubleJumpDuration = 0.2f;
    [SerializeField, Min(0f)] private float _apexVelocityRange = 1.5f;
    [SerializeField, Min(0f), Tooltip("미세 들뜸 깜빡임 방지")] private float _apexMinAirTime = 0.15f;

    private static readonly Dictionary<PlayerState, string> SimpleClips = new Dictionary<PlayerState, string>
    {
        { PlayerState.Idle, "Idle" },
        { PlayerState.Run, "Run" },
        { PlayerState.Dash, "Dash" },
        { PlayerState.Death, "Death" },
        { PlayerState.Hit, "Hit" },
        { PlayerState.WallCling, "WallCling" },
        { PlayerState.Glide, "Glide" },
    };

    private Animator _animator;
    private Rigidbody2D _body;
    private PlayerAttack _attack;
    private PlayerController _controller;
    private PlayerJumpPhaseAnimator _jumpPhase;
    private PlayerChargeJumpVisual _chargeVisual;
    private string _currentClip;

    private void Awake()
    {
        PlayerRig rig = PlayerRig.Resolve(gameObject);
        _animator = rig.Animator;
        _body = rig.Body;
        _attack = rig.Attack;
        _controller = rig.Controller;
        _jumpPhase = GetComponent<PlayerJumpPhaseAnimator>() ?? gameObject.AddComponent<PlayerJumpPhaseAnimator>();
        _chargeVisual = GetComponent<PlayerChargeJumpVisual>() ?? gameObject.AddComponent<PlayerChargeJumpVisual>();
        _jumpPhase.Configure(_doubleJumpDuration, _apexVelocityRange, _apexMinAirTime);
    }

    private void OnValidate()
    {
        var phase = GetComponent<PlayerJumpPhaseAnimator>();
        if (phase != null)
        {
            phase.Configure(_doubleJumpDuration, _apexVelocityRange, _apexMinAirTime);
        }
    }

    public void PlayState(PlayerState state, bool isDoubleJump)
    {
        if (_chargeVisual.IsActive)
        {
            if (state == PlayerState.Death || state == PlayerState.Hit)
            {
                _chargeVisual.End();
                Play(SimpleClips[state]);
                ApplyPose();
                return;
            }
            else
            {
                PlayPhase("Dash");
                return;
            }
        }
        if (state == PlayerState.Jump)
        {
            _jumpPhase.EnterJump(_body.linearVelocity.y);
            _currentClip = null;
            return;
        }
        if (state == PlayerState.Attack)
        {
            Play(_attack.CurrentAttackClip);
            return;
        }
        if (SimpleClips.TryGetValue(state, out string clipName))
        {
            Play(clipName);
            return;
        }
        throw new ArgumentOutOfRangeException(nameof(state));
    }

    public void BeginJump(bool isDoubleJump)
    {
        if (_chargeVisual.IsActive)
        {
            return;
        }
        _jumpPhase.Begin(isDoubleJump);
        _currentClip = null;
    }

    public void ResetJumpPhase() => _jumpPhase.Reset();

    public void PlayLand()
    {
        if (!_chargeVisual.IsActive)
        {
            PlayPhase("Idle");
        }
    }

    public void PlayDirectionalDash(Vector2 direction) => PlayChargeJumpLaunch(direction);
    public void EndDirectionalDash() => EndChargeJumpLaunch();

    public void PlayChargeJumpLaunch(Vector2 direction)
    {
        Play("Dash");
        ApplyPose();
        _chargeVisual.Begin(direction);
    }

    public void EndChargeJumpLaunch()
    {
        if (!_chargeVisual.IsActive)
        {
            return;
        }
        _chargeVisual.End();
        _jumpPhase.Reset();
        if (_controller.CurrentState != PlayerState.Jump)
        {
            PlayState(_controller.CurrentState, false);
        }
        else
        {
            _jumpPhase.UpdatePhase(_body.linearVelocity.y);
            _currentClip = null;
        }
        ApplyPose();
    }

    public SpriteRenderer GetChargeJumpVisualRenderer(SpriteRenderer original) => _chargeVisual.Resolve(original);

    public void Tick(PlayerState state, bool landingLocked, float verticalVelocity)
    {
        if (_chargeVisual.IsActive)
        {
            PlayPhase("Dash");
            return;
        }
        if (landingLocked)
        {
            return;
        }
        if (state == PlayerState.Jump)
        {
            _jumpPhase.UpdatePhase(verticalVelocity);
            _currentClip = null;
        }
        else if (state == PlayerState.Dash)
        {
            PlayPhase("Dash");
        }
    }

    private void OnDisable()
    {
        if (_chargeVisual != null)
        {
            _chargeVisual.End();
        }
        if (_animator != null)
        {
            _animator.speed = 1f;
        }
    }

    private void Play(string clipName)
    {
        _jumpPhase.ExitApex();
        if (_animator.speed != 1f)
        {
            _animator.speed = 1f;
        }
        _currentClip = clipName;
        _animator.Play(clipName, 0, 0f);
    }

    private void ApplyPose()
    {
        // 렌더러의 회전 전환과 새 스프라이트 적용을 같은 호출에서 끝낸다.
        if (_animator.isActiveAndEnabled && _animator.isInitialized)
        {
            _animator.Update(0f);
        }
    }

    private void PlayPhase(string clipName)
    {
        if (_currentClip == clipName)
        {
            return;
        }
        _jumpPhase.ExitApex();
        if (_animator.speed != 1f)
        {
            _animator.speed = 1f;
        }
        _currentClip = clipName;
        _animator.Play(clipName);
    }
}
