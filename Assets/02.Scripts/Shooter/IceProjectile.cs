using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class IceProjectile : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float _lifeTime = 6f;
    [Tooltip("검사할 지형 레이어. 해당 레이어 중 Ground/Platform 태그만 충돌 대상으로 처리합니다.")]
    [SerializeField] private LayerMask _terrainLayers = ~0;

    private readonly List<RaycastHit2D> _castHits = new();
    private Rigidbody2D _body;
    private CircleCollider2D _collider;
    private ContactFilter2D _terrainFilter;
    private Vector2 _velocity;
    private float _age;
    private bool _destroying;

    public Vector2 Velocity => _velocity;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
        _body.bodyType = RigidbodyType2D.Kinematic;
        _body.gravityScale = 0f;
        _body.constraints = RigidbodyConstraints2D.FreezeRotation;
        _collider.isTrigger = true;
        _terrainFilter = new ContactFilter2D { useTriggers = true };
        _terrainFilter.SetLayerMask(_terrainLayers);
    }

    public void Launch(Vector2 direction, float speed)
    {
        _age = 0f;
        SetVelocity(direction.normalized * Mathf.Max(0f, speed));
    }

    // 이후 차지 점프에서 정지/재개/반대 방향 발사에 사용할 수 있습니다. 수명은 초기화하지 않습니다.
    public void SetVelocity(Vector2 velocity) => _velocity = velocity;

    private void FixedUpdate()
    {
        if (_destroying) return;

        _age += Time.fixedDeltaTime;
        if (_age >= _lifeTime)
        {
            Despawn();
            return;
        }

        Vector2 step = _velocity * Time.fixedDeltaTime;
        float distance = step.magnitude;
        if (distance <= 0f) return;

        Vector2 direction = step / distance;
        _castHits.Clear();
        _collider.Cast(direction, _terrainFilter, _castHits, distance);
        foreach (var hit in _castHits)
        {
            if (!IsTerrain(hit.collider)) continue;
            Despawn();
            return;
        }

        _body.MovePosition(_body.position + step);
    }

    private void OnTriggerEnter2D(Collider2D other) => CheckTerrain(other);
    private void OnTriggerStay2D(Collider2D other) => CheckTerrain(other);

    private void CheckTerrain(Collider2D other)
    {
        if ((_terrainLayers.value & (1 << other.gameObject.layer)) != 0 && IsTerrain(other))
            Despawn();
    }

    private static bool IsTerrain(Collider2D other) =>
        other != null && (other.CompareTag("Ground") || other.CompareTag("Platform"));

    private void Despawn()
    {
        if (_destroying) return;
        _destroying = true;
        _collider.enabled = false;
        Destroy(gameObject);
    }
}
