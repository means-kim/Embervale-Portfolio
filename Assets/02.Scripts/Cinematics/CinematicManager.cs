using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicManager : MonoBehaviour
{
    [SerializeField] private CinematicBars _cinematicBars;
    [SerializeField] private CanvasGroup _letterboxGroup;
    [SerializeField] private Rigidbody2D _player;

    [Header("연출 중 끌 컴포넌트")]
    [SerializeField] private Behaviour[] _playerControls = new Behaviour[0];

    public bool IsPlaying { get; private set; }

    private Behaviour[] _lockedControls;
    private bool[] _originalStates;
    private Health _health;

    private PlayableDirector _director;
    private Coroutine _routine;
    private Action _onCompleted;
    private Action _onCancelled;
    private bool _timelineFinished;

    private bool _usesTimeline;
    private bool _showBars;

    public bool Play(
        PlayableDirector director,
        Action onCompleted,
        Func<IEnumerator> preparation = null,
        Action onCancelled = null)
    {
        if (!Application.isPlaying || !isActiveAndEnabled || IsPlaying)
        {
            return false;
        }

        if (_cinematicBars == null || !_cinematicBars.isActiveAndEnabled || _letterboxGroup == null || _player == null)
        {
            return false;
        }

        if (director == null || director.playableAsset == null || !director.isActiveAndEnabled ||
            director.state == PlayState.Playing)
        {
            return false;
        }

        _health = _player.GetComponent<Health>();

        if (!_player.gameObject.activeInHierarchy || !_player.simulated || (_health != null && _health.IsDead))
        {
            return false;
        }

        _director = director;
        _usesTimeline = true;
        _showBars = true;
        _onCompleted = onCompleted;
        _onCancelled = onCancelled;
        _timelineFinished = false;
        IsPlaying = true;

        if (_player.TryGetComponent(out PlayerAttack attack))
        {
            attack.CancelAttack();
        }

        LockControls();
        StopHorizontalMovement();

        _routine = StartCoroutine(PlayRoutine(preparation));
        return true;
    }

    private IEnumerator PlayRoutine(Func<IEnumerator> preparation)
    {
        _cinematicBars.FadeIn();

        if (preparation != null)
        {
            yield return preparation();
        }

        if (!IsPlaying)
        {
            yield break;
        }

        _director.stopped += HandleStopped;
        _director.time = 0;
        _director.Play();

        while (!_timelineFinished)
        {
            yield return null;
        }

        _routine = null;
        EndPlayback(true);
    }

    public bool Play(
        IEnumerator routine,
        Action onCompleted,
        Action onCancelled,
        bool showBars = false)
    {
        if (!Application.isPlaying ||
            !isActiveAndEnabled ||
            IsPlaying ||
            routine == null ||
            _player == null)
        {
            return false;
        }

        if (showBars &&
            (_cinematicBars == null ||
             !_cinematicBars.isActiveAndEnabled ||
             _letterboxGroup == null))
        {
            return false;
        }

        _health = _player.GetComponent<Health>();

        if (!_player.gameObject.activeInHierarchy ||
            !_player.simulated ||
            (_health != null && _health.IsDead))
        {
            return false;
        }

        _director = null;
        _usesTimeline = false;
        _showBars = showBars;

        _onCompleted = onCompleted;
        _onCancelled = onCancelled;

        IsPlaying = true;

        if (_player.TryGetComponent(out PlayerAttack attack))
        {
            attack.CancelAttack();
        }

        LockControls();
        StopHorizontalMovement();

        if (_showBars)
        {
            _cinematicBars.FadeIn();
        }

        _routine = StartCoroutine(PlayCoroutineRoutine(routine));

        return true;
    }

    private IEnumerator PlayCoroutineRoutine(IEnumerator routine)
    {
        // 즉시 끝나는 연출도 _routine 대입 이후에 완료
        yield return null;

        yield return routine;

        if (!IsPlaying)
        {
            yield break;
        }

        _routine = null;
        EndPlayback(true);
    }

    // 해당 연출 종료
    private void HandleStopped(PlayableDirector director)
    {
        if (director == _director)
        {
            _timelineFinished = true;
        }
    }

    public void Cancel()
    {
        if (!IsPlaying)
        {
            return;
        }

        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        EndPlayback(false);
    }

    private void EndPlayback(bool completed)
    {
        // 중단할 때 stopped가 정상 완료로 처리되지 않게 먼저 해제
        if (_director != null)
        {
            _director.stopped -= HandleStopped;

            if (!completed)
            {
                _director.Stop();
            }
        }

        if (_showBars && _cinematicBars != null && _cinematicBars.isActiveAndEnabled)
        {
            _cinematicBars.FadeOut();
        }

        StopHorizontalMovement();
        RestoreControls();

        Action callback = completed ? _onCompleted : _onCancelled;

        IsPlaying = false;
        _director = null;
        _onCompleted = null;
        _onCancelled = null;
        _usesTimeline = false;
        _showBars = false;
        callback?.Invoke();
    }

    private void LockControls()
    {
        if (_player != null)
        {
            if (_player.TryGetComponent(out PlayerChargeJump chargeJump))
            {
                chargeJump.CancelChargeJump();

            }
        }
        _lockedControls = (Behaviour[])_playerControls.Clone();
        _originalStates = new bool[_lockedControls.Length];

        // 비활성화하기 전에 원래 상태를 모두 저장
        for (int i = 0; i < _lockedControls.Length; i++)
        {
            if (_lockedControls[i] != null)
            {
                _originalStates[i] = _lockedControls[i].enabled;
            }
        }

        foreach (Behaviour component in _lockedControls)
        {
            if (component != null)
            {
                component.enabled = false;
            }
        }
    }

    private void RestoreControls()
    {
        for (int i = 0; i < _lockedControls.Length; i++)
        {
            if (_lockedControls[i] != null)
            {
                _lockedControls[i].enabled = _originalStates[i];
            }
        }
    }

    private void StopHorizontalMovement()
    {
        if (_player == null)
        {
            return;
        }

        Vector2 velocity = _player.linearVelocity;
        velocity.x = 0f;
        _player.linearVelocity = velocity;
    }

    private void Update()
    {
        if (!IsPlaying)
        {
            return;
        }

        bool playerUnavailable =
            _player == null ||
            !_player.gameObject.activeInHierarchy ||
            !_player.simulated ||
            (_health != null && _health.IsDead);

        bool directorUnavailable =
            _usesTimeline &&
            (_director == null ||
             !_director.isActiveAndEnabled);

        if (playerUnavailable || directorUnavailable)
        {
            Cancel();
        }
    }

    private void OnDisable()
    {
        Cancel();
    }
}
