using System;
using UnityEngine;
public class Border : MonoBehaviour
{
    public event Action PlayerDeath;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) PlayerDeath?.Invoke();
    }
}
