using UnityEngine;

public class BossSlamAttack : BossAttack
{
    [SerializeField] private Transform _slamEffectTransform;

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

        _animator.SetTrigger("IsSlam");
    }

    public override void AttackAction()
    {
        if (!_isAttacking) return;

        // Todo: 애니메이션 VFX 이벤트 함수 호출

        EffectManager.Instance.PlaySnowEffect(SnowEffectType.Explode, _slamEffectTransform.position);
    }

    public override void EndAttack()
    {
        if (!_isAttacking) return;
        if (_isRecoverying) return;

        _isRecoverying = true;
        _animator.ResetTrigger("IsSlam");
        _recoveryTimer = _data.RecoveryTime;
    }

    public override void CancelAttack()
    {
        _isAttacking = false;
        _isRecoverying = false;
        _recoveryTimer = 0f;

        if (_animator != null)
        {
            _animator.ResetTrigger("IsSlam");
        }
    }
}