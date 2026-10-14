using Game.Boss.Roar;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class BossEncounterCinematic : CinematicSequence
{
    [Header("연결")]
    [SerializeField] private CinematicManager _cinematicManager;
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private AirBossController _boss;
    [SerializeField] private SpriteRenderer _bossRenderer;
    [SerializeField] private Rigidbody2D _player;
    [SerializeField] private Transform _walkTarget;
    [SerializeField] private BossRoarScreenEffect _roarEffect;
    [SerializeField, Min(0.1f)] private float _roarDuration = 3f;

    [Header("이동")]
    [SerializeField, Min(0.1f)] private float _walkSpeed = 5f;
    [SerializeField, Min(0.01f)] private float _stopDistance = 0.1f;
    [SerializeField, Min(1f)] private float _walkTimeout = 10f;

    private PlayerAnimator _playerAnimator;
    private bool _requested;
    private bool _repeatPlayback;
    private bool _startedBossBgm;

    private bool CanPlay()
    {
        return Application.isPlaying && isActiveAndEnabled && !_requested
               && _boss != null && _bossRenderer != null
               && _director != null && _director.isActiveAndEnabled
               && _director.playableAsset != null && _director.state != PlayState.Playing;
    }

    public override bool TryPlay()
    {
        if (!CanPlay())
        {
            return false;
        }

        if (_cinematicManager == null
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

        bool wasBossEnabled = _boss.enabled;
        bool wasRendererEnabled = _bossRenderer.enabled;
        PrepareBoss();

        bool accepted = _cinematicManager.Play(_director, HandleCompleted, WalkToTarget, HandleCancelled);

        if (!accepted)
        {
            _requested = false;
            _boss.enabled = wasBossEnabled;
            _bossRenderer.enabled = wasRendererEnabled;
        }
        else
        {
            EnableSkipping(_cinematicManager);
        }

        return accepted;
    }

    // 재입장은 플레이어 조작과 카메라 연출을 건드리지 않고 보스 Timeline만 재생
    public override bool TryPlayWithoutCinematic()
    {
        if (!CanPlay())
        {
            return false;
        }

        PrepareBoss();
        _repeatPlayback = true;
        _director.stopped += HandleRepeatStopped;
        _director.time = 0;
        _director.Play();
        EnableSkipping(_cinematicManager);
        return true;
    }

    private void PrepareBoss()
    {
        _requested = true;
        _repeatPlayback = false;
        _startedBossBgm = false;
        _boss.enabled = false;
        _bossRenderer.enabled = false;
    }

    private void HandleRepeatStopped(PlayableDirector director)
    {
        if (!_requested || !_repeatPlayback || director != _director)
        {
            return;
        }

        _director.stopped -= HandleRepeatStopped;
        _repeatPlayback = false;
        HandleCompleted();
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

    public void ShowBoss()
    {
        if (_bossRenderer == null)
        {
            return;
        }

        _bossRenderer.enabled = true;
    }

    public void PlayRoar()
    {
        if (_boss == null || _roarEffect == null || !_roarEffect.isActiveAndEnabled)
        {
            return;
        }

        _roarEffect.Play(_boss.transform, _roarDuration, BossRoarScreenEffect.BossPhase.Phase1);
    }

    public void PlayBossBgm()
    {
        if (!_requested || _startedBossBgm || AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.StopBgm(BgmType.IntoBoss);
        AudioManager.Instance.PlayBgm(BgmType.Boss);
        _startedBossBgm = true;
    }

    private void HandleCompleted()
    {
        _requested = false;
        if (_bossRenderer != null)
        {
            _bossRenderer.enabled = true;
        }

        if (_boss != null)
        {
            _boss.enabled = true;
        }

        NotifyCompleted();
    }

    private void HandleCancelled()
    {
        _requested = false;
        _roarEffect?.Stop();
        if (_bossRenderer != null)
        {
            _bossRenderer.enabled = false;
        }

        if (_boss != null)
        {
            _boss.enabled = false;
        }

        if (_startedBossBgm)
        {
            AudioManager.Instance?.StopBgm(BgmType.Boss);
            _startedBossBgm = false;
        }

        NotifyCancelled();
    }

    public override void Skip()
    {
        if (!_requested) return;
        _roarEffect?.StopImmediately();
        PlayBossBgm();
        if (_repeatPlayback)
        {
            _director.stopped -= HandleRepeatStopped;
            _repeatPlayback = false;
            _director.Stop();
            HandleCompleted();
            return;
        }
        if (_player != null && _walkTarget != null)
        {
            _player.position = new Vector2(_walkTarget.position.x, _player.position.y);
            _playerAnimator?.PlayState(PlayerState.Idle, false);
        }
        _cinematicManager.Complete();
    }

    public override void Cancel()
    {
        if (!_requested)
        {
            return;
        }

        if (_repeatPlayback)
        {
            if (_director != null)
            {
                _director.stopped -= HandleRepeatStopped;
            }

            _repeatPlayback = false;

            if (_director != null)
            {
                _director.Stop();
            }

            HandleCancelled();
        }
        else if (_cinematicManager != null && _cinematicManager.IsPlaying)
        {
            _cinematicManager.Cancel();
        }
        else
        {
            HandleCancelled();
        }
    }

    private void OnDisable()
    {
        Cancel();
    }
}
