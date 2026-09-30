using UnityEngine;

public class EliteMonsterDeath : MonoBehaviour
{
    [SerializeField] private EliteMonsterMovement _eliteMonsterMovement;
    [SerializeField] private Health _bossHealth;
    [SerializeField] private EliteMonsterFacing _eliteMonsterFacing;
    [SerializeField] private Animator _animator;
    [SerializeField] private EliteMonsterAttackController _eliteMonsterAttackController;

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
        _eliteMonsterAttackController.CancelCurrentAttack();

        _eliteMonsterMovement.enabled = false;
        _eliteMonsterFacing.enabled = false;

        _animator.SetBool("IsWalk", false);
        _animator.SetTrigger("IsDeath");
    }
}