using UnityEngine;

// 피해를 받는 콜라이더에만 추가한다. 보스의 공격 콜라이더와 구분한다.
public class DamageReceiver : MonoBehaviour
{
    [SerializeField] private Health _health;
    public Health Health => _health != null ? _health : (_health = GetComponentInParent<Health>());
    public bool TryTakeDamage(int damage, Health attacker)
    {
        var health = Health;
        if (!CompareTag("Boss") || health == null || health == attacker || health.IsDead || damage <= 0) return false;
        health.TakeDamage(damage);
        return true;
    }
}
