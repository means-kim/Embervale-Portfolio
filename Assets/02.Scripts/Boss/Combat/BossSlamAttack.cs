using UnityEngine;

public class BossSlamAttack : BossAttack
{
    public override void StartAttack()
    {
        if (_isAttacking) return;

        if (_data == null || _animator == null)
        {
            Debug.Log($"_data : {_data}, _animator : {_animator}");
            return;
        }

        _isAttacking = true;

        _animator.SetTrigger("IsSlam");
    }

    public override void TrickAction()
    {
        if (!_isAttacking) return;

        // Todo: 애니메이션 VFX 이벤트 함수 호출
    }

    public override void EndAttck()
    {
        if (!_isAttacking) return;

        _isAttacking = false;
        _animator.ResetTrigger("IsSlam");
        NotifyAttackFinished();
    }

    public override void CancleAttack()
    {
        _isAttacking = false;

        if (_animator != null)
        {
            _animator.ResetTrigger("IsSlam");
        }
    }
}