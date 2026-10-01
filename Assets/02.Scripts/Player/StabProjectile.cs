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

    public float MaxDistance => _maxDistance;

    private int _damage;
    private int _direction = 1;
    private Vector3 _startPos;
    private float _age;
    private bool _launched;

    private void Awake()
    {
        // 프리팹 설정 실수 방지. 여기서 강제해 두면 인스펙터 누락에도 동작한다.
        var collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.isTrigger = true;
        var rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody == null)
        {
            rigidbody = gameObject.AddComponent<Rigidbody2D>();
            rigidbody.isKinematic = true;
            rigidbody.gravityScale = 0f;
        }
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
            Destroy(gameObject);
            return;
        }
        if (other.CompareTag("Ground") || other.CompareTag("Wall") || other.CompareTag("Platform"))
            Destroy(gameObject);
    }
}
