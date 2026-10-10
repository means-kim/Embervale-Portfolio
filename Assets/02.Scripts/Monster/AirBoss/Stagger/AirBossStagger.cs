using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health), typeof(AirBossController))]
public sealed class AirBossStagger : MonoBehaviour
{
    [Header("같은 보스의 컴포넌트 (비워두면 자동 연결)")]
    [SerializeField] private Health _health;

    [SerializeField] private AirBossController _controller;
    [SerializeField] private AirBossPhaseController _phaseController;
    [SerializeField] private Animator _animator;

    [Header("그로기 게이지")]
    [SerializeField, Min(1f)] private float _maxGauge = 100f;

    [SerializeField, Min(0f)] private float _gainPerHit = 10f;
    [SerializeField, Min(0f)] private float _hitInterval = 0.15f;
    [SerializeField, Min(0f)] private float _cooldown = 10f;
    [SerializeField, Min(0f)] private float _decayDelay = 4f;
    [SerializeField, Min(0f)] private float _decayPerSecond = 5f;

    [Header("페이즈별 도착 지점 (보스 루트가 도착할 월드 위치)")]
    [Tooltip("1, 2, 3페이즈 순서. 해당 페이즈의 지점이 없으면 발동하지 않습니다.")]
    [SerializeField]
    private Transform[] _staggerPoints = new Transform[3];

    [SerializeField, Min(0.01f)] private float _moveDuration = 0.6f;
    [SerializeField, Min(0f)] private float _staggerDuration = 2.5f;
    [SerializeField, Min(0f)] private float _recoverDuration = 0.4f;
    [SerializeField, Min(0.01f)] private float _returnDuration = 0.6f;

    [Header("Animator 상태 이름 (빈칸이면 해당 단계 재생 생략)")]
    [SerializeField] private string _enterAnimation = "";

    [SerializeField] private string _holdAnimation = "";
    [SerializeField] private string _recoverAnimation = "";

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
                            _health != null && !_health.IsDead && _controller != null &&
                            _phaseController != null && !_phaseController.IsTransition &&
                            Time.time >= _nextStaggerTime && Time.frameCount != _blockedFrame &&
                            GetStaggerPoint() != null;

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

        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
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

    private Transform GetStaggerPoint()
    {
        if (_phaseController == null || _staggerPoints == null)
        {
            return null;
        }

        int index = _phaseController.CurrentPhase - 1;
        return index >= 0 && index < _staggerPoints.Length ? _staggerPoints[index] : null;
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

        Vector3 destination = GetStaggerPoint().position;
        Vector3 returnPosition = transform.position;
        ResetGauge();
        _routine = StartCoroutine(StaggerRoutine(destination, returnPosition));
        return true;
    }

    private IEnumerator StaggerRoutine(Vector3 destination, Vector3 returnPosition)
    {
        // 코루틴 핸들 대입 전에 종료/취소 콜백이 실행되는 것을 막습니다.
        yield return null;
        PlayAnimation(_enterAnimation);
        yield return MoveTo(destination, _moveDuration);
        PlayAnimation(_holdAnimation);
        // 도착 이후부터 온전한 공격 기회를 제공합니다.
        yield return new WaitForSeconds(Mathf.Max(0f, _staggerDuration));
        PlayAnimation(_recoverAnimation);
        yield return new WaitForSeconds(Mathf.Max(0f, _recoverDuration));
        yield return MoveTo(returnPosition, _returnDuration);
        _routine = null;
        _nextStaggerTime = Time.time + Mathf.Max(0f, _cooldown);
        _controller.OnStaggerFinished();
    }

    private IEnumerator MoveTo(Vector3 destination, float duration)
    {
        Vector3 start = transform.position;
        duration = Mathf.Max(0.01f, duration);
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.position = Vector3.Lerp(start, destination, t);
            yield return null;
        }

        transform.position = destination;
    }

    private void PlayAnimation(string stateName)
    {
        if (string.IsNullOrWhiteSpace(stateName) || _animator == null ||
            !_animator.isActiveAndEnabled || _animator.runtimeAnimatorController == null)
        {
            return;
        }
        int hash = Animator.StringToHash(stateName);
        if (!_animator.HasState(0, hash))
        {
            Debug.LogWarning($"그로기 Animator 상태를 찾을 수 없습니다: {stateName}", this);
            return;
        }

        _animator.CrossFade(hash, 0.1f, 0);
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