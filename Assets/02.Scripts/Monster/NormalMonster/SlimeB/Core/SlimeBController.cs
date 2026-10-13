using UnityEngine;

public class SlimeBController : MonoBehaviour
{
    [Header("State 시간")]
    [SerializeField] private float _idleTimer = 1f;

    [SerializeField] private float _moveTimer = 1f;

    private SlimeBState _currentState = SlimeBState.Idle;
    private float _stateTimer = 0f;

    private void Awake()
    {
    }

    private void OnEnable()
    {
    }

    private void Start()
    {
    }

    private void Update()
    {
        _stateTimer += Time.deltaTime;

        switch (_currentState)
        {
            case SlimeBState.Idle:
                UpdateIdle();
                break;

            case SlimeBState.Move:
                UpdateMove();
                break;
        }
    }

    private void FixedUpdate()
    {
    }

    private void OnDisable()
    {
    }

    private void ChangeState(SlimeBState newState)
    {
        if (_currentState == SlimeBState.Death) return;

        _currentState = newState;
        _stateTimer = 0f;
    }

    private void UpdateIdle()
    {
        if (_stateTimer >= _idleTimer)
        {
            ChangeState(SlimeBState.Move);
        }
    }

    private void UpdateMove()
    {
        if (_stateTimer >= _moveTimer)
        {
            ChangeState(SlimeBState.Idle);
        }
    }
}