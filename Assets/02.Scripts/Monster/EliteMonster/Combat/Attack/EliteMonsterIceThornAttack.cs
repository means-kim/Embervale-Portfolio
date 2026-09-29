using UnityEngine;

public class EliteMonsterIceThornAttack : EliteMonsterAttack
{
    [SerializeField] private EliteMonsterEffect _effectPrefab;
    [SerializeField] private Transform _effectPoint;

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

        _animator.SetTrigger("IsIceThorn");
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

        _isRecoverying = true;
        _animator.ResetTrigger("IsIceThorn");
        _recoveryTimer = _data.RecoveryTime;
    }

    public override void CancelAttack()
    {
        _isAttacking = false;
        _isRecoverying = false;
        _recoveryTimer = 0f;

        if (_animator != null)
        {
            _animator.ResetTrigger("IsIceThorn");
        }
    }
}
