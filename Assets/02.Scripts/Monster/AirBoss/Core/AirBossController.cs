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
    // Todo: 3페이즈 연출 완료시 연결
    // [SerializeField] private BossPhase3Cinematic _phase3Cinematic;

    private AirBossState _currentstate = AirBossState.Idle;
    private float _stateTimer = 0f;

    private AirBossMovement _movement;
    private AirBossAnimation _animation;
    private AirBossAction _action;

    private bool _waitingForPhase2;
    // private bool _waitingForPhase3;

    private void Awake()
    {
        _movement = GetComponent<AirBossMovement>();
        _animation = GetComponent<AirBossAnimation>();
        _action = GetComponent<AirBossAction>();

        if (_phase2Cinematic == null)
        {
            _phase2Cinematic = FindFirstObjectByType<BossPhase2Cinematic>(FindObjectsInactive.Include);
            Debug.LogWarning($"BossPhase2Cinematic 참조가 누락되었습니다. 임시 연결을 합니다.");
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

        CancelOwnedPhase2();
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

        _currentstate = newstate;
        _stateTimer = 0f;

        EnterState();
    }

    private void EnterState()
    {
        if (_currentstate == AirBossState.Move)
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
        if (_waitingForPhase2) return;

        if (_phase2Cinematic == null || nextphase != 2)
        {
            Debug.LogWarning($"페이즈 {nextphase} 연출이 연결되지 않았습니다.");
            _phaseController.CancelTransition();
            return;
        }

        ChangeState(AirBossState.Wait);

        _movement.StopMove();
        _action.CancelCurrentAttack();

        AirBossAttackPool.Instance?.ReleaseAll();

        _waitingForPhase2 = true;

        if (!_phase2Cinematic.TryPlay())
        {
            HandlePhase2Cancelled();
            Debug.Log("2페이즈 연출 실패");
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
}