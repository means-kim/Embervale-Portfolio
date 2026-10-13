using System;
using UnityEngine;

public class SlimeBHitFlash : MonoBehaviour
{
    [SerializeField] private Material _hitFlashMaterial;
    [SerializeField, Min(0.01f)] private float _duration = 0.05f;

    private Health _health;
    private SpriteRenderer _spriteRenderer;
    private Material _originalMaterial;

    private bool _isFlashing = false;
    private float _remaining;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _health.OnHit += PlayFlash;
        _health.OnDie += StopFlash;
    }

    private void OnDisable()
    {
        _health.OnHit -= PlayFlash;
        _health.OnDie -= StopFlash;

        StopFlash();
    }

    private void Update()
    {
        if (!_isFlashing) return;

        _remaining -= Time.deltaTime;

        if (_remaining <= 0f)
        {
            StopFlash();
        }
    }

    private void PlayFlash()
    {
        if (_health.IsDead || _hitFlashMaterial == null) return;

        if (!_isFlashing)
        {
            _originalMaterial = _spriteRenderer.sharedMaterial;
        }

        _remaining = _duration;
        _isFlashing = true;
        _spriteRenderer.sharedMaterial = _hitFlashMaterial;
    }

    private void StopFlash()
    {
        if (!_isFlashing) return;

        _spriteRenderer.sharedMaterial = _originalMaterial;
        _isFlashing = false;
        _remaining = 0f;
    }
}