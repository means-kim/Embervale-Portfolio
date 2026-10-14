using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private bool _autoRecover;
    [SerializeField, Min(0.05f)] private float _secondsPerSlot = 1.2f;
    [SerializeField, Min(1f), Tooltip("자동 회복 숫자 표시 주기. 이 시간 동안 누적된 회복량을 한 번에 알린다.")]
    private float _regenFlushInterval = 5f;
    private float _recoveryRemainder;
    private int _regenPending;
    private float _regenTimer;
    private int _currentHealth;
    private bool _isDead = false;
    private int _maxHealthBonus;
    private bool _isPlayer;
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
    // 피격의 실제 체력 감소량(사망 일격 포함). 체력 소모(SpendHealth)는 제외한다.
    public event Action<int> OnDamaged;
    // 공격 코스트 같은 자해성 소모의 실제 감소량(사망 제외). 넉백·피격 이펙트 없이 숫자 표시용으로만 쓴다.
    public event Action<int> OnSpent;
    // 즉시 회복의 실제 회복량(초과분 제외). 자동 회복은 제외하고 5초 합산으로 별도 알린다.
    public event Action<int> OnHealed;
    // 자동 회복을 _regenFlushInterval 동안 합산한 양. 합산량이 0이면 발송하지 않는다.
    public event Action<int> OnRegenFlushed;

    private void Awake()
    {
        _isPlayer = GetComponent<Player>() != null;
        _currentHealth = MaxHealth;
    }

    private void Update()
    {
        AdvanceRecovery(Time.deltaTime);
    }

    public void AdvanceRecovery(float deltaTime)
    {
        if (_isDead)
        {
            _regenPending = 0;
            _regenTimer = 0f;
            return;
        }

        if (_currentHealth >= MaxHealth)
        {
            _recoveryRemainder = 0f;
            FlushRegen(true);
            return;
        }

        if (!_autoRecover)
        {
            _recoveryRemainder = 0f;
            return;
        }

        float recoveryMultiplier = _isPlayer ? GameDifficulty.RecoveryMultiplier : 1f;
        _recoveryRemainder += Mathf.Max(0f, deltaTime) * recoveryMultiplier * 100f / Mathf.Max(0.05f, _secondsPerSlot);
        int amount = Mathf.FloorToInt(_recoveryRemainder);

        // 주기가 벽시계 기준이 되도록 틱 유무와 관계없이 누적한다.
        _regenTimer += Mathf.Max(0f, deltaTime);
        if (_regenTimer >= _regenFlushInterval && _regenPending > 0)
        {
            FlushRegen(false);
        }

        if (amount <= 0) return;
        _recoveryRemainder -= amount;
        Heal(amount, true);
        if (_currentHealth >= MaxHealth)
        {
            _recoveryRemainder = 0f;
            FlushRegen(true);
            return;
        }
    }

    private void FlushRegen(bool force)
    {
        if (_regenPending <= 0)
        {
            _regenTimer = 0f;
            return;
        }

        if (!force && _regenTimer < _regenFlushInterval)
        {
            return;
        }

        int sum = _regenPending;
        _regenPending = 0;
        _regenTimer = 0f;
        OnRegenFlushed?.Invoke(sum);
    }

    public void TakeDamage(int damage)
    {
        if (_isPlayer && damage > 0)
        {
            damage = Mathf.CeilToInt(damage * GameDifficulty.DamageTakenMultiplier);
        }
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
            _regenPending = 0;
            _regenTimer = 0f;

            // 사망 일격도 숫자 표시 대상이라 발송한다. 구독자(PlayerHit·HeavyDamageFeedback)는 IsDead를 보고 연출을 건너뛴다.
            if (triggerHit)
            {
                OnDamaged?.Invoke(amount);
            }
            OnDie?.Invoke();
            return;
        }

        if (triggerHit)
        {
            OnHit?.Invoke();
            OnDamaged?.Invoke(amount);
        }
        else
        {
            OnSpent?.Invoke(amount);
        }
    }

    public void Heal(int healAmount, bool isRegen = false)
    {
        if (_isDead) return;

        int before = _currentHealth;
        _currentHealth += healAmount;

        if (_currentHealth > MaxHealth)
        {
            _currentHealth = MaxHealth;
        }

        int actual = _currentHealth - before;
        if (actual <= 0) return;

        if (isRegen)
        {
            _regenPending += actual;
            return;
        }

        OnHealed?.Invoke(actual);
    }

    public void ReviveFull()
    {
        _isDead = false;
        _currentHealth = MaxHealth;
        _recoveryRemainder = 0f;
        _regenPending = 0;
        _regenTimer = 0f;
    }
}
