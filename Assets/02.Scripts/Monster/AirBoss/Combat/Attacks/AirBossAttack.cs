using UnityEngine;

public class AirBossAttack : MonoBehaviour
{
    [SerializeField] private AirBossAttackType _type;
    [SerializeField] private int _damage;

    private bool _isReserved = false;
    private bool _hasHit = false;

    public bool IsReserved => _isReserved;
    public bool HasHit => _hasHit;

    public AirBossAttackType Type => _type;

    private Quaternion _defaultLocalRotation;

    private void Awake()
    {
        _defaultLocalRotation = transform.localRotation;
    }

    public bool TryReserve()
    {
        if (_isReserved || gameObject.activeSelf)
        {
            return false;
        }

        _isReserved = true;
        return true;
    }

    public void Activate(Vector3 position)
    {
        if (!_isReserved || gameObject.activeSelf) return;

        transform.position = position;
        _hasHit = false;
        gameObject.SetActive(true);
    }

    public void Release()
    {
        gameObject.SetActive(false);
        _isReserved = false;
        _hasHit = false;
        transform.localRotation = _defaultLocalRotation;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;

        Health health = other.GetComponent<Health>();

        if (health == null) return;

        if (!other.CompareTag("Player")) return;

        if (health.IsDead) return;

        _hasHit = true;

        health.TakeDamage(_damage);
        // Debug.Log("공격 적중");
    }
}