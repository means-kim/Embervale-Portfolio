using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMeleeHitbox : MonoBehaviour
{
    private int _damage;
    private float _lifeTime;
    private float _age;
    private readonly HashSet<Health> _hitTargets = new();
    public void Setup(int damage, Vector2 size, float lifeTime)
    {
        _damage = damage;
        _lifeTime = Mathf.Max(lifeTime, 0.05f);
        _age = 0f;
        _hitTargets.Clear();
        var collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = size;
        var rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody2D>();
        rigidbody.isKinematic = true;
        rigidbody.gravityScale = 0f;
    }
    private void Update()
    {
        _age += Time.deltaTime;
        if (_age >= _lifeTime) Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || !other.CompareTag("Enemy")) return;
        Health health = other.GetComponent<Health>();
        if (health == null) health = other.GetComponentInParent<Health>();
        if (health == null || health.IsDead) return;
        if (_damage <= 0 || !_hitTargets.Add(health)) return;
        health.TakeDamage(_damage);
        EffectManager.Instance?.PlayEnemyHit(other.transform.position);
    }
}
