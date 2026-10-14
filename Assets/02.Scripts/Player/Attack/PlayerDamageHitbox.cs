using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageHitbox : MonoBehaviour
{
    [SerializeField] private Collider2D[] _colliders = Array.Empty<Collider2D>();
    private readonly HashSet<Health> _hitTargets = new();
    private Health _owner;
    private int _damage;
    private int _healOnHit;
    private bool _isThirdComboHit;
    public bool IsThirdComboHit => _isThirdComboHit;
    public bool IsActive { get; private set; }
    public event Action<Collider2D> OnDamageApplied;
    public bool IsConfigured => _colliders.Length > 0 && Array.TrueForAll(_colliders, c => c != null);
    private void Awake() => SetActive(false);
    private void OnDisable() => SetActive(false);
    public void Initialize(int damage, Transform owner, bool isThirdComboHit = false, int healOnHit = 0)
    {
        _damage = damage;
        _healOnHit = Mathf.Max(0, healOnHit);
        _isThirdComboHit = isThirdComboHit;
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
        {
            if (hitCollider != null) { hitCollider.isTrigger = true; hitCollider.enabled = active; }
        }
    }
    public bool TryHit(Collider2D other)
    {
        if (!IsActive || other == null || !other.TryGetComponent<DamageReceiver>(out var receiver))
        {
            return false;
        }
        var target = receiver.Health;
        if (target == null || !_hitTargets.Add(target))
        {
            return false;
        }
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
        if (!receiver.TryTakeDamage(_damage, _owner))
        {
            _hitTargets.Remove(target);
            return false;
        }
        if (_healOnHit > 0)
        {
            _owner?.Heal(_healOnHit);
        }
        EffectManager.Instance?.PlayBossHit(hitPoint, direction, _damage, target.transform, _isThirdComboHit);
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
            if (hitCollider == null || !hitCollider.enabled)
            {
                continue;
            }
            if (!found) { bounds = hitCollider.bounds; found = true; }
            else
            {
                bounds.Encapsulate(hitCollider.bounds);
            }
        }
        return found;
    }
}
