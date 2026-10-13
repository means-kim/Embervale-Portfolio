using UnityEngine;

public class SlimeBController : MonoBehaviour
{
    [Header("State 시간")]
    [SerializeField] private float _idleTimer = 1f;

    [SerializeField] private float _hitTimer = 0.2f;

    private SlimeBMovement _movement;
    private SlimeBAnimation _animation;
    private SlimeBDeath _death;
    private Health _health;

    private SlimeBState _currentState;

    private float _stateTimer = 0f;
    private bool _hasState = false;
    private bool _started = false;

    private void Awake()
    {
        _movement = GetComponent<SlimeBMovement>();
        _animation = GetComponent<SlimeBAnimation>();
        _death = GetComponent<SlimeBDeath>();
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        _movement.Initialize();
        _started = true;
        ChangeState(SlimeBState.Idle);
    }

    private void Update()
    {
        if (!_hasState) return;

        _stateTimer += Time.deltaTime;


        switch (_currentState)
        {
            case SlimeBState.Idle:
                if (_stateTimer >= _idleTimer)
                {
                    ChangeState(SlimeBState.Move);
                }

                break;

            case SlimeBState.Hit:
                if (_stateTimer >= _hitTimer && !_health.IsDead)
                {
                    ChangeState(SlimeBState.Move);
                }

                break;
        }
    }

    private void FixedUpdate()
    {
        if (!_hasState || _currentState != SlimeBState.Move) return;

        if (_movement.Move())
        {
            ChangeState(SlimeBState.Idle);
        }
    }

    private void ChangeState(SlimeBState newState)
    {
        if (_hasState)
        {
            if (_currentState == SlimeBState.Death) return;

            if (_currentState == newState) return;
        }

        _movement.StopMove();

        _currentState = newState;
        _stateTimer = 0f;
        _hasState = true;

        switch (newState)
        {
            case SlimeBState.Idle:
                _animation.PlayIdleAnimation();
                break;

            case SlimeBState.Move:
                _movement.StartMove();
                _animation.PlayMoveAnimation();
                break;

            case SlimeBState.Hit:
                _animation.PlayHitAnimation();
                break;

            case SlimeBState.Death:
                _animation.PlayDeathAnimation();
                _death.BeginDeath();
                break;
        }
    }

    private void OnEnable()
    {
        if (_started && !_hasState)
        {
            ChangeState(SlimeBState.Idle);
        }

        _health.OnHit += HandleHit;
        _health.OnDie += HandleDeath;
    }

    private void OnDisable()
    {
        if (_movement != null)
        {
            _movement.StopMove();
        }

        if (_hasState && _currentState != SlimeBState.Death)
        {
            _hasState = false;
        }

        _health.OnHit -= HandleHit;
        _health.OnDie -= HandleDeath;
    }

    private void HandleHit()
    {
        if (!_started || _health.IsDead) return;

        ChangeState(SlimeBState.Hit);
    }

    private void HandleDeath()
    {
        ChangeState(SlimeBState.Death);
    }
}