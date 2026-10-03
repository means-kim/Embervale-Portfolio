using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class BossEncounterCinematic : CinematicSequence
{
    [Header("연결")]
    [SerializeField] private CinematicManager _cinematicManager;
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private AirBossController _boss;
    [SerializeField] private Rigidbody2D _player;
    [SerializeField] private Transform _walkTarget;

    [SerializeField] private AreaController _area;

    [Header("이동")]
    [SerializeField, Min(0.1f)] private float _walkSpeed = 5f;
    [SerializeField, Min(0.01f)] private float _stopDistance = 0.1f;
    [SerializeField, Min(1f)] private float _walkTimeout = 10f;

    private PlayerAnimator _playerAnimator;
    private bool _requested;

    public bool TryPlay()
    {
        if (!isActiveAndEnabled || _requested)
        {
            return false;
        }

        if (_area == null || _cinematicManager == null
            || _director == null || _boss == null
            || _player == null || _walkTarget == null)
        {
            return false;
        }

        _playerAnimator = _player.GetComponent<PlayerAnimator>();

        if (_playerAnimator == null)
        {
            return false;
        }

        _requested = true;

        bool accepted = _cinematicManager.Play(_director, HandleCompleted, WalkToTarget, HandleCancelled);

        if (!accepted)
        {
            _requested = false;
        }

        return accepted;
    }

    private IEnumerator WalkToTarget()
    {
        float distance = _walkTarget.position.x - _player.position.x;

        if (Mathf.Abs(distance) > _stopDistance)
        {
            Vector3 scale = _player.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(distance);
            _player.transform.localScale = scale;

            _playerAnimator.PlayState(PlayerState.Run, false);
        }

        float elapsed = 0f;
        var wait = new WaitForFixedUpdate();

        while (true)
        {
            if (_walkTarget == null || elapsed >= _walkTimeout)
            {
                _cinematicManager.Cancel();
                yield break;
            }

            distance = _walkTarget.position.x - _player.position.x;

            if (Mathf.Abs(distance) <= _stopDistance)
            {
                break;
            }

            float speed = Mathf.Clamp(distance / Time.fixedDeltaTime, -_walkSpeed, _walkSpeed);

            Vector2 velocity = _player.linearVelocity;
            velocity.x = speed;
            _player.linearVelocity = velocity;

            yield return wait;
            elapsed += Time.fixedDeltaTime;
        }

        Vector2 stoppedVelocity = _player.linearVelocity;
        stoppedVelocity.x = 0f;
        _player.linearVelocity = stoppedVelocity;

        _playerAnimator?.PlayState(PlayerState.Idle, false);
    }

    private void HandleCompleted()
    {
        _requested = false;
        _area.HandleIntroCompleted();
    }

    private void HandleCancelled()
    {
        _requested = false;
        _area.HandleIntroCancelled();
    }

    private void OnDisable()
    {
        if (_requested && _cinematicManager != null)
        {
            _cinematicManager.Cancel();
        }
    }
}