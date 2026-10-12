using System;
using UnityEngine;

public class Border : MonoBehaviour
{
    public event Action PlayerDeath;
    private int _returnDamage = 50;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerDeath?.Invoke();
        }

        Health health = collision.transform.GetComponent<Health>();
        if (health == null || health.IsDead)
        {
            return;
        }

        health.TakeDamage(_returnDamage);
        if (health.IsDead)
        {
            return;
        }
    }
}