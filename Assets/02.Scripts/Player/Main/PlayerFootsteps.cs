using UnityEngine;
using UnityEngine.Serialization;
[RequireComponent(typeof(PlayerMove), typeof(PlayerJump), typeof(PlayerController))]
[RequireComponent(typeof(PlayerWall))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Surface Banks")]
    [Tooltip("Ground 태그 위에서 재생")]
    [FormerlySerializedAs("_bank")][SerializeField] private FootstepSFXBank _groundBank;
    [Tooltip("Platform 태그 위에서 재생")]
    [SerializeField] private FootstepSFXBank _platformBank;
    [Header("Dedicated Sources")]
    [Tooltip("Ground용")]
    [SerializeField] private AudioSource _groundSource;
    [Tooltip("Platform용")]
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
        bool runStep = _controller.CurrentState == PlayerState.Run && _jump.IsGrounded && _move.IsMoving && _move.HasMoveInput;
        bool wallStep = _wall != null && _wall.IsClimbing && _rigidbody != null && Mathf.Abs(_rigidbody.linearVelocity.y) > 0.01f;
        bool shouldStep = runStep || wallStep;
        if (!shouldStep)
        {
            if (_wasStepping)
            {
                _wasStepping = false;
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
    private void HandleSteppingStopped()
    {
        _wasStepping = false;
        StopFootstepSounds();
        _nextStepTime = Time.time + _stepInterval;
    }
    private void StopFootstepSounds()
    {
        if (_groundSource != null) _groundSource.Stop();
        if (_platformSource != null && _platformSource != _groundSource) _platformSource.Stop();
    }
    private void PlayCurrentSurface()
    {
        bool onPlatform = _jump != null && _jump.IsOnPlatform;
        FootstepSFXBank bank = onPlatform ? _platformBank : _groundBank;
        AudioSource source = onPlatform ? _platformSource : _groundSource;
        if (bank == null || source == null)
        {
            bank = _groundBank != null ? _groundBank : _platformBank;
            source = _groundSource != null ? _groundSource : _platformSource;
        }
        if (bank == null || source == null) return;
        bank.PlayOn(source);
    }
    public void PlayStepOnce() => PlayCurrentSurface();
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
