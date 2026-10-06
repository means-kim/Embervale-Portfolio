using System;
using System.Collections.Generic;
using UnityEngine;

public class HitImpactFeedback : MonoBehaviour
{
    public static HitImpactFeedback Instance { get; private set; }

    [Serializable]
    private class ImpactSettings
    {
        public GameObject effectPrefab;

        [Min(0f)] public float effectScale = 1f;
        [Min(0.05f)] public float effectLifetime = 2f;
        [Header("실제 시간 기준 화면 정지 시간")]
        [Min(0f)] public float hitstopDuration = 0.05f;
    }

    private struct SpawnedEffect
    {
        public GameObject instance;
        public float destroyAt;
    }

    [Header("보스를 타격했을 때")]
    [SerializeField] private ImpactSettings _bossHit = new ImpactSettings { hitstopDuration = 0.05f };

    [Header("플레이어가 피격됐을 때")]
    [SerializeField] private ImpactSettings _playerHit = new ImpactSettings { hitstopDuration = 0.08f };

    private readonly List<SpawnedEffect> _effects = new();

    private bool _isStopping;
    private float _resumeAt;
    private float _previousTimeScale;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        float now = Time.unscaledTime;

        if (_isStopping && now >= _resumeAt)
        {
            RestoreTimeScale();
        }

        for (int i = _effects.Count - 1; i >= 0; i--)
        {
            SpawnedEffect effect = _effects[i];

            if (effect.instance == null)
            {
                _effects.RemoveAt(i);
                continue;
            }

            if (now < effect.destroyAt)
            {
                continue;
            }

            Destroy(effect.instance);
            _effects.RemoveAt(i);
        }
    }

    public void PlayBossHit(Vector3 position)
    {
        PlayImpact(_bossHit, position);
    }

    public void PlayPlayerHit(Vector3 position)
    {
        PlayImpact(_playerHit, position);
    }

    private void PlayImpact(ImpactSettings settings, Vector3 position)
    {
        if (!isActiveAndEnabled || Instance != this || settings == null)
        {
            return;
        }

        SpawnEffect(settings, position);
        RequestHitstop(settings.hitstopDuration);
    }

    private void SpawnEffect(ImpactSettings settings, Vector3 position)
    {
        if (settings.effectPrefab == null)
        {
            return;
        }

        GameObject effect = Instantiate(
            settings.effectPrefab,
            position,
            Quaternion.identity
        );

        effect.transform.localScale *= Mathf.Max(0f, settings.effectScale);

        // hitstop 중에도 파티클은 실제 시간으로 재생
        foreach (ParticleSystem particle in effect.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = particle.main;
            main.useUnscaledTime = true;
        }

        // 애니메이션 기반 이펙트도 정지 중에 재생한다.
        foreach (Animator animator in effect.GetComponentsInChildren<Animator>(true))
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        _effects.Add(new SpawnedEffect
        {
            instance = effect,
            destroyAt = Time.unscaledTime + Mathf.Max(0.05f, settings.effectLifetime)
        });
    }

    private void RequestHitstop(float duration)
    {
        if (duration <= 0f)
        {
            return;
        }

        float requestedResumeAt = Time.unscaledTime + duration;

        if (_isStopping)
        {
            _resumeAt = Mathf.Max(_resumeAt, requestedResumeAt);
            return;
        }

        // 이미 일시정지된 상태에서는 hitstop을 시작하지 않는다.
        if (Time.timeScale <= 0f)
        {
            return;
        }

        _previousTimeScale = Time.timeScale;
        _resumeAt = requestedResumeAt;
        _isStopping = true;

        Time.timeScale = 0f;
    }

    private void RestoreTimeScale()
    {
        if (!_isStopping)
        {
            return;
        }

        _isStopping = false;

        // 다른 코드가 시간 배율을 변경했다면 덮어쓰지 않는다.
        if (Time.timeScale == 0f)
        {
            Time.timeScale = _previousTimeScale;
        }
    }

    private void OnDisable()
    {
        RestoreTimeScale();
        ClearEffects();
    }

    private void OnDestroy()
    {
        RestoreTimeScale();

        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void ClearEffects()
    {
        foreach (SpawnedEffect effect in _effects)
        {
            if (effect.instance != null)
            {
                Destroy(effect.instance);
            }
        }

        _effects.Clear();
    }
}