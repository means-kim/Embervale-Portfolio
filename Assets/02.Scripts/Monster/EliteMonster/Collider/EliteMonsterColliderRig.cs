using UnityEngine;

public sealed class EliteMonsterColliderRig : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _facingRoot;
    [SerializeField] private Health _health;
    [SerializeField] private BoxCollider2D _hurtbox;
    [SerializeField] private BoxCollider2D _slamHitbox;
    [SerializeField] private BoxCollider2D _swingHitbox;
    [SerializeField] private BoxCollider2D _chargeHitbox;

    public BoxCollider2D Hurtbox => _hurtbox;
    public BoxCollider2D SlamHitbox => _slamHitbox;
    public BoxCollider2D SwingHitbox => _swingHitbox;
    public BoxCollider2D ChargeHitbox => _chargeHitbox;

    private void LateUpdate()
    {
        SyncFacing();
        if (_health != null && _health.IsDead)
            DisableAll();
    }

    public void SyncFacing()
    {
        if (_spriteRenderer == null || _facingRoot == null) return;

        _facingRoot.localScale = new Vector3(
            _spriteRenderer.flipX ? -1f : 1f,
            _spriteRenderer.flipY ? -1f : 1f, 1f);
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
            DisableAll();
    }

    private void DisableAll()
    {
        if (_hurtbox != null) _hurtbox.enabled = false;
        if (_slamHitbox != null) _slamHitbox.enabled = false;
        if (_swingHitbox != null) _swingHitbox.enabled = false;
        if (_chargeHitbox != null) _chargeHitbox.enabled = false;
    }
}