using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health), typeof(AirBossController), typeof(AirBossAnimation))]
public sealed class AirBossStagger : MonoBehaviour
{
    [Header("같은 보스의 컴포넌트 (비워두면 자동 연결)")]
    [SerializeField] private Health _health;

    [SerializeField] private AirBossController _controller;
    [SerializeField] private AirBossPhaseController _phaseController;
    [SerializeField] private AirBossAnimation _animation;

    [Header("그로기 게이지")]
    [SerializeField, Min(1f)] private float _maxGauge = 100f;

    [SerializeField, Min(0f)] private float _gainPerHit = 10f;
    [SerializeField, Min(0f)] private float _hitInterval = 0.15f;
    [SerializeField, Min(0f)] private float _cooldown = 10f;
    [SerializeField, Min(0f)] private float _decayDelay = 4f;
    [SerializeField, Min(0f)] private float _decayPerSecond = 5f;

    [Header("그로기 애니메이션 시간")]
    [SerializeField, Min(0f)] private float _enterDuration = 0.6f;
    [SerializeField, Min(0f)] private float _staggerDuration = 2.5f;
    [SerializeField, Min(0f)] private float _recoverDuration = 0.8f;

    private float _gauge;
    private float _lastHitTime = float.NegativeInfinity;
    private float _nextHitTime;
    private float _nextStaggerTime;
    private int _blockedFrame = -1;
    private bool _pending;
    private Coroutine _routine;

    public float Gauge => _gauge;
    public float MaxGauge => Mathf.Max(1f, _maxGauge);
    public float NormalizedGauge => _gauge / MaxGauge;
    public bool IsRunning => _routine != null;

    public bool CanBegin => isActiveAndEnabled && !IsRunning &&
                            _health != null && !_health.IsDead && _controller != null && _animation != null &&
                            _phaseController != null && !_phaseController.IsTransition &&
                            Time.time >= _nextStaggerTime && Time.frameCount != _blockedFrame;

    private void Awake()
    {
        if (_health == null)
        {
            _health = GetComponent<Health>();
        }

        if (_controller == null)
        {
            _controller = GetComponent<AirBossController>();
        }

        if (_phaseController == null)
        {
            _phaseController = GetComponent<AirBossPhaseController>();
        }

        if (_animation == null)
        {
            _animation = GetComponent<AirBossAnimation>();
        }
    }

    private void OnEnable()
    {
        _health.OnHit += HandleHit;
        _health.OnDie += Cancel;

        if (_phaseController != null)
        {
            _phaseController.TransitionRequested += HandleTransitionRequested;
        }
    }

    private void OnDisable()
    {
        _health.OnHit -= HandleHit;
        _health.OnDie -= Cancel;

        if (_phaseController != null)
        {
            _phaseController.TransitionRequested -= HandleTransitionRequested;
        }

        Cancel();

        // 이 컴포넌트만 꺼진 경우에도 FSM이 그로기에 남지 않게 합니다.
        if (_controller != null)
        {
            _controller.OnStaggerFinished();
        }
    }

    private void HandleHit()
    {
        if (!CanBegin || !_controller.CanEnterStagger || Time.time < _nextHitTime) return;

        _lastHitTime = Time.time;
        _nextHitTime = Time.time + Mathf.Max(0f, _hitInterval);
        _gauge = Mathf.Min(MaxGauge, _gauge + Mathf.Max(0f, _gainPerHit));
        _pending = _gauge >= MaxGauge;
    }

    private void LateUpdate()
    {
        if (_controller == null || !_controller.isActiveAndEnabled)
        {
            Cancel();
            return;
        }

        // OnHit의 모든 구독자가 처리된 뒤 사망/페이즈 전환을 다시 확인합니다.
        if (_pending)
        {
            _pending = false;
            if (CanBegin && _controller.TryEnterStagger()) return;
            ResetGauge();
        }

        if (!IsRunning && Time.time - _lastHitTime >= _decayDelay)
        {
            _gauge = Mathf.Max(0f, _gauge - Mathf.Max(0f, _decayPerSecond) * Time.deltaTime);
        }
    }

    private void HandleTransitionRequested(int nextPhase)
    {
        // 전환이 즉시 취소돼도 같은 타격에서 그로기를 발동하지 않습니다.
        _blockedFrame = Time.frameCount;
        ResetGauge();
        // 실행 중 동작 취소는 FSM이 Stagger를 벗어날 때 처리합니다.
    }

    public void ResetGauge()
    {
        _gauge = 0f;
        _pending = false;
    }

    // FSM이 Stagger 상태로 바꾼 뒤 호출합니다.
    public bool Begin()
    {
        if (!CanBegin || !_controller.IsStaggered)
        {
            return false;
        }

        ResetGauge();

        _routine = StartCoroutine(StaggerRoutine());

        return true;
    }

    private IEnumerator StaggerRoutine()
    {
        // 코루틴 핸들 대입 전에 종료/취소 콜백이 실행되는 것을 막습니다.
        yield return null;

        _animation.PlayStaggerEnterAnimation();
        yield return new WaitForSeconds(Mathf.Max(0f, _enterDuration));

        _animation.PlayStaggerHoldAnimation();
        // 진입 애니메이션 이후부터 온전한 공격 기회를 제공합니다.
        yield return new WaitForSeconds(Mathf.Max(0f, _staggerDuration));

        _animation.PlayStaggerRecoveryAnimation();
        yield return new WaitForSeconds(Mathf.Max(0f, _recoverDuration));

        _routine = null;
        _nextStaggerTime = Time.time + Mathf.Max(0f, _cooldown);
        _controller.OnStaggerFinished();
    }

    public void Cancel()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
            _nextStaggerTime = Time.time + Mathf.Max(0f, _cooldown);
        }

        ResetGauge();
        // 위치/애니메이션/FSM은 변경하지 않습니다. 다음 상태가 제어를 이어받습니다.
    }
}