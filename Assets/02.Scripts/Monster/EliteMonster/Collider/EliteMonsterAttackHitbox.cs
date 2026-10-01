using UnityEngine;

public class EliteMonsterAttackHitbox : MonoBehaviour
{
    [SerializeField] private EliteMonsterAttack _attack;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_attack == null || _attack.Data == null) return;

        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(_attack.Data.Damage);
        }
    }
}