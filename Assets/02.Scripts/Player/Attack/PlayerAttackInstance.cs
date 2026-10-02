using UnityEngine;

[RequireComponent(typeof(PlayerDamageHitbox), typeof(Rigidbody2D))]
public class PlayerAttackInstance : MonoBehaviour
{
    [SerializeField, Min(0)] private float _hitDelay;
    [SerializeField, Min(0.05f)] private float _hitDuration = 0.22f;
    [Tooltip("플레이어 행동 종료/피격 후에도 이미 생성된 공격을 유지한다. 화염구에 사용.")]
    [SerializeField] private bool _independent;
    private PlayerDamageHitbox _hitbox;
    private float _age;
    private bool _initialized;
    private bool _ended;
    public int Direction { get; private set; } = 1;
    public Transform Owner { get; private set; }
    private void Awake()
    {
        _hitbox = GetComponent<PlayerDamageHitbox>();
        var body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0;
        _hitbox.SetActive(false);
    }
    public bool Initialize(int damage, int direction, Transform owner)
    {
        if (!_hitbox.IsConfigured)
        {
            Debug.LogError($"공격 판정 콜라이더를 명시적으로 연결하세요: {name}", this);
            return false;
        }
        Owner = owner;
        Direction = direction < 0 ? -1 : 1;
        var scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Direction;
        transform.localScale = scale;
        _age = 0;
        _initialized = true;
        _hitbox.Initialize(damage, owner);
        _hitbox.SetActive(_hitDelay <= 0);
        return true;
    }
    private void Update()
    {
        if (!_initialized || _ended) return;
        _age += Time.deltaTime;
        if (_age >= Mathf.Max(0, _hitDelay) + Mathf.Max(0.05f, _hitDuration)) StopDamage();
        else if (_age >= _hitDelay && !_hitbox.IsActive) _hitbox.SetActive(true);
    }
    public void EndPlayerAction()
    {
        if (!_independent) StopDamage();
    }
    public void StopDamage()
    {
        _ended = true;
        _hitbox.SetActive(false);
    }
    public bool TryGetHitBounds(out Bounds bounds) => _hitbox.TryGetBounds(out bounds);
}
