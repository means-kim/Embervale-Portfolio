using UnityEngine;

public class BossDeath : MonoBehaviour
{
    [SerializeField] private BossMovement _bossMovement;
    [SerializeField] private Health _bossHealth;
    [SerializeField] private BossFacing _bossFacing;
    [SerializeField] private Animator _animator;
    [SerializeField] private BossAttackController _bossAttackController;

    private void OnEnable()
    {
        _bossHealth.OnDie += HandleDeath;
    }

    private void OnDisable()
    {
        _bossHealth.OnDie -= HandleDeath;
    }

    private void HandleDeath()
    {
        _bossAttackController.CancelCurrentAttack();

        _bossMovement.enabled = false;
        _bossFacing.enabled = false;

        _animator.SetBool("IsWalk", false);
        _animator.SetTrigger("IsDeath");
    }
}