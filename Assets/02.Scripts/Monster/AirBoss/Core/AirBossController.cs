using UnityEngine;

public class AirBossController : MonoBehaviour
{
    [Header("States 시간")]
    [SerializeField] private float _idleTimer = 3f;

    [SerializeField] private float _moveTimer = 1f;

    [Header("Random 가중치")]
    [SerializeField] private int _idleWeight = 20;

    [SerializeField] private int _moveWeight = 30;
    [SerializeField] private int _actionWeight = 50;

    [Header("페이즈 관리")]
    [SerializeField] private AirBossPhaseController _phaseController;

    [SerializeField] private BossPhase2Cinematic _phase2Cinematic;
    [SerializeField] private BossPhase3Cinematic _phase3Cinematic;

    private AirBossState _currentstate = AirBossState.Idle;
    private float _stateTimer = 0f;

    private AirBossMovement _movement;
    private AirBossAnimation _animation;
    private AirBossAction _action;
    private AirBossStagger _stagger;
    private Health _health;

    public bool IsStaggered => _currentstate == AirBossState.Stagger;
    public bool CanEnterStagger => isActiveAndEnabled && _health != null && !_health.IsDead &&
        _phaseController != null && !_phaseController.IsTransition &&
        (_currentstate == AirBossState.Idle || _currentstate == AirBossState.Move ||
         _currentstate == AirBossState.Action);

    public bool TryEnterStagger()
    {
        if (_stagger == null)
        {
            _stagger = GetComponent<AirBossStagger>();
        }
        if (!CanEnterStagger || _stagger == null || !_stagger.CanBegin)
        {
            return false;
        }
        ChangeState(AirBossState.Stagger);
        return IsStaggered;
    }

    public void OnStaggerFinished()
    {
        if (!isActiveAndEnabled || !IsStaggered || _health.IsDead) return;
        if (_phaseController != null && _phaseController.IsTransition) return;
        ChangeState(AirBossState.Idle);
    }

    private bool _waitingForPhase2;
    private bool _waitingForPhase3;

    private void Awake()
    {
        _movement = GetComponent<AirBossMovement>();
        _animation = GetComponent<AirBossAnimation>();
        _action = GetComponent<AirBossAction>();
        _stagger = GetComponent<AirBossStagger>();
        _health = GetComponent<Health>();

        if (_phase2Cinematic == null)
        {
            _phase2Cinematic = FindFirstObjectByType<BossPhase2Cinematic>(FindObjectsInactive.Include);
            Debug.LogWarning($"BossPhase2Cinematic 참조가 누락되었습니다. 임시 연결을 합니다.");
        }
        if (_phase3Cinematic == null)
        {
            _phase3Cinematic = FindFirstObjectByType<BossPhase3Cinematic>(FindObjectsInactive.Include);
        }
    }

    private void OnEnable()
    {
        if (_phaseController != null)
        {
            _phaseController.TransitionRequested += HandleTransitionRequested;
        }

        if (_phase2Cinematic != null)
        {
            _phase2Cinematic.Completed += HandlePhase2Completed;
            _phase2Cinematic.Cancelled += HandlePhase2Cancelled;
        }

        if (_phase3Cinematic != null)
        {
            _phase3Cinematic.Completed += HandlePhase3Completed;
            _phase3Cinematic.Cancelled += HandlePhase3Cancelled;
        }
    }

    private void Update()
    {
        _stateTimer += Time.deltaTime;

        switch (_currentstate)
        {
            case AirBossState.Wait:
                UpdateWait();
                break;

            case AirBossState.Encounter:
                UpdateEncounter();
                break;

            case AirBossState.Idle:
                UpdateIdle();
                break;

            case AirBossState.Move:
                UpdateMove();
                break;

            case AirBossState.Action:
                UpdateAction();
                break;

            case AirBossState.Death:
                UpdateDeath();
                break;
        }
    }

    private void OnDisable()
    {
        if (_phaseController != null)
        {
            _phaseController.TransitionRequested -= HandleTransitionRequested;
        }

        if (_phase2Cinematic != null)
        {
            _phase2Cinematic.Completed -= HandlePhase2Completed;
            _phase2Cinematic.Cancelled -= HandlePhase2Cancelled;
        }

        if (_phase3Cinematic != null)
        {
            _phase3Cinematic.Completed -= HandlePhase3Completed;
            _phase3Cinematic.Cancelled -= HandlePhase3Cancelled;
        }

        CancelOwnedPhase2();
        CancelOwnedPhase3();
        _stagger?.Cancel();
        if (IsStaggered)
        {
            _currentstate = AirBossState.Idle;
            _stateTimer = 0f;
        }
    }

    private void UpdateWait()
    {
    }

    private void UpdateEncounter()
    {
    }

    private void UpdateIdle()
    {
        if (_stateTimer >= _idleTimer)
        {
            ChangeRandomState();
        }
    }

    private void UpdateMove()
    {
        _movement.Move();

        if (_stateTimer >= _moveTimer)
        {
            ChangeState(AirBossState.Idle);
        }
    }

    private void UpdateAction()
    {
    }

    private void UpdateDeath()
    {
    }

    private void ChangeState(AirBossState newstate)
    {
        if (_currentstate == AirBossState.Death) return;

        if (IsStaggered && newstate != AirBossState.Stagger)
        {
            _stagger?.Cancel();
        }

        _currentstate = newstate;
        _stateTimer = 0f;

        EnterState();
    }

    private void EnterState()
    {
        if (_currentstate == AirBossState.Stagger)
        {
            _movement.StopMove();
            _action.CancelCurrentAttack();
            AirBossAttackPool.Instance?.ReleaseAll();
            if (_stagger == null || !_stagger.Begin())
            {
                ChangeState(AirBossState.Idle);
            }
        }
        else if (_currentstate == AirBossState.Move)
        {
            _movement.StartMove();
            _animation.PlayMoveAnimation(_movement.Direction);
        }
        else if (_currentstate == AirBossState.Idle)
        {
            _movement.StopMove();
            _animation.PlayIdleAnimation();
        }
        else if (_currentstate == AirBossState.Action)
        {
            _movement.StopMove();

            if (_phaseController == null || _action == null)
            {
                ChangeState(AirBossState.Idle);
                return;
            }

            if (_phaseController.IsTransition)
            {
                ChangeState(AirBossState.Wait);
                return;
            }

            if (!_action.ExecuteRandom(_phaseController.CurrentPhase))
            {
                OnActionFinished();
            }
        }
    }

    private void ChangeRandomState()
    {
        int totalWeight = _idleWeight + _moveWeight + _actionWeight;

        int randomWeight = Random.Range(0, totalWeight);

        if (randomWeight < _idleWeight)
        {
            ChangeState(AirBossState.Idle);
        }
        else if (randomWeight < _idleWeight + _moveWeight)
        {
            ChangeState(AirBossState.Move);
        }
        else
        {
            ChangeState(AirBossState.Action);
        }
    }

    public void EnterDeath()
    {
        if (_currentstate == AirBossState.Death) return;

        ChangeState(AirBossState.Death);
        CancelOwnedPhase2();
        CancelOwnedPhase3();
        _movement.StopMove();
        _action.CancelCurrentAttack();
    }

    public void OnActionFinished()
    {
        if (_currentstate != AirBossState.Action) return;

        _action.CancelCurrentAttack();
        ChangeState(AirBossState.Idle);
    }

    private void HandleTransitionRequested(int nextphase)
    {
        if (_currentstate == AirBossState.Death) return;
        if (_waitingForPhase2 || _waitingForPhase3) return;

        CinematicSequence cinematic = nextphase == 2 ? _phase2Cinematic :
            nextphase == 3 ? _phase3Cinematic : null;
        if (cinematic == null)
        {
            Debug.LogWarning($"페이즈 {nextphase} 연출이 연결되지 않았습니다.");
            _phaseController.CancelTransition();
            return;
        }

        ChangeState(AirBossState.Wait);

        _movement.StopMove();
        _action.CancelCurrentAttack();

        AirBossAttackPool.Instance?.ReleaseAll();

        _waitingForPhase2 = nextphase == 2;
        _waitingForPhase3 = nextphase == 3;

        if (!cinematic.TryPlay())
        {
            if (nextphase == 2) HandlePhase2Cancelled();
            else HandlePhase3Cancelled();
            Debug.Log($"{nextphase}페이즈 연출 실패");
        }
    }

    private void HandlePhase2Completed()
    {
        if (!_waitingForPhase2) return;
        _waitingForPhase2 = false;

        if (_currentstate == AirBossState.Death) return;
        if (_phaseController == null) return;

        if (!_phaseController.IsTransition || _phaseController.PendingPhase != 2) return;

        _phaseController.CompletePhase();

        _phaseController.CheckPhase();

        if (!_phaseController.IsTransition)
        {
            ChangeState(AirBossState.Idle);
        }

        Debug.Log("2페이즈 연출 완료");
    }

    private void HandlePhase2Cancelled()
    {
        if (!_waitingForPhase2) return;
        _waitingForPhase2 = false;

        if (_phaseController != null)
        {
            _phaseController.CancelTransition();
        }

        if (!isActiveAndEnabled) return;
        if (_currentstate == AirBossState.Death) return;

        ChangeState(AirBossState.Idle);

        Debug.Log("2페이즈 연출 취소");
    }

    private void CancelOwnedPhase2()
    {
        if (!_waitingForPhase2) return;

        // Cancel()이 취소 이벤트를 보내도 전투를 재개하지 않도록 먼저 해제
        _waitingForPhase2 = false;

        _phase2Cinematic?.Cancel();
        _phaseController?.CancelTransition();
    }

    private void HandlePhase3Completed()
    {
        if (!_waitingForPhase3) return;
        _waitingForPhase3 = false;

        if (_currentstate == AirBossState.Death) return;
        if (_phaseController == null) return;

        if (!_phaseController.IsTransition || _phaseController.PendingPhase != 3) return;

        _movement.UsePhase3Bounds(transform.position);
        _phaseController.CompletePhase();

        _phaseController.CheckPhase();

        if (!_phaseController.IsTransition)
        {
            ChangeState(AirBossState.Idle);
        }

        Debug.Log("3페이즈 연출 완료");
    }

    private void HandlePhase3Cancelled()
    {
        if (!_waitingForPhase3) return;
        _waitingForPhase3 = false;

        if (_phaseController != null)
        {
            _phaseController.CancelTransition();
        }

        if (!isActiveAndEnabled) return;
        if (_currentstate == AirBossState.Death) return;

        ChangeState(AirBossState.Idle);

        Debug.Log("3페이즈 연출 취소");
    }

    private void CancelOwnedPhase3()
    {
        if (!_waitingForPhase3) return;

        // Cancel()이 취소 이벤트를 보내도 전투를 재개하지 않도록 먼저 해제
        _waitingForPhase3 = false;

        _phase3Cinematic?.Cancel();
        _phaseController?.CancelTransition();
    }
}