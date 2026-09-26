using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private BossSlamAttack _slamAttack;
    [SerializeField] private BossAttackDataSO _attackDataSo;
    [SerializeField] private BossAttackSellector _bossAttackSellector;

    [SerializeField] private BossMovement _movement;
    [SerializeField] private BossFacing _facing;
    [SerializeField] private Health _health;
    [SerializeField] private Animator _animator;

    private BossAttack _currentAttack;

    private void Start()
    {
        for (int i = 0; i < _attackDataSo.Attacks.Length; i++)
        {
            BossAttackData data = _attackDataSo.Attacks[i];

            if (data != null && data.Type == BossAttackType.Slam)
            {
                _slamAttack.SetData(data);
                break;
            }
        }
    }

    private void Update()
    {
        if (_health.IsDead) return;
        if (_currentAttack != null) return;

        float distance = Mathf.Abs(_target.position.x - transform.position.x);
        BossAttack sellectAttack = _bossAttackSellector.SellectAttack(distance);

        if (sellectAttack == null) return;

        _currentAttack = sellectAttack;
        _currentAttack.OnAttackFinished += HandleAttackFinsed;
        _currentAttack.StartAttack();

        _movement.enabled = false;
        _facing.enabled = false;
        _animator.SetBool("IsWalk", false);
    }

    private void OnDisable()
    {
        if (_currentAttack == null) return;

        _currentAttack.OnAttackFinished -= HandleAttackFinsed;
        _currentAttack.CancelAttack();
        _currentAttack = null;

        if (_health != null && !_health.IsDead)
        {
            _movement.enabled = true;
            _facing.enabled = true;
        }
    }

    private void HandleAttackFinsed()
    {
        if (_currentAttack == null) return;

        _currentAttack.OnAttackFinished -= HandleAttackFinsed;
        _currentAttack.CancelAttack();
        _currentAttack = null;

        if (_health.IsDead) return;

        _movement.enabled = true;
        _facing.enabled = true;
    }
}