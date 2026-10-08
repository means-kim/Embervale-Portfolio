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

    private void Awake()
    {
        _movement = GetComponent<AirBossMovement>();
        _animation = GetComponent<AirBossAnimation>();
        _action = GetComponent<AirBossAction>();
    }

    private void OnEnable()
    {
        if (_phaseController == null) return;

        _phaseController.TransitionRequested += HandleTransitionRequested;
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
        if (_phaseController == null) return;

        _phaseController.TransitionRequested -= HandleTransitionRequested;
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
            _action.ExecuteRandom();
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
        _movement.StopMove();
        _action.CancelCurrentAttack();
    }

    public void OnActionFinished()
    {
        if (_currentstate != AirBossState.Action) return;

        ChangeState(AirBossState.Idle);
    }

    private void HandleTransitionRequested(int nextphase)
    {
        Debug.Log("다음 페이즈로 전환");
    }
}