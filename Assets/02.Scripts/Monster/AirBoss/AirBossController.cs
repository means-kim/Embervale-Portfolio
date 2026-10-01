using UnityEngine;

public class AirBossController : MonoBehaviour
{
    [Header("States 시간")]
    [SerializeField] private float _idleTimer = 3f;

    [SerializeField] private float _moveTimer = 1f;

    private AirBossState _currentstate = AirBossState.Idle;
    private float _stateTimer = 0f;

    private AirBossMovement _movement;
    private AirBossAnimation _animation;

    private void Awake()
    {
        _movement = GetComponent<AirBossMovement>();
        _animation = GetComponent<AirBossAnimation>();
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
            ChangeState(AirBossState.Move);
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
    }
}