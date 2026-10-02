using System.Collections.Generic;
using UnityEngine;

public class EliteMonsterEffectHitbox : MonoBehaviour
{
    [SerializeField] private EliteMonsterAttackDataSO _attacks;
    [SerializeField] private EliteMonsterAttackType _attackType = EliteMonsterAttackType.IceThorn;

    private readonly HashSet<Health> _hitTargets = new();
    private int _damage;

    private void OnEnable()
    {
        _hitTargets.Clear();
        _damage = 0;
        if (_attacks == null || _attacks.Attacks == null) return;

        foreach (EliteMonsterAttackData data in _attacks.Attacks)
        {
            if (data == null || data.Type != _attackType) continue;
            _damage = data.Damage;
            break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActiveAndEnabled || _damage <= 0) return;

        Health health = other.GetComponentInParent<Health>();
        if (health == null || health.IsDead) return;
        if (!other.CompareTag("Player") && !health.CompareTag("Player")) return;
        if (!_hitTargets.Add(health)) return;

        health.TakeDamage(_damage);
    }
}