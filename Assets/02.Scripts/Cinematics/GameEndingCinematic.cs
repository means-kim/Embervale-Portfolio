using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class GameEndingCinematic : CinematicSequence
{
    [SerializeField] private CinematicManager _cinematicManager;
    [SerializeField] private AirBossAnimation _bossAnimation;
    [SerializeField] private CanvasGroup _screenFade;

    [SerializeField, Range(0.01f, 1f)] private float _slowTimeScale = 0.05f;

    [SerializeField, Min(0f)] private float _slowDuration = 3f;
    [SerializeField, Min(0f)] private float _restoreDuration = 1.5f;
    [SerializeField, Min(0f)] private float _hitShakeStrength = 2f;

    [SerializeField, Min(0f)] private float _earthquakeStrength = 1f;
    [SerializeField, Min(0f)] private float _earthquakeHoldDuration = 1f;
    [SerializeField, Min(0f)] private float _fadeDuration = 2f;

    public bool IsPlaying => _requested;
    private Animator _bossAnimator;
    private bool _requested;
    private bool _stateCaptured;

    private float _originalTimeScale;
    private float _originalFixedDeltaTime;
    private float _originalFadeAlpha;
    private bool _originalBlocksRaycasts;

    public override bool TryPlay()
    {
        if (!isActiveAndEnabled || _requested)
        {
            return false;
        }

        if (_cinematicManager == null || _bossAnimation == null || _screenFade == null)
        {
            return false;
        }

        _bossAnimator = _bossAnimation.GetComponent<Animator>();
        if (_bossAnimator == null || !_bossAnimator.isActiveAndEnabled ||
            _bossAnimator.runtimeAnimatorController == null)
        {
            return false;
        }

        // 현재 AirBoss Animator의 Base Layer에 있는 Death 상태.
        if (!_bossAnimator.HasState(0, Animator.StringToHash("Base Layer.Death")))
        {
            Debug.LogWarning("보스 Animator에 Death 상태가 없습니다.", this);
            return false;
        }

        _requested = true;

        bool accepted = _cinematicManager.Play(EndingRoutine(), HandleCompleted, HandleCancelled);

        if (!accepted)
        {
            _requested = false;
        }

        return accepted;
    }

    private IEnumerator EndingRoutine()
    {
        while (HitImpactFeedback.Instance != null && HitImpactFeedback.Instance.IsStopping)
        {
            yield return null;
        }

        while (Time.timeScale <= 0f)
        {
            yield return null;
        }

        CaptureState();

        _screenFade.alpha = 0f;
        _screenFade.blocksRaycasts = true;

        float slowScale = Mathf.Min(_slowTimeScale, _originalTimeScale);

        SetTimeScale(slowScale);

        CameraManager.ShakeCamera(_hitShakeStrength, _slowDuration);

        yield return new WaitForSecondsRealtime(_slowDuration);

        _bossAnimation.PlayDeathAnimation();

        yield return RestoreSpeedRoutine(slowScale);

        yield return WaitForDeathRoutine();

        CameraManager.ShakeCamera(_earthquakeStrength, _earthquakeHoldDuration + _fadeDuration);

        yield return new WaitForSecondsRealtime(_earthquakeHoldDuration);

        yield return FadeToBlackRoutine();
    }

    private void CaptureState()
    {
        _originalTimeScale = Time.timeScale;
        _originalFixedDeltaTime = Time.fixedDeltaTime;
        _originalFadeAlpha = _screenFade.alpha;
        _originalBlocksRaycasts = _screenFade.blocksRaycasts;
        _stateCaptured = true;
    }

    private void SetTimeScale(float scale)
    {
        Time.timeScale = scale;

        // 슬로모션에서도 물리 갱신이 지나치게 뜸해지지 않게 조절.
        Time.fixedDeltaTime = _originalFixedDeltaTime * (scale / _originalTimeScale);
    }

    private IEnumerator RestoreSpeedRoutine(float startScale)
    {
        float elapsed = 0f;

        while (elapsed < _restoreDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / _restoreDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            SetTimeScale(Mathf.Lerp(startScale, _originalTimeScale, t));

            yield return null;
        }

        RestoreTime();
    }

    private IEnumerator WaitForDeathRoutine()
    {
        while (true)
        {
            AnimatorStateInfo state = _bossAnimator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("Death") && state.normalizedTime >= 1f && !_bossAnimator.IsInTransition(0))
            {
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator FadeToBlackRoutine()
    {
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            _screenFade.alpha = Mathf.Clamp01(elapsed / _fadeDuration);

            yield return null;
        }

        _screenFade.alpha = 1f;
    }

    private void RestoreTime()
    {
        if (!_stateCaptured)
        {
            return;
        }

        Time.timeScale = _originalTimeScale;
        Time.fixedDeltaTime = _originalFixedDeltaTime;
    }


    private void HandleCompleted()
    {
        RestoreTime();

        _stateCaptured = false;
        _requested = false;

        NotifyCompleted();
    }

    private void HandleCancelled()
    {
        RestoreTime();

        if (_stateCaptured && _screenFade != null)
        {
            _screenFade.alpha = _originalFadeAlpha;
            _screenFade.blocksRaycasts = _originalBlocksRaycasts;
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

    [ContextMenu("Test/엔딩 사망 연출")]
    private void TestEnding()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("Play 모드에서 실행하세요.", this);
            return;
        }

        Debug.Log($"엔딩 재생: {TryPlay()}", this);
    }
}