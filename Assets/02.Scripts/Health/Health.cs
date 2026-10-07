using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private bool _autoRecover;
    [SerializeField, Min(0.05f)] private float _secondsPerSlot = 1.2f;
    private float _recoveryRemainder;
    private int _currentHealth;
    private bool _isDead = false;
    public int MaxHealth => _maxHealth;
    public int CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = value;
    }
    public bool IsDead => _isDead;
    public event Action OnDie;
    public event Action OnHit;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    private void Update()
    {
        AdvanceRecovery(Time.deltaTime);
    }

    public void AdvanceRecovery(float deltaTime)
    {
        if (!_autoRecover || _isDead || _currentHealth >= _maxHealth)
        {
            _recoveryRemainder = 0f;
            return;
        }

        _recoveryRemainder += Mathf.Max(0f, deltaTime) * 100f / Mathf.Max(0.05f, _secondsPerSlot);
        int amount = Mathf.FloorToInt(_recoveryRemainder);
        if (amount <= 0) return;
        _recoveryRemainder -= amount;
        Heal(amount);
        if (_currentHealth >= _maxHealth) _recoveryRemainder = 0f;
    }

    public void TakeDamage(int damage)
    {
        ApplyHealthLoss(damage, true);
    }

    // 공격 코스트 같은 자해성 소모. OnHit(넉백·피격 이펙트) 없이 체력만 감소한다.
    public void SpendHealth(int amount)
    {
        ApplyHealthLoss(amount, false);
    }

    private void ApplyHealthLoss(int amount, bool triggerHit)
    {
        if (_isDead || amount <= 0) return;

        _recoveryRemainder = 0f;

        _currentHealth -= amount;

        if (_currentHealth <= 0)
        {
            _isDead = true;
            _currentHealth = 0;

            OnDie?.Invoke();
            return;
        }

        if (triggerHit) OnHit?.Invoke();
    }

    public void Heal(int healAmount)
    {
        if (_isDead) return;

        _currentHealth += healAmount;

        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    public void ReviveFull()
    {
        _isDead = false;
        _currentHealth = _maxHealth;
        _recoveryRemainder = 0f;
    }
}