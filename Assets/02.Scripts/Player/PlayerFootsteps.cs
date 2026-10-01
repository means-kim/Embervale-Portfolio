using UnityEngine;
using UnityEngine.Serialization;
[RequireComponent(typeof(PlayerMove), typeof(PlayerJump), typeof(PlayerController))]
[RequireComponent(typeof(PlayerWall))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Surface Banks (AudioManager를 거치지 않음)")]
    [Tooltip("Ground 태그 위에서 재생. 기존 _bank 값은 여기로 자동 이전된다.")]
    [FormerlySerializedAs("_bank")][SerializeField] private FootstepSFXBank _groundBank;
    [Tooltip("Platform 태그 위에서 재생. EarthgroundSteps 뱅크를 넣는다.")]
    [SerializeField] private FootstepSFXBank _platformBank;

    [Header("Dedicated Sources (Player 전용, AudioManager와 별도)")]
    [Tooltip("Ground용. 비워 두면 Platform용과 공유하거나 자동 생성한다.")]
    [SerializeField] private AudioSource _groundSource;
    [Tooltip("Platform용. 비워 두면 Ground용과 공유하거나 자동 생성한다.")]
    [SerializeField] private AudioSource _platformSource;

    [SerializeField, Min(0.05f)] private float _stepInterval = 0.35f;
    [SerializeField] private bool _playOnLand;
    private PlayerMove _move;
    private PlayerJump _jump;
    private PlayerController _controller;
    private PlayerWall _wall;
    private Rigidbody2D _rigidbody;
    private float _nextStepTime;
    private bool _wasStepping;
    private void Awake()
    {
        _move = GetComponent<PlayerMove>();
        _jump = GetComponent<PlayerJump>();
        _controller = GetComponent<PlayerController>();
        _wall = GetComponent<PlayerWall>();
        _rigidbody = GetComponent<Rigidbody2D>();
        EnsureSources();
    }
    private void OnValidate()
    {
        // 에디터에서 할당 없이 테스트할 때도 소리가 나도록 미리보기용으로 보정한다.
        // 실제 AudioSource 추가는 Play 시점 Awake에서만 한다.
        if (_groundSource == null && _platformSource != null) _groundSource = _platformSource;
        else if (_platformSource == null && _groundSource != null) _platformSource = _groundSource;
    }
    private void EnsureSources()
    {
        if (_groundSource == null) _groundSource = GetComponent<AudioSource>();
        if (_platformSource == null) _platformSource = _groundSource;
        if (_groundSource == null)
        {
            _groundSource = gameObject.AddComponent<AudioSource>();
            _groundSource.playOnAwake = false;
            _groundSource.spatialBlend = 0f;
        }
        if (_platformSource == null) _platformSource = _groundSource;
    }
    private void OnEnable()
    {
        if (_jump != null)
        {
            _jump.OnLanded += HandleLanded;
            // 이륙 순간 즉시 발소리 예약/잔향을 끊는다. 상태·속도 신호가 한 프레임 늦게 바뀌는 동안 새 스텝이 잡히는 것을 방지.
            _jump.OnJumpStarted += HandleSteppingStopped;
            _jump.OnGroundLost += HandleSteppingStopped;
        }
    }
    private void OnDisable()
    {
        if (_jump != null)
        {
            _jump.OnLanded -= HandleLanded;
            _jump.OnJumpStarted -= HandleSteppingStopped;
            _jump.OnGroundLost -= HandleSteppingStopped;
        }
        StopFootstepSounds();
    }
    private void Update()
    {
        // 입력 의도(HasMoveInput)를 함께 본다. 정지 입력 후 물리 속도가 1~2프레임 늦게 0이 되거나,
        // 컨트롤러 상태가 한 프레임 늦게 Idle로 바뀌는 동안 예약된 스텝이 새로 잡히는 것이 밀림 체감의 원인.
        bool runStep = _controller.CurrentState == PlayerState.Run
            && _jump.IsGrounded
            && _move.IsMoving
            && _move.HasMoveInput;
        // 벽 타기 중 상하 이동(IsClimbing은 Y 입력 포함)은 Platform 발소리로 재생한다. 수동 슬라이딩(입력 없음)은 제외.
        bool wallStep = _wall != null && _wall.IsClimbing
            && _rigidbody != null && Mathf.Abs(_rigidbody.linearVelocity.y) > 0.01f;
        bool shouldStep = runStep || wallStep;
        if (!shouldStep)
        {
            if (_wasStepping)
            {
                _wasStepping = false;
                // 이미 깔린 PlayOneShot 잔향을 끊고 타이머를 리셋한다. 재개 시 묵은 타이머가 즉시 발화하지 않게.
                StopFootstepSounds();
                _nextStepTime = Time.time + _stepInterval;
            }
            return;
        }
        _wasStepping = true;
        if (Time.time < _nextStepTime) return;
        _nextStepTime = Time.time + _stepInterval;
        if (wallStep) PlayPlatformStepOnce();
        else PlayCurrentSurface();
        EffectManager.Instance?.PlayFootstep(transform.position);
    }
    private void HandleLanded()
    {
        if (!_playOnLand) return;
        PlayCurrentSurface();
        _nextStepTime = Time.time + _stepInterval;
    }
    /// <summary>점프 이륙·낙하 등으로 스테핑 조건이 깨지면 잔향과 예약을 즉시 정리.</summary>
    private void HandleSteppingStopped()
    {
        _wasStepping = false;
        StopFootstepSounds();
        _nextStepTime = Time.time + _stepInterval;
    }
    /// <summary>이미 재생 중인 발소리(PlayOneShot 잔향 포함)를 끊는다. 같은 소스 공유 시 중복 Stop 방지.</summary>
    private void StopFootstepSounds()
    {
        if (_groundSource != null) _groundSource.Stop();
        if (_platformSource != null && _platformSource != _groundSource) _platformSource.Stop();
    }
    /// <summary>현재 서 있는 표면(Ground/Platform)에 맞는 뱅크+소스로 한 발짝 재생.</summary>
    private void PlayCurrentSurface()
    {
        bool onPlatform = _jump != null && _jump.IsOnPlatform;
        FootstepSFXBank bank = onPlatform ? _platformBank : _groundBank;
        AudioSource source = onPlatform ? _platformSource : _groundSource;
        // 폴백: Platform 설정이 비었으면 Ground 설정을 그대로 쓴다 (무음 방지).
        if (bank == null || source == null)
        {
            bank = _groundBank != null ? _groundBank : _platformBank;
            source = _groundSource != null ? _groundSource : _platformSource;
        }
        if (bank == null || source == null) return;
        bank.PlayOn(source);
    }
    public void PlayStepOnce()
    {
        PlayCurrentSurface();
    }
    /// <summary>애니메이션 이벤트 등에서 표면을 지정해 재생할 때 사용.</summary>
    public void PlayGroundStepOnce()
    {
        if (_groundBank != null && _groundSource != null) _groundBank.PlayOn(_groundSource);
    }
    public void PlayPlatformStepOnce()
    {
        FootstepSFXBank bank = _platformBank != null ? _platformBank : _groundBank;
        AudioSource source = _platformSource != null ? _platformSource : _groundSource;
        if (bank != null && source != null) bank.PlayOn(source);
    }
}
