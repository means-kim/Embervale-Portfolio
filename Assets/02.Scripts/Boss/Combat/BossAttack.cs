using UnityEngine;

public abstract class BossAttack : MonoBehaviour
{
    protected BossAttackData _data;
    protected Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public BossAttackData Data
    {
        get { return _data; }
    }

    public BossAttackType Type
    {
        get { return _data.Type; }
    }

    public void SetData(BossAttackData data)
    {
        _data = data;
    }

    public abstract void StartAttack();
    public abstract void TrickAction();
    public abstract void EndAttck();
}