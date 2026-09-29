using UnityEngine;

/// <summary>
/// Run 상태에서 지면에 있을 때 일정 간격으로 FootstepSFXBank에서 랜덤 1개를 재생한다.
/// 애니메이션 이벤트로 쓰고 싶으면 PlayStepOnce()를 Run 클립에 연결해도 된다.
/// </summary>
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

    private void Awake()
    {
        _move = GetComponent<PlayerMove>();
        _jump = GetComponent<PlayerJump>();
        _controller = GetComponent<PlayerController>();
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
        if (_bank == null || _move == null || _jump == null || _controller == null) return;
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

    /// <summary>애니메이션 이벤트용. 간격 제한 없이 1회 재생.</summary>
    public void PlayStepOnce()
    {
        if (_bank == null) return;
        _bank.Play();
    }
}
