using UnityEngine;
using UnityEngine.Serialization;
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Surface Banks")]
    [Tooltip("Ground 태그 위에서 재생")][FormerlySerializedAs("_bank")][SerializeField] private FootstepSFXBank _groundBank;
    [Tooltip("Platform 태그 위에서 재생")][SerializeField] private FootstepSFXBank _platformBank;
    [Header("Dedicated Source")]
    [FormerlySerializedAs("_groundSource")][FormerlySerializedAs("_platformSource")][SerializeField] private AudioSource _audioSource;
    [SerializeField] private bool _playOnLand;
    [Header("Walking VFX")]
    [SerializeField, Tooltip("발 아래 생성 위치. 비워두면 플레이어 중심에서 생성합니다.")]
    private Transform _footstepPoint;
    [SerializeField, Min(0.05f)] private float _stepInterval = 0.35f;
    private PlayerMove _move;
    private PlayerJump _jump;
    private PlayerController _controller;
    private float _nextStepTime;
    private bool _wasStepping;
    private void Awake()
    {
        PlayerRig rig = PlayerRig.Resolve(gameObject);
        _move = rig.Move;
        _jump = rig.Jump;
        _controller = rig.Controller;
        EnsureSource();
    }
    private void EnsureSource()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;
        }
    }
    private void OnEnable()
    {
        _jump.OnLanded += HandleLanded;
        _jump.OnJumpStarted += HandleSteppingStopped;
        _jump.OnGroundLost += HandleSteppingStopped;
    }
    private void OnDisable()
    {
        StopFootstepSounds();
        _jump.OnLanded -= HandleLanded;
        _jump.OnJumpStarted -= HandleSteppingStopped;
        _jump.OnGroundLost -= HandleSteppingStopped;
    }
    private void Update()
    {
        if (Time.deltaTime <= 0f)
        {
            return;
        }
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
        if (Time.time < _nextStepTime)
        {
            return;
        }
        _nextStepTime = Time.time + _stepInterval;
        PlayCurrentSurface();
        Vector3 position = transform.position;
        if (_footstepPoint != null)
        {
            position = _footstepPoint.position;
        }
        EffectManager.Instance?.PlayFootstep(position, _move.FacingDirection);
    }
    private void HandleLanded()
    {
        if (!_playOnLand)
        {
            return;
        }
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
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }
    private void PlayCurrentSurface()
    {
        bool onPlatform = _jump.IsOnPlatform;
        FootstepSFXBank bank = _groundBank;
        if (onPlatform)
        {
            bank = _platformBank;
        }
        if (bank == null)
        {
            bank = _groundBank;
            if (bank == null)
            {
                bank = _platformBank;
            }
        }
        if (bank == null || _audioSource == null)
        {
            return;
        }
        bank.PlayOn(_audioSource);
    }
}
