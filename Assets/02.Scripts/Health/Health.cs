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
    private int _maxHealthBonus;
    public int MaxHealth => _maxHealth + _maxHealthBonus;

    // Keep the serialized base value intact so reloading never compounds bonuses.
    public void SetMaxHealthBonus(int bonus)
    {
        int previousMax = MaxHealth;
        _maxHealthBonus = Math.Max(0, bonus);
        if (!_isDead)
            _currentHealth = Math.Clamp(_currentHealth + MaxHealth - previousMax, 0, Math.Max(0, MaxHealth));
    }
    public int CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = value;
    }
    public bool IsDead => _isDead;
    public event Action OnDie;
    public event Action OnHit;
    // 생존한 피격의 실제 체력 감소량. 체력 소모(SpendHealth)는 제외한다.
    public event Action<int> OnDamaged;

    private void Awake()
    {
        _currentHealth = MaxHealth;
    }

    private void Update()
    {
        AdvanceRecovery(Time.deltaTime);
    }

    public void AdvanceRecovery(float deltaTime)
    {
        if (!_autoRecover || _isDead || _currentHealth >= MaxHealth)
        {
            _recoveryRemainder = 0f;
            return;
        }

        _recoveryRemainder += Mathf.Max(0f, deltaTime) * 100f / Mathf.Max(0.05f, _secondsPerSlot);
        int amount = Mathf.FloorToInt(_recoveryRemainder);
        if (amount <= 0) return;
        _recoveryRemainder -= amount;
        Heal(amount);
        if (_currentHealth >= MaxHealth) _recoveryRemainder = 0f;
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

        if (triggerHit)
        {
            OnHit?.Invoke();
            OnDamaged?.Invoke(amount);
        }
    }

    public void Heal(int healAmount)
    {
        if (_isDead) return;

        _currentHealth += healAmount;

        if (_currentHealth > MaxHealth)
        {
            _currentHealth = MaxHealth;
        }
    }

    public void ReviveFull()
    {
        _isDead = false;
        _currentHealth = MaxHealth;
        _recoveryRemainder = 0f;
    }
}
