using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격 VFX 프리팹 자체의 콜라이더를 공격 판정으로 쓴다.
/// SO 크기로 별도 박스를 만들지 않고 프리팹에 붙은 트리거를 그대로 사용한다.
/// 판정 수명이 다하면 콜라이더만 끄고, 오브젝트 파괴는 OneShotVFX에 맡긴다.
/// </summary>
public class PlayerVfxHitbox : MonoBehaviour
{
    private int _damage;
    private float _lifeTime;
    private float _age;
    private Collider2D[] _colliders = System.Array.Empty<Collider2D>();
    private readonly HashSet<Health> _hitTargets = new();
    public void Setup(int damage, float lifeTime)
    {
        _damage = damage;
        _lifeTime = Mathf.Max(lifeTime, 0.05f);
        _age = 0f;
        _hitTargets.Clear();
        _colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (var collider in _colliders)
        {
            if (collider != null) collider.isTrigger = true;
        }
    }
    private void Update()
    {
        _age += Time.deltaTime;
        if (_age < _lifeTime) return;
        foreach (var collider in _colliders)
        {
            if (collider != null) collider.enabled = false;
        }
        enabled = false;
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
