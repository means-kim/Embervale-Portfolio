using UnityEngine;
using UnityEngine.Serialization;
[RequireComponent(typeof(PlayerMove), typeof(PlayerJump), typeof(PlayerController))]
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
    [SerializeField] private bool _playOnLand;
    private PlayerMove _move;
    private PlayerJump _jump;
    private PlayerController _controller;
    private float _nextStepTime;
    private float _stepInterval = 0.35f;
    private bool _wasStepping;
    private void Awake()
    {
        _move = GetComponent<PlayerMove>();
        _jump = GetComponent<PlayerJump>();
        _controller = GetComponent<PlayerController>();
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
        if (_jump == null) return;
        _jump.OnLanded += HandleLanded;
        _jump.OnJumpStarted += HandleSteppingStopped;
        _jump.OnGroundLost += HandleSteppingStopped;
    }
    private void OnDisable()
    {
        StopFootstepSounds();
        if (_jump == null) return;
        _jump.OnLanded -= HandleLanded;
        _jump.OnJumpStarted -= HandleSteppingStopped;
        _jump.OnGroundLost -= HandleSteppingStopped;
    }
    private void Update()
    {
        bool runStep = _controller.CurrentState == PlayerState.Run && _jump.IsGrounded && _move.IsMoving && _move.HasMoveInput;
        if (!runStep)
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
        PlayCurrentSurface();
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
}
