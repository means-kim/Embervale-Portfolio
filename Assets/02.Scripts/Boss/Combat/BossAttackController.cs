using UnityEngine;

public class BossAttackController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private BossSlamAttack _slamAttack;
    [SerializeField] private BossAttackDataSO _attackDataSo;

    [SerializeField] private BossMovement _movement;
    [SerializeField] private BossFacing _facing;
    [SerializeField] private Health _health;
    [SerializeField] private Animator _animator;

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

        float distance = Mathf.Abs(_target.position.x - transform.position.x);
        BossAttackData data = _slamAttack.Data;

        if (distance <= data.MaxDistance && distance >= data.MinDistance)
        {
            _slamAttack.StartAttack();

            if (!_slamAttack.IsAttacking) return;

            _movement.enabled = false;
            _facing.enabled = false;
            _animator.SetBool("IsWalk", false);
        }
    }

    private void OnEnable()
    {
        _slamAttack.OnAttackFinished += HandleAttackFinsed;
    }

    private void OnDisable()
    {
        _slamAttack.OnAttackFinished -= HandleAttackFinsed;
    }

    private void HandleAttackFinsed()
    {
        if (_health.IsDead) return;

        _movement.enabled = true;
        _facing.enabled = true;
    }
}