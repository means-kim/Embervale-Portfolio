using UnityEngine;

public class EliteMonsterDeath : MonoBehaviour
{
    [SerializeField] private EliteMonsterMovement _eliteMonsterMovement;
    [SerializeField] private Health _bossHealth;
    [SerializeField] private EliteMonsterFacing _eliteMonsterFacing;
    [SerializeField] private Animator _animator;
    [SerializeField] private EliteMonsterAttackController _eliteMonsterAttackController;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private CapsuleCollider2D _bodyCollider;

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
        EffectManager.Instance?.PlayBossDeath(transform.position);

        _eliteMonsterAttackController.CancelCurrentAttack();

        _eliteMonsterMovement.enabled = false;
        _eliteMonsterFacing.enabled = false;

        DisableBodyCollision();

        _animator.SetBool("IsWalk", false);
        _animator.SetTrigger("IsDeath");
    }

    private void DisableBodyCollision()
    {
        if (!_bossHealth.IsDead) return;

        _rigidBody.simulated = false;
        _bodyCollider.enabled = false;
    }
}
