using System.Collections;
using EndingFX;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
public class GameEndingCinematic : CinematicSequence
{
    [SerializeField] private CinematicManager _cinematicManager;
    [SerializeField] private AirBossAnimation _bossAnimation;
    [SerializeField] private CanvasGroup _screenFade;
    [SerializeField] private bool _playOnBossDeath;
    [SerializeField] private string _titleSceneName = "OutGameScene";

    [SerializeField, Range(0.01f, 1f)] private float _slowTimeScale = 0.05f;

    [SerializeField, Min(0f)] private float _slowDuration = 4.5f;
    [FormerlySerializedAs("_restoreDuration")]
    [SerializeField, Min(0f)] private float _finalHitShakeDuration = 0.35f;
    [SerializeField, Min(0f)] private float _hitShakeStrength = 2f;

    [SerializeField, Min(0f)] private float _earthquakeStrength = 0.6f;
    [SerializeField, Min(0f)] private float _earthquakeHoldDuration = 0.5f;
    [SerializeField, Min(0f)] private float _fadeDuration = 2.2f;
    [SerializeField, Min(0f)] private float _blackScreenHoldDuration = 0.35f;
    [Header("Achievement after blackout")]
    [SerializeField] private EndingAchievementVideo _achievementVideo;
    [SerializeField, Min(0f)] private float _afterVideoHoldDuration = 0.6f;
    [Header("Interactive ending")]
    [SerializeField] private EndingHitMontage _hitMontage;
    [SerializeField] private BossDissolveEffect _bossDissolve;
    [SerializeField] private Transform _hitTarget;
    [SerializeField] private CanvasGroup _endingPrompt;
    [SerializeField] private string _roarStateName = "BothArmsRaise";
    [SerializeField, Min(0f)] private float _glowDuration = 0.8f;
    [SerializeField, Min(0f)] private float _montageHitShakeStrength = 1f;
    [FormerlySerializedAs("_deathTimeout")]
    [SerializeField, Min(0f)] private float _deathVisibleDuration = 1.2f;
    [Header("Boss lighting during hits")]
    [SerializeField] private Light2D _bossHitLight;
    [SerializeField, Min(0f)] private float _hitLightIntensity = 0.3f;
    [SerializeField, Min(0f)] private float _hitLightBlendDuration = 0.25f;
    private float _originalLightIntensity;
    private bool _originalLightEnabled;
    private bool _lightCaptured;
    [Header("Ending shake (real time)")]
    [SerializeField, Min(0f)] private float _endingShakeStrength = 0.15f;
    [SerializeField, Min(0.02f)] private float _endingShakeInterval = 0.045f;
    [SerializeField, Min(0.01f)] private float _endingShakeDuration = 0.07f;
    private float _nextShakeTime;
    private bool _shakeEnding;
    private AnimatorUpdateMode _originalBossUpdateMode;
    private int _originalAnimatorState;
    private float _originalAnimatorTime;

    public bool HandlesBossDeath(AirBossAnimation boss) =>
        isActiveAndEnabled && _playOnBossDeath && _bossAnimation == boss;

    public bool IsPlaying => _requested;
    private Animator _bossAnimator;
    private bool _requested;
    private bool _stateCaptured;

    private float _originalTimeScale;
    private float _originalFixedDeltaTime;
    private float _originalFadeAlpha;
    private bool _originalBlocksRaycasts;
    private Health _bossHealth;
    private Coroutine _deathRequest;

    private void OnEnable()
    {
        if (_hitMontage != null) _hitMontage.HitLanded += HandleMontageHit;
        if (_playOnBossDeath && _bossAnimation != null)
        {
            _bossHealth = _bossAnimation.GetComponent<Health>();
            if (_bossHealth != null)
            {
                _bossHealth.OnDie += HandleBossDeath;
            }
        }
    }

    private void HandleBossDeath()
    {
        if (_requested || _deathRequest != null)
        {
            return;
        }

        _deathRequest = StartCoroutine(PlayAfterDeathCleanup());
    }

    private IEnumerator PlayAfterDeathCleanup()
    {
        // 사망 이벤트의 다른 구독자가 공격과 페이즈 연출을 정리한 뒤 시작한다.
        yield return null;
        _deathRequest = null;
        if (!TryPlay())
        {
            _bossAnimation?.PlayDeathAnimation();
        }
    }

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

        if (string.IsNullOrWhiteSpace(_titleSceneName) || !Application.CanStreamedLevelBeLoaded(_titleSceneName))
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
            return false;
        }

        _requested = true;

        bool accepted = _cinematicManager.Play(EndingRoutine(), HandleCompleted, HandleCancelled);

        // Ending input remains available; slow game time alone controls the descent.
        if (accepted) _cinematicManager.ReleasePlayerControls();

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
        _achievementVideo?.Prepare();
        _shakeEnding = true;
        _nextShakeTime = Time.unscaledTime;

        _screenFade.alpha = 0f;
        _screenFade.blocksRaycasts = true;

        float slowScale = Mathf.Min(Mathf.Max(0.01f, _slowTimeScale), _originalTimeScale);

        SetTimeScale(slowScale);
        if (!string.IsNullOrWhiteSpace(_roarStateName))
        {
            int hash = Animator.StringToHash("Base Layer." + _roarStateName);
            if (_bossAnimator.HasState(0, hash)) _bossAnimator.Play(hash, 0, 0);
        }

        if (_bossDissolve != null) _bossDissolve.BindBoss(_bossAnimation.GetComponent<SpriteRenderer>(), _bossAnimator);
        if (_hitMontage != null && _hitMontage.IsConfigured)
        {
            yield return BrightenBossRoutine();
            if (_endingPrompt != null)
            {
                _endingPrompt.alpha = 1;
            }
            Transform target = _hitTarget != null ? _hitTarget : _bossAnimation.transform;
            yield return _hitMontage.PlayHits(target, _slowDuration);
            if (_endingPrompt != null)
            {
                _endingPrompt.alpha = 0;
            }
            yield return _hitMontage.PlayFinalHit(target);
            yield return RestoreBossLightRoutine();
            _bossDissolve?.BeginGlow();
            yield return new WaitForSecondsRealtime(_glowDuration);
        }
        else
        {
            yield return new WaitForSecondsRealtime(_slowDuration);
        }

        CameraManager.ShakeCamera(_hitShakeStrength, Mathf.Max(0.1f, _finalHitShakeDuration));
        _bossDissolve?.Dissolve();
        // Boss death progresses independently while the player stays in slow motion.
        _bossAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        _bossAnimation.PlayDeathAnimation();

        yield return WaitForDeathRoutine();

        CameraManager.ShakeCamera(_earthquakeStrength, _earthquakeHoldDuration + _fadeDuration);

        yield return new WaitForSecondsRealtime(_earthquakeHoldDuration);

        yield return FadeToBlackRoutine();
        yield return new WaitForSecondsRealtime(_blackScreenHoldDuration);
        if (_achievementVideo != null) yield return _achievementVideo.PlayRoutine();
        yield return new WaitForSecondsRealtime(_afterVideoHoldDuration);
    }

    private void CaptureState()
    {
        _originalTimeScale = Time.timeScale;
        _originalFixedDeltaTime = Time.fixedDeltaTime;
        _originalFadeAlpha = _screenFade.alpha;
        _originalBlocksRaycasts = _screenFade.blocksRaycasts;
        _stateCaptured = true;
        var state = _bossAnimator.GetCurrentAnimatorStateInfo(0);
        _originalAnimatorState = state.fullPathHash;
        _originalAnimatorTime = state.normalizedTime;
        _originalBossUpdateMode = _bossAnimator.updateMode;
    }

    private void HandleMontageHit(float progress)
    {
        if (_requested && _stateCaptured)
        {
            CameraManager.ShakeCamera(_montageHitShakeStrength * (1f + progress), 0.08f);
        }
    }

    private IEnumerator BrightenBossRoutine()
    {
        if (_bossHitLight == null)
        {
            yield break;
        }
        _originalLightIntensity = _bossHitLight.intensity;
        _originalLightEnabled = _bossHitLight.enabled;
        _lightCaptured = true;
        _bossHitLight.enabled = true;
        yield return BlendBossLightRoutine(Mathf.Max(_originalLightIntensity, _hitLightIntensity));
    }

    private IEnumerator RestoreBossLightRoutine()
    {
        if (!_lightCaptured)
        {
            yield break;
        }
        yield return BlendBossLightRoutine(_originalLightIntensity);
        RestoreBossLight();
    }

    private IEnumerator BlendBossLightRoutine(float target)
    {
        if (_bossHitLight == null)
        {
            yield break;
        }
        float start = _bossHitLight.intensity;
        float elapsed = 0;
        while (elapsed < _hitLightBlendDuration && _bossHitLight != null)
        {
            elapsed += Time.unscaledDeltaTime;
            _bossHitLight.intensity = Mathf.Lerp(start, target,
                Mathf.SmoothStep(0, 1, Mathf.Clamp01(elapsed / _hitLightBlendDuration)));
            yield return null;
        }

        if (_bossHitLight != null)
        {
            _bossHitLight.intensity = target;
        }
    }

    private void RestoreBossLight()
    {
        if (!_lightCaptured)
        {
            return;
        }
        _lightCaptured = false;
        if (_bossHitLight == null)
        {
            return;
        }
        _bossHitLight.intensity = _originalLightIntensity;
        _bossHitLight.enabled = _originalLightEnabled;
    }

    private void SetTimeScale(float scale)
    {
        Time.timeScale = scale;

        // 슬로모션에서도 물리 갱신이 지나치게 뜸해지지 않게 조절.
        Time.fixedDeltaTime = _originalFixedDeltaTime * (scale / _originalTimeScale);
    }

    private IEnumerator WaitForDeathRoutine()
    {
        float deadline = Time.unscaledTime + _deathVisibleDuration;
        while (Time.unscaledTime < deadline && _bossAnimator != null && _bossAnimator.isActiveAndEnabled)
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
        _shakeEnding = false;
    }

    private void Update()
    {
        if (!_shakeEnding || Time.unscaledTime < _nextShakeTime) return;
        _nextShakeTime = Time.unscaledTime + _endingShakeInterval;
        CameraManager.ShakeCamera(_endingShakeStrength, _endingShakeDuration);
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
        _achievementVideo?.StopPlayback();
        RestoreBossLight();
        _shakeEnding = false;
        RestoreTime();
        _hitMontage?.Stop();
        if (_endingPrompt != null) _endingPrompt.alpha = 0;

        _stateCaptured = false;
        _requested = false;

        NotifyCompleted();
        SceneManager.LoadSceneAsync(_titleSceneName, LoadSceneMode.Single);
    }

    private void HandleCancelled()
    {
        _achievementVideo?.StopPlayback();
        RestoreBossLight();
        _shakeEnding = false;
        RestoreTime();
        _hitMontage?.Stop();
        _bossDissolve?.ResetEffect();
        if (_endingPrompt != null) _endingPrompt.alpha = 0;
        if (_stateCaptured && _bossAnimator != null)
        {
            _bossAnimator.updateMode = _originalBossUpdateMode;
            _bossAnimator.Play(_originalAnimatorState, 0, _originalAnimatorTime);
            _bossAnimator.Update(0);
            var health = _bossAnimation.GetComponent<Health>();
            if (health != null && health.IsDead)
            {
                _bossAnimation.PlayDeathAnimation();
            }
        }

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
        if (_hitMontage != null)
        {
            _hitMontage.HitLanded -= HandleMontageHit;
        }
        if (_bossHealth != null)
        {
            _bossHealth.OnDie -= HandleBossDeath;
            _bossHealth = null;
        }

        if (_deathRequest != null)
        {
            StopCoroutine(_deathRequest);
            _deathRequest = null;
        }

        Cancel();
        RestoreBossLight();
        _achievementVideo?.StopPlayback();
    }
}
