using UnityEngine;

public class SlimeBController : MonoBehaviour
{
    [Header("State 시간")]
    [SerializeField] private float _idleTimer = 1f;

    private SlimeBMovement _movement;
    private SlimeBAnimation _animation;

    private SlimeBState _currentState;

    private float _stateTimer = 0f;
    private bool _hasState = false;
    private bool _started = false;

    private void Awake()
    {
        _movement = GetComponent<SlimeBMovement>();
        _animation = GetComponent<SlimeBAnimation>();
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

        if (_currentState == SlimeBState.Idle && _stateTimer >= _idleTimer)
        {
            ChangeState(SlimeBState.Move);
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
                break;
        }
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
    }

    private void OnEnable()
    {
        if (_started && !_hasState)
        {
            ChangeState(SlimeBState.Idle);
        }
    }
}