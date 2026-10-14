using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;

public class BossPhase2Cinematic : CinematicSequence
{
    [Header("연결")]
    [SerializeField] private CinematicManager _cinematicManager;
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private AirBossController _boss;

    [Header("보스 연출 위치")]
    [SerializeField] private Transform _phase2Position;

    [Header("달 가림")]
    [SerializeField] private GameObject _moonCover;
    [SerializeField] private Light2D _moonLight;
    [SerializeField] private SpriteMask _moonMask;

    public bool IsPlaying => _requested;

    private bool _requested;
    private bool _stateCaptured;
    private SpriteRenderer _coverRenderer;

    // 취소할 때 연출 시작 전의 표시와 위치로 복원한다.
    private bool _coverWasActive;
    private bool _coverRendererWasEnabled;
    private bool _lightWasEnabled;
    private bool _maskWasEnabled;
    private Vector3 _bossOriginalPosition;
    private Vector3 _maskOriginalScale;
    private SpriteMaskInteraction _originalMaskInteraction;

    public override bool TryPlay()
    {
        if (!isActiveAndEnabled || _requested)
        {
            return false;
        }

        if (_cinematicManager == null || _director == null || _boss == null || _phase2Position == null
            || _moonCover == null || _moonLight == null || _moonMask == null)
        {
            return false;
        }

        // 정상 종료 시 완료 콜백이 호출되도록 Timeline은 None으로 설정한다.
        if (_director.extrapolationMode != DirectorWrapMode.None)
        {
            return false;
        }

        _coverRenderer = _moonCover.GetComponent<SpriteRenderer>();
        if (_coverRenderer == null)
        {
            return false;
        }

        _requested = true;
        bool accepted = _cinematicManager.Play(_director, HandleCompleted, PrepareRoutine, HandleCancelled);

        if (!accepted)
        {
            _requested = false;
        }
        else
        {
            EnableSkipping(_cinematicManager);
        }

        return accepted;
    }

    private IEnumerator PrepareRoutine()
    {
        CaptureState();

        _moonCover.SetActive(false);
        _moonMask.enabled = true;
        _coverRenderer.enabled = true;
        _coverRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

        // 준비가 즉시 끝나도 매니저의 코루틴 참조 대입을 먼저 완료한다.
        yield return null;
    }

    private void CaptureState()
    {
        _bossOriginalPosition = _boss.transform.position;
        _coverWasActive = _moonCover.activeSelf;
        _coverRendererWasEnabled = _coverRenderer.enabled;
        _lightWasEnabled = _moonLight.enabled;
        _maskWasEnabled = _moonMask.enabled;
        _maskOriginalScale = _moonMask.transform.localScale;
        _originalMaskInteraction = _coverRenderer.maskInteraction;
        _stateCaptured = true;
    }

    // Timeline Signal: 사라짐 클립이 끝난 뒤 연출 위치로 이동한다.
    public void MoveBossToPhase2Position()
    {
        if (!_requested || !_stateCaptured || _boss == null || _phase2Position == null)
        {
            return;
        }

        _boss.transform.position = _phase2Position.position;
    }

    // Timeline Signal: 연출 위치에서 사라진 뒤 시작 위치로 돌아간다.
    public void ReturnBossToOriginalPosition()
    {
        if (!_requested || !_stateCaptured || _boss == null)
        {
            return;
        }

        _boss.transform.position = _bossOriginalPosition;
    }

    // Timeline Signal: 가림 시작. 마스크 Scale 애니메이션은 Timeline에서 재생한다.
    public void CoverMoon()
    {
        if (!_requested || !_stateCaptured)
        {
            return;
        }

        AudioManager.Instance?.PlayBossPhase2Layers();
        _coverRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        _moonCover.SetActive(true);
    }

    // Timeline Signal: Timeline 종료 후에도 완전히 덮인 상태를 유지한다.
    public void FinishMoonCover()
    {
        if (!_requested || !_stateCaptured)
        {
            return;
        }

        _moonCover.SetActive(true);
        _coverRenderer.maskInteraction = SpriteMaskInteraction.None;
        _moonLight.enabled = false;
        _cinematicManager.ReleasePlayerControls(_director);
    }

    private void HandleCompleted()
    {
        FinishMoonCover();
        ReturnBossToOriginalPosition();
        _stateCaptured = false;
        _requested = false;
        NotifyCompleted();
    }

    private void HandleCancelled()
    {
        if (_stateCaptured)
        {
            if (_coverRenderer != null)
            {
                _coverRenderer.enabled = _coverRendererWasEnabled;
                _coverRenderer.maskInteraction = _originalMaskInteraction;
            }

            if (_moonLight != null)
            {
                _moonLight.enabled = _lightWasEnabled;
            }

            if (_moonMask != null)
            {
                _moonMask.enabled = _maskWasEnabled;
                _moonMask.transform.localScale = _maskOriginalScale;
            }

            if (_moonCover != null)
            {
                _moonCover.SetActive(_coverWasActive);
            }

            ReturnBossToOriginalPosition();
        }

        _stateCaptured = false;
        _requested = false;
        NotifyCancelled();
    }

    public override void Skip()
    {
        if (!_requested) return;
        // The music normally starts via a Timeline Signal, which a skip bypasses.
        AudioManager.Instance?.PlayBossPhase2Layers();
        _cinematicManager.Complete();
    }

    public override void Cancel()
    {
        if (!_requested)
        {
            return;
        }

        if (_cinematicManager != null && _cinematicManager.IsPlaying)
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

    [ContextMenu("Test/2페이즈 연출")]
    private void TestPhase2()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("Play 모드에서 실행하세요.", this);
            return;
        }

        Debug.Log($"2페이즈 연출 재생: {TryPlay()}", this);
    }

    [ContextMenu("Test/2페이즈 중단")]
    private void TestCancelPhase2()
    {
        Cancel();
    }
}
