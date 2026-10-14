using UnityEngine;

[RequireComponent(typeof(PlayerDamageHitbox))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAttackInstance : MonoBehaviour
{
    [SerializeField, Min(0)] private float _hitDelay;
    [SerializeField, Min(0.05f)] private float _hitDuration = 0.22f;
    [Tooltip("플레이어 행동 종료/피격 후에도 이미 생성된 공격을 유지한다. 화염구에 사용.")]
    [SerializeField] private bool _independent;
    [Tooltip("켜져 있으면 사라질 때까지 오너(플레이어)의 위치와 방향을 따라다닌다. 투사체(FireBall)에는 사용하지 않는다.")]
    [SerializeField] private bool _followOwner;
    //public bool FollowOwner { get => _followOwner; set => _followOwner = value; }
    private PlayerDamageHitbox _hitbox;
    private Rigidbody2D _body;
    private PlayerMove _ownerMove;
    private Vector3 _spawnOffset;
    private bool _isProjectile;
    private float _age;
    private float _playbackSpeed = 1f;
    private bool _initialized;
    private bool _ended;
    public int Direction { get; private set; } = 1;
    public Transform Owner { get; private set; }
    private void Awake()
    {
        _hitbox = GetComponent<PlayerDamageHitbox>();
        _body = GetComponent<Rigidbody2D>();
        _body.bodyType = RigidbodyType2D.Kinematic;
        _body.gravityScale = 0;
        _isProjectile = GetComponent<FireBallPrefab>() != null;
        _hitbox.SetActive(false);
    }
    public bool Initialize(int damage, int direction, Transform owner, float attackSpeed = 1f, bool isThirdComboHit = false, int healOnHit = 0)
    {
        if (!_hitbox.IsConfigured)
        {
            Debug.LogError($"공격 판정 콜라이더를 명시적으로 연결하세요: {name}", this);
            return false;
        }
        Owner = owner;
        Direction = 1;
        if (direction < 0)
        {
            Direction = -1;
        }
        var scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Direction;
        transform.localScale = scale;
        _spawnOffset = Vector3.zero;
        if (owner != null)
        {
            _spawnOffset = transform.position - owner.position;
        }
        _ownerMove = null;
        if (owner != null)
        {
            _ownerMove = owner.GetComponent<PlayerRig>()?.Move;
        }
        _age = 0;
        _playbackSpeed = Mathf.Max(0.01f, attackSpeed);
        if (_independent || _isProjectile)
        {
            _playbackSpeed = 1f;
        }
        foreach (var animator in GetComponentsInChildren<Animator>(true))
        {
            animator.speed *= _playbackSpeed;
        }
        _initialized = true;
        _hitbox.Initialize(damage, owner, isThirdComboHit, healOnHit);
        _hitbox.SetActive(_hitDelay <= 0);
        return true;
    }
    private void Update()
    {
        if (!_initialized || _ended)
        {
            return;
        }
        _age += Time.deltaTime * _playbackSpeed;
        if (_age >= Mathf.Max(0, _hitDelay) + Mathf.Max(0.05f, _hitDuration))
        {
            StopDamage();
        }
        else if (_age >= _hitDelay && !_hitbox.IsActive)
        {
            _hitbox.SetActive(true);
        }
    }
    public void EndPlayerAction()
    {
        if (!_independent)
        {
            StopDamage();
        }
    }
    private void LateUpdate()
    {
        if (!_followOwner || !_initialized || _ended || Owner == null || _isProjectile)
        {
            return;
        }
        int facing = Direction;
        if (_ownerMove != null && _ownerMove.FacingDirection < 0)
        {
            facing = -1;
        }
        else if (_ownerMove != null)
        {
            facing = 1;
        }
        if (facing != Direction)
        {
            Direction = facing;
            var followScale = transform.localScale;
            followScale.x = Mathf.Abs(followScale.x) * Direction;
            transform.localScale = followScale;
        }
        Vector3 offset = new Vector3(Mathf.Abs(_spawnOffset.x) * Direction, _spawnOffset.y, _spawnOffset.z);
        transform.position = Owner.position + offset;
    }
    public void StopDamage()
    {
        _ended = true;
        _hitbox.SetActive(false);
    }
    //public bool TryGetHitBounds(out Bounds bounds) => _hitbox.TryGetBounds(out bounds);
}
