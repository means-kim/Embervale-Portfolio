using UnityEngine;
using System;

public abstract class BossAttack : MonoBehaviour
{
    protected BossAttackData _data;
    protected Animator _animator;
    protected bool _isAttacking = false;
    protected bool _isRecoverying = false;
    protected float _coolTimer;
    protected float _recoveryTimer;

    public bool IsAttacking => _isAttacking;
    public bool IsRecoverying => _isRecoverying;

    public event Action OnAttackFinished;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (_coolTimer > 0f)
        {
            _coolTimer -= Time.deltaTime;
        }

        if (_isRecoverying)
        {
            _recoveryTimer -= Time.deltaTime;

            if (_recoveryTimer <= 0f)
            {
                _isRecoverying = false;
                _isAttacking = false;
                NotifyAttackFinished();
            }
        }
    }

    public BossAttackData Data
    {
        get { return _data; }
    }

    public void SetData(BossAttackData data)
    {
        _data = data;
    }

    protected void NotifyAttackFinished()
    {
        OnAttackFinished?.Invoke();
    }

    public abstract void StartAttack();
    public abstract void AttackAction();
    public abstract void EndAttack();
    public abstract void CancelAttack();
}