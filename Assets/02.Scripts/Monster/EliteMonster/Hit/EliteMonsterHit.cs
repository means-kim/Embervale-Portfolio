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
    private float _flashTimer;
    private bool _isWhiteFlash = false;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _originalMaterial = _spriteRenderer.sharedMaterial;
    }

    private void Update()
    {
        if (!_isHit) return;
        if (_bossHealth.IsDead) return;

        _flashTimer -= Time.deltaTime;
        if (_flashTimer > 0f) return;

        _flashTimer = _flashInterval;
        _isWhiteFlash = !_isWhiteFlash;

        _spriteRenderer.sharedMaterial = _isWhiteFlash ? _hitMaterial : _originalMaterial;
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
        if (_eliteMonsterAttackController.IsAttacking) return;
        if (_bossHealth.IsDead) return;
        if (_isHit) return;

        _isHit = true;
        _isWhiteFlash = true;
        _flashTimer = _flashInterval;
        _spriteRenderer.sharedMaterial = _hitMaterial;

        _eliteMonsterMovement.enabled = false;
        _eliteMonsterFacing.enabled = false;

        _animator.SetBool("IsWalk", false);
        _animator.SetTrigger("IsHit");
    }

    private void EndHit()
    {
        if (!_isHit) return;

        ClearHit();

        if (_bossHealth.IsDead) return;
        if (_eliteMonsterAttackController.IsAttacking) return;

        _eliteMonsterMovement.enabled = true;
        _eliteMonsterFacing.enabled = true;
    }

    private void ClearHit()
    {
        _isHit = false;
        _isWhiteFlash = false;
        _flashTimer = 0f;
        _spriteRenderer.sharedMaterial = _originalMaterial;
    }

    [ContextMenu("Test Hit")]
    private void TestHit()
    {
        if (!Application.isPlaying) return;

        HandleHit();
    }
}