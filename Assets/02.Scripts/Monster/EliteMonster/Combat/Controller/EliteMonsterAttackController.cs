using UnityEngine;

public class EliteMonsterAttackController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private EliteMonsterSlamAttack _slamAttack;
    [SerializeField] private EliteMonsterSwingAttack _swingAttack;
    [SerializeField] private EliteMonsterChargeAttack _chargeAttack;
    [SerializeField] private EliteMonsterIceThornAttack _iceThornAttack;
    [SerializeField] private EliteMonsterAttackDataSO _attackDataSo;
    [SerializeField] private EliteMonsterAttackSellector _eliteMonsterAttackSellector;
    [SerializeField] private EliteMonsterHit _eliteMonsterHit;

    [SerializeField] private EliteMonsterMovement _movement;
    [SerializeField] private EliteMonsterFacing _facing;
    [SerializeField] private Health _health;
    [SerializeField] private Animator _animator;

    private EliteMonsterAttack _currentAttack;
    public bool IsAttacking => _currentAttack != null;

    private void Start()
    {
        AttackDataSettings();
        FindTarget();
    }

    private void Update()
    {
        if (_health.IsDead) return;
        if (_target == null) FindTarget();
        if (_target == null) return;
        if (_currentAttack != null) return;
        if (_eliteMonsterHit.IsHit) return;
        if (!_movement.IsChasing) return;

        float distance = Mathf.Abs(_target.position.x - transform.position.x);
        EliteMonsterAttack sellectAttack = _eliteMonsterAttackSellector.SellectAttack(distance);

        if (sellectAttack == null) return;

        _currentAttack = sellectAttack;
        _currentAttack.OnAttackFinished += HandleAttackFinsed;
        foreach (EliteMonsterAttackHitbox hitbox in GetComponentsInChildren<EliteMonsterAttackHitbox>(true))
        {
            hitbox.ResetHits();
        }

        _currentAttack.StartAttack();

        _movement.enabled = false;
        _facing.enabled = false;
        _animator.SetBool("IsWalk", false);
    }

    private void OnDisable()
    {
        CancelCurrentAttack();

        if (_health != null && !_health.IsDead)
        {
            _movement.enabled = true;
            _facing.enabled = true;
        }
    }

    private void HandleAttackFinsed()
    {
        CancelCurrentAttack();


        if (_health.IsDead) return;

        _movement.enabled = true;
        _facing.enabled = true;
    }

    public void CancelCurrentAttack()
    {
        if (_currentAttack == null) return;

        _currentAttack.OnAttackFinished -= HandleAttackFinsed;
        _currentAttack.CancelAttack();
        _currentAttack = null;
    }

    private void FindTarget()
    {
        if (_target != null) return;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _target = player.transform;
    }

    private void AttackDataSettings()
    {
        foreach (EliteMonsterAttackData data in _attackDataSo.Attacks)
        {
            if (data == null) continue;

            switch (data.Type)
            {
                case EliteMonsterAttackType.Slam:
                    _slamAttack.SetData(data);
                    break;

                case EliteMonsterAttackType.Swing:
                    _swingAttack.SetData(data);
                    break;

                case EliteMonsterAttackType.Charge:
                    _chargeAttack.SetData(data);
                    break;

                case EliteMonsterAttackType.IceThorn:
                    _iceThornAttack.SetData(data);
                    break;
            }
        }
    }
}