using System.Collections.Generic;
using UnityEngine;

public class EliteMonsterAttackHitbox : MonoBehaviour
{
    [SerializeField] private EliteMonsterAttack _attack;
    private readonly HashSet<Health> _hitTargets = new();

    public void Initialize(EliteMonsterAttack attack)
    {
        _attack = attack;
        ResetHits();
    }

    public void ResetHits()
    {
        _hitTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_attack == null || _attack.Data == null || !_attack.IsAttacking) return;

        Health health = other.GetComponentInParent<Health>();
        if (health == null || health.IsDead) return;
        if (!other.CompareTag("Player") && !health.CompareTag("Player")) return;
        if (_attack.Data.Damage <= 0 || !_hitTargets.Add(health)) return;

        health.TakeDamage(_attack.Data.Damage);
    }
}
