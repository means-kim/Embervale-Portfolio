using UnityEngine;

public class EliteMonsterChargeAttack : EliteMonsterAttack
{
    [SerializeField] private EliteMonsterMovement _movement;
    [SerializeField] private float _moveSpeedUp;

    [SerializeField] private EliteMonsterEffect _effectPrefab;
    [SerializeField] private Transform _effectPoint;

    private bool _isChargeMove;

    public override void StartAttack()
    {
        if (_isAttacking) return;

        if (_coolTimer > 0f) return;

        if (_data == null || _animator == null)
        {
            Debug.Log($"_data : {_data}, _animator : {_animator}");
            return;
        }

        _isAttacking = true;
        _coolTimer = _data.CoolTime;

        _animator.SetTrigger("IsCharge");
    }

    public override void AttackAction()
    {
        if (!_isAttacking) return;

        EffectManager.Instance.PlayBossEffect(_effectPrefab, _effectPoint);
    }

    public override void EndAttack()
    {
        if (!_isAttacking) return;
        if (_isRecoverying) return;

        StopChargeMovement();

        _isRecoverying = true;
        _animator.ResetTrigger("IsCharge");
        _recoveryTimer = _data.RecoveryTime;
    }

    public override void CancelAttack()
    {
        _isAttacking = false;
        _isRecoverying = false;
        _recoveryTimer = 0f;

        StopChargeMovement();

        if (_animator != null)
        {
            _animator.ResetTrigger("IsCharge");
        }
    }

    public void BeginChargeMovement()
    {
        if (!_isAttacking || _isRecoverying) return;
        if (_isChargeMove) return;

        _isChargeMove = true;

        _movement.ChargeMovement(_moveSpeedUp);
        _movement.enabled = true;
    }

    public void StopChargeMovement()
    {
        if (_isChargeMove)
        {
            _movement.ChargeMovement(-_moveSpeedUp);
            _isChargeMove = false;
        }

        _movement.enabled = false;

        if (_animator != null)
        {
            _animator.SetBool("IsWalk", false);
        }
    }
}