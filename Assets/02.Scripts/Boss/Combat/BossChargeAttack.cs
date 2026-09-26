using UnityEngine;

public class BossChargeAttack : BossAttack
{
    [SerializeField] private BossMovement _movement;
    [SerializeField] private float _moveSpeedUp;

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

        // Todo: 애니메이션 VFX 이벤트 함수 호출
    }

    public override void EndAttack()
    {
        if (!_isAttacking) return;
        if (_isRecoverying) return;

        _isRecoverying = true;
        _animator.ResetTrigger("IsCharge");
        _recoveryTimer = _data.RecoveryTime;
    }

    public override void CancelAttack()
    {
        _isAttacking = false;
        _isRecoverying = false;
        _recoveryTimer = 0f;

        if (_animator != null)
        {
            _animator.ResetTrigger("IsCharge");
        }
    }

    public void BeginChargeMovement()
    {
        _movement.ChargeMovement(_moveSpeedUp);
        _movement.enabled = true;
    }

    public void StopChargeMovement()
    {
        _movement.ChargeMovement(-_moveSpeedUp);
        _movement.enabled = false;
        _animator.SetBool("IsWalk", false);
    }
}