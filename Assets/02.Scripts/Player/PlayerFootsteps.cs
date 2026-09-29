using UnityEngine;
[RequireComponent(typeof(PlayerMove), typeof(PlayerJump), typeof(PlayerController))]
public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private FootstepSFXBank _bank;
    [SerializeField, Min(0.05f)] private float _stepInterval = 0.35f;
    [SerializeField] private bool _playOnLand;
    private PlayerMove _move;
    private PlayerJump _jump;
    private PlayerController _controller;
    private float _nextStepTime;
    private bool _hasBank;
    private void Awake()
    {
        _move = GetComponent<PlayerMove>();
        _jump = GetComponent<PlayerJump>();
        _controller = GetComponent<PlayerController>();
        _hasBank = _bank != null;
    }
    private void OnValidate()
    {
        _hasBank = _bank != null;
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
        if (!_hasBank) return;
        if (_controller.CurrentState != PlayerState.Run) return;
        if (!_jump.IsGrounded || !_move.IsMoving) return;
        if (Time.time < _nextStepTime) return;
        _nextStepTime = Time.time + _stepInterval;
        _bank.Play();
    }
    private void HandleLanded()
    {
        if (!_playOnLand || _bank == null) return;
        _bank.Play();
        _nextStepTime = Time.time + _stepInterval;
    }
    public void PlayStepOnce()
    {
        if (_bank == null) return;
        _bank.Play();
    }
}
