using UnityEngine;
using UnityEngine.Playables;

public class GameEndingCinematic : CinematicSequence
{
    [SerializeField] private CinematicManager _cinematicManager;
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private AirBossController _boss;
    [SerializeField] private Transform _bossPosition;
    public bool IsPlaying => _requested;

    private bool _requested;

    public override bool TryPlay()
    {
        if (!isActiveAndEnabled || _requested)
        {
            return false;
        }

        if (_cinematicManager == null || _director == null || _boss == null)
        {
            return false;
        }

        // 정상 종료 시 완료 콜백이 호출되도록 Timeline은 None으로 설정한다.
        if (_director.extrapolationMode != DirectorWrapMode.None)
        {
            return false;
        }

        _requested = true;
        bool accepted = _cinematicManager.Play(_director, HandleCompleted, onCancelled: HandleCancelled);

        if (!accepted)
        {
            _requested = false;
        }

        return accepted;
    }

    private void HandleCompleted()
    {
        _requested = false;
        NotifyCompleted();
    }

    private void HandleCancelled()
    {
        _requested = false;
        NotifyCancelled();
    }

    public override void Cancel()
    {
    }
}