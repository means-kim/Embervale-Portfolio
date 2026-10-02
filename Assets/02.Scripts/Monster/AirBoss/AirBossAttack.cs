using UnityEngine;

public class AirBossAttack : MonoBehaviour
{
    [SerializeField] private AirBossAttackType _type;
    [SerializeField] private int _damage;

    public AirBossAttackType Type => _type;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();

            health.TakeDamage(_damage);
        }
    }
}