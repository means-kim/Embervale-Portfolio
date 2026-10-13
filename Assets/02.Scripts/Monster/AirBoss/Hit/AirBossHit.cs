using System;
using UnityEngine;

public class AirBossHit : MonoBehaviour
{
    [Header("공중 보스 컴포넌트")]
    [SerializeField] private Health _airBossHealth;

    [SerializeField] private SpriteRenderer _airBossSpriteRenderer;

    [Header("피격 화이트 플래시")]
    [SerializeField] private Material _hitMaterial;

    [SerializeField] private float _flashInterval = 0.1f;
    [SerializeField] private float _flashDuration = 0.20f;

    private Material _originalMaterial;
    private float _flashRemaining;
    private float _flashTimer;
    private bool _isWhiteFlash = false;

    private void Awake()
    {
        if (_airBossSpriteRenderer == null)
        {
            _airBossSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        _originalMaterial = _airBossSpriteRenderer.sharedMaterial;
    }

    private void Update()
    {
        if (_flashRemaining <= 0f) return;

        _flashRemaining -= Time.deltaTime;

        if (_airBossHealth == null || _airBossHealth.IsDead || _flashRemaining <= 0f)
        {
            StopFlash();
            return;
        }

        _flashTimer -= Time.deltaTime;
        if (_flashTimer > 0f) return;

        _flashTimer = _flashInterval;
        _isWhiteFlash = !_isWhiteFlash;

        _airBossSpriteRenderer.sharedMaterial = _isWhiteFlash ? _hitMaterial : _originalMaterial;
    }

    private void OnEnable()
    {
        if (_airBossHealth == null) return;

        _airBossHealth.OnHit += HandleHit;
        _airBossHealth.OnDie += StopFlash;
    }

    private void OnDisable()
    {
        if (_airBossHealth != null)
        {
            _airBossHealth.OnHit -= HandleHit;
            _airBossHealth.OnDie -= StopFlash;
        }

        StopFlash();
    }

    private void HandleHit()
    {
        if (_airBossHealth == null || _airBossHealth.IsDead) return;
        if (_airBossSpriteRenderer == null || _hitMaterial == null) return;

        // 이동과 FSM은 유지
        StartFlash();
    }

    private void StartFlash()
    {
        _flashRemaining = _flashDuration;
        _flashTimer = _flashInterval;
        _isWhiteFlash = true;

        _airBossSpriteRenderer.sharedMaterial = _hitMaterial;
    }

    private void StopFlash()
    {
        _flashRemaining = 0f;
        _flashTimer = 0f;
        _isWhiteFlash = false;

        if (_airBossSpriteRenderer != null)
        {
            _airBossSpriteRenderer.sharedMaterial = _originalMaterial;
        }
    }

    [ContextMenu("Test Hit")]
    private void TestHit()
    {
        if (!Application.isPlaying) return;
        HandleHit();
    }
}