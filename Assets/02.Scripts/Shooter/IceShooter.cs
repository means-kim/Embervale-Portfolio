using UnityEngine;

[DisallowMultipleComponent]
public class IceShooter : MonoBehaviour
{
    [SerializeField] private IceProjectile _projectilePrefab;
    [Tooltip("입구 위치를 지정합니다. X는 중심부터 입구까지의 거리로 사용하고, 발사 방향은 Visual의 +X와 Flip으로 계산합니다. FirePoint 회전은 필요 없습니다.")]
    [SerializeField] private Transform _firePoint;
    [Tooltip("비워두면 자식 SpriteRenderer를 찾습니다. 루트 회전·Scale 반전과 Visual Flip X/Y를 발사에 함께 적용합니다.")]
    [SerializeField] private SpriteRenderer _visual;
    [SerializeField, Min(0.05f)] private float _fireInterval = 2f;
    [SerializeField, Min(0f)] private float _initialDelay = 0.5f;
    [SerializeField, Min(0.01f)] private float _projectileSpeed = 4f;

    private float _timeUntilNextShot;

    private void OnEnable() => _timeUntilNextShot = _initialDelay;

    private void Start()
    {
        if (_projectilePrefab != null && _firePoint != null) return;
        Debug.LogError("IceShooter: Projectile Prefab과 Fire Point를 연결하세요.", this);
        enabled = false;
    }

    private void Update()
    {
        _timeUntilNextShot -= Time.deltaTime;
        if (_timeUntilNextShot > 0f) return;

        Fire();
        // 프레임이 지연되어도 밀린 투사체를 한꺼번에 발사하지 않습니다.
        _timeUntilNextShot = Mathf.Max(0.05f, _fireInterval);
    }

    public void Fire()
    {
        if (_projectilePrefab == null || !TryGetFirePose(out Vector3 position, out Vector2 direction)) return;

        IceProjectile projectile = Instantiate(_projectilePrefab, position, Quaternion.identity);
        projectile.Launch(direction, _projectileSpeed);
    }

    public bool TryGetFirePose(out Vector3 position, out Vector2 direction)
    {
        position = transform.position;
        direction = Vector2.zero;
        if (_firePoint == null) return false;
        SpriteRenderer visual = _visual != null ? _visual : GetComponentInChildren<SpriteRenderer>();
        Transform basis = visual != null ? visual.transform : transform;
        Vector3 center = visual != null && visual.sprite != null ? visual.sprite.bounds.center : Vector3.zero;
        Vector3 muzzle = basis.InverseTransformPoint(_firePoint.position) - center;
        // 원본 이미지의 입구는 +X입니다. 기존 왼쪽 입구 설정도 거리로 재사용합니다.
        muzzle.x = Mathf.Abs(muzzle.x);
        Vector3 localDirection = Vector3.right;
        if (visual != null && visual.flipX) { muzzle.x = -muzzle.x; localDirection.x = -1f; }
        if (visual != null && visual.flipY) muzzle.y = -muzzle.y;
        position = basis.TransformPoint(center + muzzle);
        // Transform.right와 달리 음수 Scale 및 부모 반전도 반영합니다.
        Vector3 worldDirection = basis.TransformVector(localDirection);
        direction = new Vector2(worldDirection.x, worldDirection.y).normalized;
        return direction.sqrMagnitude > 0.0001f;
    }

    private void OnDrawGizmosSelected()
    {
        if (!TryGetFirePose(out Vector3 position, out Vector2 direction)) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(position, 0.12f);
        Vector3 forward = direction;
        Vector3 side = new Vector3(-direction.y, direction.x, 0f);
        Vector3 tip = position + forward;
        Gizmos.DrawLine(position, tip);
        Gizmos.DrawLine(tip, tip - forward * 0.25f + side * 0.15f);
        Gizmos.DrawLine(tip, tip - forward * 0.25f - side * 0.15f);
    }
}
