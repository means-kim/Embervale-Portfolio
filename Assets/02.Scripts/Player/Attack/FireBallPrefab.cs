using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class FireBallPrefab : MonoBehaviour, IPlayerProjectile
{
    [SerializeField, Min(0f)] private float _speed = 12f;
    [SerializeField, Min(0f)] private float _maxDistance = 8f;
    [SerializeField, Min(0f)] private float _lifeTime = 2f;
    [Tooltip("발사 직후 이 시간 동안은 지형(Ground/Wall/Platform) 접촉을 무시한다. 스폰 위치 겹침 오폭 방지. 적은 즉시 맞는다.")]
    [SerializeField, Min(0f)] private float _terrainGrace = 0.15f;
    [Header("소멸 페이드")]
    [Tooltip("켜면 수명/사거리 만료나 벽 충돌 시 스프라이트가 투명해지며 사라진다. 적 명중은 즉시 소멸한다.")]
    [SerializeField] private bool _fadeOut = true;
    [Tooltip("페이드 진행 시간. 0이면 즉시 소멸한다.")]
    [SerializeField, Min(0f)] private float _fadeOutTime = 0.25f;
    public float MaxDistance => _maxDistance;
    private int _damage;
    private int _direction = 1;
    private Vector3 _startPos;
    private float _age;
    private bool _launched;
    private bool _destroying;
    private void Awake()
    {
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.isTrigger = true;
        var rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.isKinematic = true;
        rigidbody.gravityScale = 0f;
    }
    public void Launch(int damage, int direction)
    {
        _damage = damage;
        _direction = direction >= 0 ? 1 : -1;
        _startPos = transform.position;
        _age = 0f;
        _launched = true;
        if (_direction < 0)
        {
            var scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
    private void Update()
    {
        if (!_launched) return;
        transform.position += new Vector3(_direction * _speed * Time.deltaTime, 0f, 0f);
        _age += Time.deltaTime;
        if (_age >= _lifeTime || Vector3.Distance(_startPos, transform.position) >= _maxDistance) BeginDestroy(instant: false);
    }
    private void BeginDestroy(bool instant)
    {
        if (_destroying) return;
        _destroying = true;
        if (!instant && _fadeOut && _fadeOutTime > 0f)
        {
            _launched = false;
            var collider = GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;
            StartCoroutine(FadeAndDestroy());
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator FadeAndDestroy()
    {
        var sprite = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        Color start = sprite != null ? sprite.color : Color.white;
        float t = 0f;
        while (t < _fadeOutTime)
        {
            t += Time.deltaTime;
            if (sprite != null)
            {
                Color c = start;
                c.a *= Mathf.Clamp01(1f - t / _fadeOutTime);
                sprite.color = c;
            }
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_launched || other == null) return;
        if (other.CompareTag("Enemy"))
        {
            var health = other.GetComponent<Health>();
            if (health == null) health = other.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(_damage);
                EffectManager.Instance?.PlayEnemyHit(other.transform.position);
            }
            BeginDestroy(instant: true);
            return;
        }
        if (other.CompareTag("Ground") || other.CompareTag("Wall") || other.CompareTag("Platform"))
        {
            if (_age < _terrainGrace) return;
            BeginDestroy(instant: false);
        }
    }
}
