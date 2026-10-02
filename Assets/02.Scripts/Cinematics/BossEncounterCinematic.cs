using UnityEngine;
using UnityEngine.Events;

public class BossEncounterCinematic : CinematicSequence
{
    [Header("연결")]
    [SerializeField] private Rigidbody2D _player;
    [SerializeField] private Transform _walkTarget;

    [Header("이동")]
    [SerializeField, Min(0.1f)] private float _walkSpeed = 3f;
    [SerializeField, Min(0.01f)] private float _stopDistance = 0.05f;
    [SerializeField, Min(1f)] private float _walkTimeout = 30f;

    [Header("이벤트")]
    [SerializeField] private UnityEvent _onArrived;
    [SerializeField] private UnityEvent _onCancelled;

    private Behaviour[] _components;
    private bool[] _originalEnabledStates;
    private PlayerAnimator _playerAnimator;
    private Health _health;

    private bool _hasControl;
    private bool _isWalking;
    private float _elapsed;

    public bool TryPlay()
    {
        if (_hasControl || !Application.isPlaying) return false;

        if (_player == null || _walkTarget == null)
        {
            return false;
        }

        if (!_player.gameObject.activeInHierarchy || !_player.simulated)
        {
            return false;
        }

        _health = _player.GetComponent<Health>();
        if (_health != null && _health.IsDead)
        {
            return false;
        }

        _playerAnimator = _player.GetComponent<PlayerAnimator>();

        _components = new Behaviour[]
        {
            _player.GetComponent<PlayerController>(),
            _player.GetComponent<PlayerInput>(),
            _player.GetComponent<PlayerMove>(),
            _player.GetComponent<PlayerJump>(),
            _player.GetComponent<PlayerDash>(),
            _player.GetComponent<PlayerAttack>(),
            _player.GetComponent<PlayerWall>(),
            _player.GetComponent<PlayerFootsteps>()
        };

        _originalEnabledStates = new bool[_components.Length];
        _hasControl = true;

        for (int i = 0; i < _components.Length; i++)
        {
            Behaviour component = _components[i];
            if (component == null)
            {
                continue;
            }

            _originalEnabledStates[i] = component.enabled;
            component.enabled = false;
        }

        SetHorizontalVelocity(0f);

        float distance = _walkTarget.position.x - _player.position.x;

        if (Mathf.Abs(distance) > _stopDistance)
        {
            Vector3 scale = _player.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(distance);
            _player.transform.localScale = scale;
        }

        _elapsed = 0f;
        _isWalking = true;

        if (_playerAnimator != null)
            _playerAnimator.PlayState(PlayerState.Run, false);

        return true;
    }

    private void FixedUpdate()
    {
        if (!_isWalking) return;

        if (_player == null || _walkTarget == null
                            || !_player.gameObject.activeInHierarchy
                            || (_health != null && _health.IsDead))
        {
            Cancel();
            return;
        }

        _elapsed += Time.fixedDeltaTime;

        if (_elapsed >= _walkTimeout)
        {
            Cancel();
            return;
        }

        float distance = _walkTarget.position.x - _player.position.x;

        if (Mathf.Abs(distance) <= _stopDistance)
        {
            _isWalking = false;
            SetHorizontalVelocity(0f);

            if (_playerAnimator != null)
            {
                _playerAnimator.PlayState(PlayerState.Idle, false);
            }

            _onArrived?.Invoke();
            return;
        }

        float speed = Mathf.Clamp(
            distance / Time.fixedDeltaTime,
            -_walkSpeed,
            _walkSpeed);

        SetHorizontalVelocity(speed);
    }

    private void SetHorizontalVelocity(float speed)
    {
        if (_player == null)
        {
            return;
        }

        Vector2 velocity = _player.linearVelocity;
        velocity.x = speed;
        _player.linearVelocity = velocity;
    }

    public void Finish()
    {
        if (!_hasControl)
        {
            return;
        }

        _hasControl = false;
        _isWalking = false;
        SetHorizontalVelocity(0f);

        for (int i = 0; i < _components.Length; i++)
        {
            Behaviour component = _components[i];
            if (component != null)
            {
                component.enabled = _originalEnabledStates[i];
            }
        }
    }

    private void Cancel()
    {
        Finish();
        _onCancelled?.Invoke();
    }

    private void OnDisable()
    {
        Finish();
    }
}