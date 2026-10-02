using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAttackInstance), typeof(Collider2D))]
public class FireBallPrefab : MonoBehaviour
{
    [SerializeField, Min(0)] private float _speed = 12;
    [SerializeField, Min(0)] private float _maxDistance = 8;
    [SerializeField, Min(0)] private float _lifeTime = 2;
    [SerializeField] private bool _fadeOut = true;
    [SerializeField, Min(0)] private float _fadeOutTime = 0.25f;
    private readonly HashSet<Collider2D> _spawnOverlaps = new();
    private readonly List<Collider2D> _overlaps = new();
    private readonly List<RaycastHit2D> _castHits = new();
    private PlayerAttackInstance _attack;
    private PlayerDamageHitbox _hitbox;
    private Collider2D _collider;
    private Rigidbody2D _body;
    private float _age;
    private float _distance;
    private bool _destroying;
    private ContactFilter2D _filter;
    public float EffectiveRange => Mathf.Min(_maxDistance, _speed * _lifeTime);
    private void Awake()
    {
        _attack = GetComponent<PlayerAttackInstance>();
        _hitbox = GetComponent<PlayerDamageHitbox>();
        _body = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _filter = new ContactFilter2D { useTriggers = true };
        _filter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
    }
    private void OnEnable() => _hitbox.OnDamageApplied += HandleDamage;
    private void OnDisable() => _hitbox.OnDamageApplied -= HandleDamage;
    private void Start()
    {
        Physics2D.SyncTransforms();
        _collider.Overlap(_filter, _overlaps);
        foreach (var other in _overlaps)
            if (IsTerrain(other)) _spawnOverlaps.Add(other);
    }
    private void FixedUpdate()
    {
        if (_destroying) return;
        _age += Time.fixedDeltaTime;
        if (_age >= _lifeTime || _distance >= _maxDistance) { BeginDestroy(false); return; }
        _spawnOverlaps.RemoveWhere(c => c == null || !c.enabled || !_collider.Distance(c).isOverlapped);
        Vector2 direction = Vector2.right * _attack.Direction;
        float step = Mathf.Min(_speed * Time.fixedDeltaTime, Mathf.Max(0, _maxDistance - _distance));
        _castHits.Clear();
        _collider.Cast(direction, _filter, _castHits, step);
        _castHits.Sort((a, b) => a.distance.CompareTo(b.distance));
        foreach (var hit in _castHits)
        {
            if (hit.collider == null || (_attack.Owner != null && hit.collider.transform.IsChildOf(_attack.Owner))) continue;
            if (IsTerrain(hit.collider) && !_spawnOverlaps.Contains(hit.collider))
            {
                _body.position += direction * Mathf.Max(0, hit.distance);
                BeginDestroy(false);
                return;
            }
            if (_hitbox.TryHit(hit.collider)) return;
        }
        _body.MovePosition(_body.position + direction * step);
        _distance += step;
    }
    private static bool IsTerrain(Collider2D other) => other != null &&
        (other.CompareTag("Ground") || other.CompareTag("Platform"));
    private void OnTriggerEnter2D(Collider2D other) => CheckTerrain(other);
    private void OnTriggerStay2D(Collider2D other) => CheckTerrain(other);
    private void CheckTerrain(Collider2D other)
    {
        if (!_destroying && IsTerrain(other) && !_spawnOverlaps.Contains(other)) BeginDestroy(false);
    }
    private void HandleDamage(Collider2D _) => BeginDestroy(true);
    private void BeginDestroy(bool instant)
    {
        if (_destroying) return;
        _destroying = true;
        _attack.StopDamage();
        if (instant || !_fadeOut || _fadeOutTime <= 0) Destroy(gameObject);
        else StartCoroutine(FadeAndDestroy());
    }
    private IEnumerator FadeAndDestroy()
    {
        var sprites = GetComponentsInChildren<SpriteRenderer>();
        var colors = Array.ConvertAll(sprites, s => s.color);
        var animator = GetComponent<Animator>();
        if (animator != null) animator.enabled = false;
        float t = 0;
        while (t < _fadeOutTime)
        {
            t += Time.deltaTime;
            for (int i = 0; i < sprites.Length; i++)
            {
                var color = colors[i];
                color.a *= Mathf.Clamp01(1 - t / _fadeOutTime);
                sprites[i].color = color;
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
