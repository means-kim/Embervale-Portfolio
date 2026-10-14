using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class PlayerJumpPhaseAnimator : MonoBehaviour
{
    private const float ApexProgressEpsilon = 0.02f;

    private Animator _animator;
    private float _doubleJumpDuration = 0.2f;
    private float _apexVelocityRange = 1.5f;
    private float _apexMinAirTime = 0.15f;

    private string _currentClip;
    private bool _playingDoubleJump;
    private float _doubleJumpStartTime;
    private bool _inApex;
    private float _lastApexProgress = -1f;
    private float _jumpEnterTime = -100f;
    private float? _cachedDoubleJumpSpeed;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Configure(float doubleJumpDuration, float apexVelocityRange, float apexMinAirTime)
    {
        _doubleJumpDuration = doubleJumpDuration;
        _apexVelocityRange = apexVelocityRange;
        _apexMinAirTime = apexMinAirTime;
        _cachedDoubleJumpSpeed = null;
    }

    public void EnterJump(float verticalVelocity)
    {
        _jumpEnterTime = Time.time;
        _playingDoubleJump = false;
        if (verticalVelocity < 0f)
        {
            Play("Fall");
        }
        else
        {
            Play("JumpRise");
        }
    }

    public void Begin(bool isDoubleJump)
    {
        _jumpEnterTime = Time.time;
        _playingDoubleJump = isDoubleJump;
        _doubleJumpStartTime = Time.time;
        _inApex = false;
        _lastApexProgress = -1f;
        if (isDoubleJump)
        {
            Play("DoubleJump");
        }
        else
        {
            Play("JumpRise");
        }
        if (isDoubleJump)
        {
            FitDoubleJumpSpeed();
        }
    }

    public void Reset()
    {
        _currentClip = null;
        _playingDoubleJump = false;
        ExitApex();
        if (_animator.speed != 1f)
        {
            _animator.speed = 1f;
        }
    }

    public void ExitApex()
    {
        if (!_inApex)
        {
            return;
        }
        _inApex = false;
        _lastApexProgress = -1f;
        if (_animator.speed == 0f)
        {
            _animator.speed = 1f;
        }
    }

    public void UpdatePhase(float verticalVelocity)
    {
        if (_playingDoubleJump)
        {
            float elapsed = Time.time - _doubleJumpStartTime;
            if (_doubleJumpDuration > 0f && elapsed < _doubleJumpDuration)
            {
                return;
            }
            _playingDoubleJump = false;
            _animator.speed = 1f;
            _currentClip = null;
        }
        if (verticalVelocity > _apexVelocityRange)
        {
            ExitApex();
            PlayPhase("JumpRise");
        }
        else if (verticalVelocity < -_apexVelocityRange)
        {
            ExitApex();
            PlayPhase("Fall");
        }
        else
        {
            if (_currentClip != null && Time.time - _jumpEnterTime < _apexMinAirTime)
            {
                return;
            }
            float progress = Mathf.Clamp01(Mathf.InverseLerp(_apexVelocityRange, -_apexVelocityRange, verticalVelocity));
            progress = Mathf.Min(progress, 0.9999f);
            if (!_inApex || _currentClip != "JumpApex")
            {
                _inApex = true;
                _currentClip = "JumpApex";
                _lastApexProgress = progress;
                _animator.speed = 0f;
                _animator.Play("JumpApex", 0, progress);
            }
            else if (Mathf.Abs(progress - _lastApexProgress) >= ApexProgressEpsilon)
            {
                _lastApexProgress = progress;
                _animator.Play("JumpApex", 0, progress);
            }
        }
    }

    private void Play(string clipName)
    {
        ExitApex();
        if (_animator.speed != 1f)
        {
            _animator.speed = 1f;
        }
        _currentClip = clipName;
        _animator.Play(clipName, 0, 0f);
    }

    private void PlayPhase(string clipName)
    {
        if (_currentClip == clipName)
        {
            return;
        }
        ExitApex();
        if (_animator.speed != 1f)
        {
            _animator.speed = 1f;
        }
        _currentClip = clipName;
        _animator.Play(clipName);
    }

    private void FitDoubleJumpSpeed()
    {
        if (_cachedDoubleJumpSpeed.HasValue)
        {
            _animator.speed = _cachedDoubleJumpSpeed.Value;
            return;
        }
        float speed = 1f;
        if (_doubleJumpDuration > 0f)
        {
            var controller = _animator.runtimeAnimatorController;
            if (controller != null)
            {
                foreach (var clip in controller.animationClips)
                {
                    if (clip != null && clip.length > 0f && clip.name.Contains("DoubleJump"))
                    {
                        speed = clip.length / _doubleJumpDuration;
                        break;
                    }
                }
            }
        }
        _cachedDoubleJumpSpeed = speed;
        _animator.speed = speed;
    }
}
