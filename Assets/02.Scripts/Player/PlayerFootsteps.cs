using UnityEngine;
using UnityEngine.Serialization;
[RequireComponent(typeof(PlayerMove), typeof(PlayerJump), typeof(PlayerController))]
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
    private float _nextStepTime;
    private void Awake()
    {
        _move = GetComponent<PlayerMove>();
        _jump = GetComponent<PlayerJump>();
        _controller = GetComponent<PlayerController>();
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
        if (_jump != null) _jump.OnLanded += HandleLanded;
    }
    private void OnDisable()
    {
        if (_jump != null) _jump.OnLanded -= HandleLanded;
    }
    private void Update()
    {
        if (_controller.CurrentState != PlayerState.Run) return;
        if (!_jump.IsGrounded || !_move.IsMoving) return;
        if (Time.time < _nextStepTime) return;
        _nextStepTime = Time.time + _stepInterval;
        PlayCurrentSurface();
        EffectManager.Instance?.PlayFootstep(transform.position);
    }
    private void HandleLanded()
    {
        if (!_playOnLand) return;
        PlayCurrentSurface();
        _nextStepTime = Time.time + _stepInterval;
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
