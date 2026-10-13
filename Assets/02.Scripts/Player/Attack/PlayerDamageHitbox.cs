using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageHitbox : MonoBehaviour
{
    [SerializeField] private Collider2D[] _colliders = Array.Empty<Collider2D>();
    private readonly HashSet<Health> _hitTargets = new();
    private Health _owner;
    private int _damage;
    public bool IsActive { get; private set; }
    public event Action<Collider2D> OnDamageApplied;
    public bool IsConfigured => _colliders.Length > 0 && Array.TrueForAll(_colliders, c => c != null);
    private void Awake() => SetActive(false);
    private void OnDisable() => SetActive(false);
    public void Initialize(int damage, Transform owner)
    {
        _damage = damage;
        _owner = null;
        if (owner != null)
        {
            _owner = owner.GetComponentInParent<Health>();
        }
        _hitTargets.Clear();
    }
    public void SetActive(bool active)
    {
        IsActive = active;
        foreach (var hitCollider in _colliders)
            if (hitCollider != null) { hitCollider.isTrigger = true; hitCollider.enabled = active; }
    }
    public bool TryHit(Collider2D other)
    {
        if (!IsActive || other == null || !other.TryGetComponent<DamageReceiver>(out var receiver)) return false;
        var target = receiver.Health;
        if (target == null || !_hitTargets.Add(target)) return false;
        // 데미지(사망 시 Dummy가 Destroy 예약) 전에 타격점을 먼저 확정한다.
        // transform.position은 공격 인스턴스 원점(플레이어 발 근처)이라 그대로
        // ClosestPoint에 쓰면 Dummy 밑단 가장자리가 나온다. 실제 판정 콜라이더
        // bounds 중심을 기준으로 구하고, 그 점이 대상 안에 있으면 중심을 쓴다.
        Vector3 targetCenter = other.bounds.center;
        Vector3 queryPoint = transform.position;
        if (TryGetBounds(out Bounds attackerBounds))
        {
            queryPoint = attackerBounds.center;
        }
        Vector3 hitPoint = other.ClosestPoint(queryPoint);
        if (other.OverlapPoint(queryPoint))
        {
            hitPoint = targetCenter;
        }
        Vector3 ownerPos = transform.position;
        if (_owner != null)
        {
            ownerPos = _owner.transform.position;
        }
        Vector2 direction = (Vector2)targetCenter - (Vector2)ownerPos;
        if (!receiver.TryTakeDamage(_damage, _owner)) { _hitTargets.Remove(target); return false; }
        EffectManager.Instance?.PlayBossHit(hitPoint, direction);
        OnDamageApplied?.Invoke(other);
        return true;
    }
    private void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    private void OnTriggerStay2D(Collider2D other) => TryHit(other);
    public bool TryGetBounds(out Bounds bounds)
    {
        bounds = default;
        bool found = false;
        foreach (var hitCollider in _colliders)
        {
            if (hitCollider == null || !hitCollider.enabled) continue;
            if (!found) { bounds = hitCollider.bounds; found = true; }
            else
            {
                bounds.Encapsulate(hitCollider.bounds);
            }
        }
        return found;
    }
}
