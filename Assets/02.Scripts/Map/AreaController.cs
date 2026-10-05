using UnityEngine;
using UnityEngine.Events;

public class AreaController : MonoBehaviour
{
    private enum AreaState
    {
        Waiting,
        Intro,
        Fighting,
        Cleared
    }

    [Header("이력")]
    [SerializeField] private EncounterId _encounterId;
    [SerializeField] private EliminateId _eliminateId;

    [Header("장벽")]
    [SerializeField] private AreaBlock _leftBlock;
    [SerializeField] private AreaBlock _rightBlock;

    [Header("등장 연출")]
    [SerializeField] private bool _useIntro;
    [SerializeField] private CinematicSequence _intro;

    [Header("전투 시작")]
    [SerializeField] private UnityEvent _onBattleStarted;

    private AreaState _state = AreaState.Waiting;

    private void OnEnable()
    {
        if (_intro == null)
        {
            return;
        }

        _intro.Completed += HandleIntroCompleted;
        _intro.Cancelled += HandleIntroCancelled;
    }

    private void Start()
    {
        EncounterHistory history = EncounterHistory.Instance;

        if (history != null && history.IsEliminate(_eliminateId))
        {
            _state = AreaState.Cleared;
            OpenBlocks();
        }
    }

    private void OpenBlocks()
    {
        _leftBlock?.OpenImmediate();
        _rightBlock?.OpenImmediate();
    }

    public void TryEnter()
    {
        if (_state != AreaState.Waiting)
        {
            return;
        }

        EncounterHistory history = EncounterHistory.Instance;

        if (history == null)
        {
            return;
        }

        if (history.IsEliminate(_eliminateId))
        {
            _state = AreaState.Cleared;
            OpenBlocks();
            return;
        }

        if (_useIntro && _intro == null)
        {
            return;
        }

        _leftBlock?.Close();
        _rightBlock?.Close();

        if (!_useIntro)
        {
            StartBattle();
            return;
        }

        bool firstEncounter = !history.HasSeenIntro(_encounterId);

        _state = AreaState.Intro;

        bool accepted;

        if (firstEncounter)
        {
            accepted = _intro.TryPlay();
        }
        else
        {
            accepted = _intro.TryPlayWithoutCinematic();
        }

        if (!accepted)
        {
            HandleIntroCancelled();
        }
    }

    public void HandleIntroCompleted()
    {
        if (_state != AreaState.Intro)
        {
            return;
        }

        EncounterHistory history = EncounterHistory.Instance;

        if (history != null)
        {
            history.MarkIntroSeen(_encounterId);
        }

        StartBattle();
    }

    public void HandleIntroCancelled()
    {
        if (_state != AreaState.Intro)
        {
            return;
        }

        _state = AreaState.Waiting;
        OpenBlocks();
    }

    private void StartBattle()
    {
        _state = AreaState.Fighting;
        _onBattleStarted.Invoke();
    }

    public void HandleEnemyEliminated()
    {
        if (_state != AreaState.Fighting)
        {
            return;
        }

        _state = AreaState.Cleared;

        EncounterHistory history = EncounterHistory.Instance;

        if (history != null)
        {
            history.MarkEliminate(_eliminateId);
        }

        OpenBlocks();
    }

    private void OnDisable()
    {
        if (_state == AreaState.Intro)
        {
            _intro?.Cancel();

            _state = AreaState.Waiting;
            OpenBlocks();
        }

        if (_intro != null)
        {
            _intro.Completed -= HandleIntroCompleted;
            _intro.Cancelled -= HandleIntroCancelled;
        }
    }

}
