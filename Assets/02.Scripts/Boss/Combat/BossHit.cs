using System;
using UnityEngine;

public class BossHit : MonoBehaviour
{
    [SerializeField] private Health _bossHealth;
    [SerializeField] private BossMovement _bossMovement;
    [SerializeField] private BossFacing _bossFacing;
    [SerializeField] private BossAttackController _bossAttackController;
    [SerializeField] private Animator _animator;

    private bool _isHit = false;
    public bool IsHit => _isHit;

    private void OnEnable()
    {
        _bossHealth.OnHit += HandleHit;
    }

    private void OnDisable()
    {
        _bossHealth.OnHit -= HandleHit;
    }

    private void HandleHit()
    {
        if (_bossAttackController.IsAttacking) return;

        _isHit = true;

        _bossMovement.enabled = false;
        _bossFacing.enabled = false;

        _animator.SetBool("IsWalk", false);
        _animator.SetTrigger("IsHit");
    }
}