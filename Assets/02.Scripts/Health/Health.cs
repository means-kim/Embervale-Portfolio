using System;
using UnityEngine;
public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    private int _currentHealth;
    private bool _isDead = false;
    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public bool IsDead => _isDead;
    public event Action OnDie;
    public event Action OnHit;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (_isDead || damage <= 0) return;

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _isDead = true;
            _currentHealth = 0;

            OnDie?.Invoke();
            return;
        }

        OnHit?.Invoke();
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
}