using UnityEngine;

public class EliteMonsterEffectHitbox : MonoBehaviour
{
    [SerializeField] private EliteMonsterAttackDataSO _attacks;

    private int _damage;

    private void Awake()
    {
        foreach (EliteMonsterAttackData data in _attacks.Attacks)
        {
            if (data.Type != EliteMonsterAttackType.IceThorn) continue;

            _damage = data.Damage;
            break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(_damage);
        }
    }
}