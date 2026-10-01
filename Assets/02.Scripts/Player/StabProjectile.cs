using System.Collections;
using UnityEngine;

/// <summary>
/// Stab 전용 투사체. 발사 방향으로 직진하며 Enemy에 닿으면 데미지를 주고 소멸한다.
/// 벽/바닥에 닿거나 사거리·수명을 다해도 소멸한다. PlayerAttack이 발사한다.
/// 프리팹 구성: SpriteRenderer + Collider2D(Trigger) + Rigidbody2D(Kinematic, Gravity 0).
/// </summary>
public class StabProjectile : MonoBehaviour
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
        // 프리팹 설정 실수 방지. 여기서 강제해 두면 인스펙터 누락에도 동작한다.
        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.isTrigger = true;
        var rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody == null)
            rigidbody = gameObject.AddComponent<Rigidbody2D>();
        // 이동은 transform으로 직접 하므로 물리 영향을 끈다. 프리팹 설정이 바뀌어도 동작 보장.
        rigidbody.isKinematic = true;
        rigidbody.gravityScale = 0f;
    }

    /// <summary>발사 초기화. Instantiate 직후 1회 호출한다.</summary>
    public void Launch(int damage, int direction)
    {
        _damage = damage;
        _direction = direction >= 0 ? 1 : -1;
        _startPos = transform.position;
        _age = 0f;
        _launched = true;
        // 왼쪽 발사면 시각만 뒤집는다. 이동은 _direction 기준이라 그대로 간다.
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
        if (_age >= _lifeTime || Vector3.Distance(_startPos, transform.position) >= _maxDistance)
            BeginDestroy(instant: false);
    }

    /// <summary>소멸 처리. instant면 즉시 파괴, 아니면 페이드 설정에 따라 사라진다.</summary>
    private void BeginDestroy(bool instant)
    {
        if (_destroying) return;
        _destroying = true;
        if (!instant && _fadeOut && _fadeOutTime > 0f)
        {
            _launched = false; // 그 자리에서 멈추고 투명해진다
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
        // 근접 공격(DoHit)과 같은 기준. Enemy 태그에만 데미지를 준다.
        if (other.CompareTag("Enemy"))
        {
            var health = other.GetComponent<Health>();
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
