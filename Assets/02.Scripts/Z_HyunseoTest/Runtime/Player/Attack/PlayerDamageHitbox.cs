using System;
using System.Collections.Generic;
using UnityEngine;

namespace HyunseoTest
{
    public class PlayerDamageHitbox : MonoBehaviour
    {
        [SerializeField] private Collider2D[] _colliders = Array.Empty<Collider2D>();
        private readonly HashSet<global::Health> _hitTargets = new();
        private global::Health _owner;
        private AudioClip _impactClip;
        private float _impactVolume;
        private int _damage;
        public bool IsActive { get; private set; }
        public event Action<Collider2D> OnDamageApplied;
        public bool IsConfigured => _colliders.Length > 0 && Array.TrueForAll(_colliders, c => c != null);
        private void Awake() => SetActive(false);
        private void OnDisable() => SetActive(false);
        public void Initialize(int damage, Transform owner)
        {
            _damage = damage;
            _owner = owner != null ? owner.GetComponentInParent<global::Health>() : null;
            _hitTargets.Clear();
            var audio = GetComponent<AudioSource>();
            if (audio != null) { _impactClip = audio.clip; _impactVolume = audio.volume; }
        }
        public void SetActive(bool active)
        {
            IsActive = active;
            foreach (var collider in _colliders)
                if (collider != null) { collider.isTrigger = true; collider.enabled = active; }
        }
        public bool TryHit(Collider2D other)
        {
            if (!IsActive || other == null || !other.TryGetComponent<global::DamageReceiver>(out var receiver)) return false;
            var target = receiver.Health;
            if (target == null || !_hitTargets.Add(target)) return false;
            // 데미지(사망 시 Dummy가 Destroy 예약) 전에 타격점을 먼저 확정한다.
            // transform.position은 공격 인스턴스 원점(플레이어 발 근처)이라 그대로
            // ClosestPoint에 쓰면 Dummy 밑단 가장자리가 나온다. 실제 판정 콜라이더
            // bounds 중심을 기준으로 구하고, 그 점이 대상 안에 있으면 중심을 쓴다.
            Vector3 targetCenter = other.bounds.center;
            Vector3 queryPoint = TryGetBounds(out Bounds attackerBounds)
                ? (Vector3)attackerBounds.center
                : transform.position;
            Vector3 hitPoint = other.OverlapPoint(queryPoint) ? targetCenter : (Vector3)other.ClosestPoint(queryPoint);
            Vector3 ownerPos = _owner != null ? _owner.transform.position : transform.position;
            Vector2 direction = (Vector2)targetCenter - (Vector2)ownerPos;
            if (!receiver.TryTakeDamage(_damage, _owner)) { _hitTargets.Remove(target); return false; }
            // This point is reached only after damage was accepted, once per target.
            EffectManager.Instance?.PlayBossHit(hitPoint, direction);
            if (_impactClip != null)
            {
                var sound = new GameObject("PlayerImpactSound");
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(sound, gameObject.scene);
                var source = sound.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 0f;
                source.PlayOneShot(_impactClip, _impactVolume);
                Destroy(sound, _impactClip.length + 0.1f);
            }
            OnDamageApplied?.Invoke(other);
            return true;
        }
        private void OnTriggerEnter2D(Collider2D other) => TryHit(other);
        private void OnTriggerStay2D(Collider2D other) => TryHit(other);
        public bool TryGetBounds(out Bounds bounds)
        {
            bounds = default;
            bool found = false;
            foreach (var collider in _colliders)
            {
                if (collider == null || !collider.enabled) continue;
                if (!found) { bounds = collider.bounds; found = true; }
                else
                {
                    bounds.Encapsulate(collider.bounds);
                }
            }
            return found;
        }
    }

}
