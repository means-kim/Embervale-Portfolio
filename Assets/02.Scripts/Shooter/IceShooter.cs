using UnityEngine;

[DisallowMultipleComponent]
public class IceShooter : MonoBehaviour
{
    [SerializeField] private IceProjectile _projectilePrefab;
    [Tooltip("입구 앞에 배치합니다. 이 Transform의 로컬 +X(빨간 축)가 발사 방향입니다.")]
    [SerializeField] private Transform _firePoint;
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
        if (_projectilePrefab == null || _firePoint == null) return;

        IceProjectile projectile = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);
        projectile.Launch(_firePoint.right, _projectileSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        if (_firePoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_firePoint.position, 0.12f);
        Vector3 tip = _firePoint.position + _firePoint.right;
        Gizmos.DrawLine(_firePoint.position, tip);
        Gizmos.DrawLine(tip, tip - _firePoint.right * 0.25f + _firePoint.up * 0.15f);
        Gizmos.DrawLine(tip, tip - _firePoint.right * 0.25f - _firePoint.up * 0.15f);
    }
}
