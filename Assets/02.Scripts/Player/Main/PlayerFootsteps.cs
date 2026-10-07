using UnityEngine;
using UnityEngine.Serialization;
[RequireComponent(typeof(PlayerMove), typeof(PlayerJump), typeof(PlayerController))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Surface Banks")]
    [Tooltip("Ground 태그 위에서 재생")][FormerlySerializedAs("_bank")][SerializeField] private FootstepSFXBank _groundBank;
    [Tooltip("Platform 태그 위에서 재생")][SerializeField] private FootstepSFXBank _platformBank;
    [Header("Dedicated Source")]
    [FormerlySerializedAs("_groundSource")][FormerlySerializedAs("_platformSource")][SerializeField] private AudioSource _audioSource;
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
        EnsureSource();
    }
    private void EnsureSource()
    {
        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;
        }
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
        if (_audioSource != null) _audioSource.Stop();
    }
    private void PlayCurrentSurface()
    {
        bool onPlatform = _jump != null && _jump.IsOnPlatform;
        FootstepSFXBank bank = onPlatform ? _platformBank : _groundBank;
        if (bank == null)
        {
            bank = _groundBank != null ? _groundBank : _platformBank;
        }
        if (bank == null || _audioSource == null) return;
        bank.PlayOn(_audioSource);
    }
}
