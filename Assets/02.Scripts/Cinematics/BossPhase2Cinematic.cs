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

    [Header("보스 연출 위치 (선택)")]
    [SerializeField] private Transform _phase2Position;
    [SerializeField, Min(0.01f)] private float _moveDuration = 0.5f;

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

        if (_cinematicManager == null || _director == null || _boss == null
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
        bool accepted = _cinematicManager.Play(
            _director, HandleCompleted, PrepareRoutine, HandleCancelled);

        if (!accepted)
        {
            _requested = false;
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

        if (_phase2Position != null)
        {
            Vector3 start = _boss.transform.position;
            Vector3 target = _phase2Position.position;
            float elapsed = 0f;
            float duration = Mathf.Max(0.01f, _moveDuration);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
                _boss.transform.position = Vector3.Lerp(start, target, t);
                yield return null;
            }

            _boss.transform.position = target;
        }

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

    // Timeline Signal: 가림 시작. 마스크 Scale 애니메이션은 Timeline에서 재생한다.
    public void CoverMoon()
    {
        if (!_requested || !_stateCaptured)
        {
            return;
        }

        _coverRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        _moonCover.SetActive(true);
        _moonLight.enabled = false;
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
    }

    private void HandleCompleted()
    {
        FinishMoonCover();
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

            if (_boss != null)
            {
                _boss.transform.position = _bossOriginalPosition;
            }
        }

        _stateCaptured = false;
        _requested = false;
        NotifyCancelled();
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
}