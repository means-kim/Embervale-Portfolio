using UnityEngine;

public class SlimeBAttack : MonoBehaviour
{
    [SerializeField] private int _damage = 50;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damage);
            }
        }
    }
}