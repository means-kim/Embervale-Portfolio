using System;
using UnityEngine;

public class EliteMonsterHit : MonoBehaviour
{
    [SerializeField] private Health _bossHealth;
    [SerializeField] private EliteMonsterMovement _eliteMonsterMovement;
    [SerializeField] private EliteMonsterFacing _eliteMonsterFacing;
    [SerializeField] private EliteMonsterAttackController _eliteMonsterAttackController;
    [SerializeField] private Animator _animator;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Material _hitMaterial;
    private Material _originalMaterial;

    private bool _isHit = false;
    public bool IsHit => _isHit;

    [SerializeField] private float _flashInterval = 0.08f;
    [SerializeField] private float _flashDuration = 0.24f;
    private float _flashRemaining;
    private float _flashTimer;
    private bool _isWhiteFlash = false;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _originalMaterial = _spriteRenderer.sharedMaterial;
    }

    private void Update()
    {
        if (_flashRemaining <= 0f) return;

        _flashRemaining -= Time.deltaTime;

        if (_bossHealth.IsDead || _flashRemaining <= 0f)
        {
            StopFlash();
            return;
        }

        _flashTimer -= Time.deltaTime;
        if (_flashTimer > 0f) return;

        _flashTimer = _flashInterval;
        _isWhiteFlash = !_isWhiteFlash;
        _spriteRenderer.sharedMaterial =
            _isWhiteFlash ? _hitMaterial : _originalMaterial;
    }

    private void OnEnable()
    {
        _bossHealth.OnHit += HandleHit;
        _bossHealth.OnDie += ClearHit;
    }

    private void OnDisable()
    {
        _bossHealth.OnHit -= HandleHit;
        _bossHealth.OnDie -= ClearHit;

        ClearHit();
    }

    private void HandleHit()
    {
        if (_bossHealth.IsDead) return;

        StartFlash();

        if (_eliteMonsterAttackController.IsAttacking || _isHit)
            return;

        _isHit = true;
        _eliteMonsterMovement.enabled = false;
        _eliteMonsterFacing.enabled = false;

        _animator.SetBool("IsWalk", false);
        _animator.SetTrigger("IsHit");
    }

    private void EndHit()
    {
        if (!_isHit) return;

        _isHit = false;

        if (_bossHealth.IsDead) return;
        if (_eliteMonsterAttackController.IsAttacking) return;

        _eliteMonsterMovement.enabled = true;
        _eliteMonsterFacing.enabled = true;
    }

    private void ClearHit()
    {
        _isHit = false;
        StopFlash();
    }

    [ContextMenu("Test Hit")]
    private void TestHit()
    {
        if (!Application.isPlaying) return;

        HandleHit();
    }

    private void StartFlash()
    {
        _flashRemaining = _flashDuration;
        _flashTimer = _flashInterval;
        _isWhiteFlash = true;
        _spriteRenderer.sharedMaterial = _hitMaterial;
    }

    private void StopFlash()
    {
        _flashRemaining = 0f;
        _flashTimer = 0f;
        _isWhiteFlash = false;
        _spriteRenderer.sharedMaterial = _originalMaterial;
    }
}