using UnityEngine;

public class SlimeBDeath : MonoBehaviour
{
    private Health _health;
    private Rigidbody2D _rigidbody;
    private PolygonCollider2D _collider;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<PolygonCollider2D>();
    }

    public void BeginDeath()
    {
        if (!_health.IsDead) return;

        _collider.enabled = false;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0;
        _rigidbody.simulated = false;
    }
}