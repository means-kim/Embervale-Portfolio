using UnityEngine;
using System;

public abstract class BossAttack : MonoBehaviour
{
    protected BossAttackData _data;
    protected Animator _animator;
    protected bool _isAttacking = false;

    public event Action OnAttackFinished;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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