using UnityEngine;
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerDash))]
[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerChargeJump))]
public class PlayerHit : MonoBehaviour
{
    [SerializeField] private Color _flashColor = new Color(1f, 0.25f, 0.25f, 1f);
    [SerializeField] private float _hitInvincibilityTime = 0.3f;
    [Header("피해량별 피격 이펙트")]
    [SerializeField, Min(1), Tooltip("이 피해량 이상이면 기본 피격 효과, 미만이면 Horizontal 피격 효과를 사용합니다.")]
    private int _heavyHitDamageThreshold = 80;
    [Header("피격 넉백")]
    [SerializeField, Min(0f)] private float _knockbackForce = 4f;
    [SerializeField] private float _knockbackUp = 2f;
    [SerializeField, Min(0f)] private float _knockbackLockTime = 0.15f;
    public bool IsHit => Time.time < _hitUntil;
    private Health _health;
    private PlayerController _controller;
    private PlayerDash _dash;
    private PlayerAttack _attack;
    private PlayerMove _move;
    private PlayerChargeJump _chargeJump;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer[] _renderers;
    private Color[] _originalColors;
    private float _hitUntil;
    private float _flashTimer;
    private bool _flashOn;
    private bool _isInvincible = false;
    private float _invincibilityTimer = 0f;
    private float _moveUnlockAt;
    private float _hitDuration = 0.35f;
    private float _flashInterval = 0.08f;
    private bool _knockbackLocking;
    private void Awake()
    {
        _health = GetComponent<Health>();
        _controller = GetComponent<PlayerController>();
        _dash = GetComponent<PlayerDash>();
        _attack = GetComponent<PlayerAttack>();
        _move = GetComponent<PlayerMove>();
        _chargeJump = GetComponent<PlayerChargeJump>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _renderers = GetComponentsInChildren<SpriteRenderer>(true);
        _originalColors = new Color[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalColors[i] = _renderers[i].color;
        }
    }
    private void OnEnable()
    {
        _health.OnDamaged += HandleHit;
        _health.OnDie += HandleDie;
    }
    private void OnDisable()
    {
        _health.OnDamaged -= HandleHit;
        _health.OnDie -= HandleDie;
        RestoreColors();
        ReleaseKnockbackLock();
    }

    private void ApplyKnockback()
    {
        int facing = _move.FacingDirection;
        float xPush = -facing * _knockbackForce;
        if (_knockbackLockTime > 0f)
        {
            _move.SetMovementLocked(this, true);
            _moveUnlockAt = Time.time + _knockbackLockTime;
            _knockbackLocking = true;
        }
        _rigidbody.linearVelocity = new Vector2(xPush, Mathf.Max(_rigidbody.linearVelocity.y, _knockbackUp));
    }

    public void ResetForRespawn()
    {
        _hitUntil = 0f;
        _flashTimer = 0f;
        _moveUnlockAt = 0f;
        _isInvincible = false;
        _invincibilityTimer = 0f;
        ReleaseKnockbackLock();
        RestoreColors();
        _move.SetMovementLocked(_health, false);
    }
    private void ReleaseKnockbackLock()
    {
        if (!_knockbackLocking)
        {
            return;
        }
        _knockbackLocking = false;
        _move.SetMovementLocked(this, false);
    }
    private void HandleHit(int damage)
    {
        if (_health.IsDead || _isInvincible)
        {
            return;
        }
        _chargeJump.CancelChargeJump();
        _attack.CancelAttack();
        _hitUntil = Time.time + _hitDuration;
        _flashTimer = 0f;
        _flashOn = false;
        _isInvincible = true;
        _invincibilityTimer = _hitInvincibilityTime;
        _dash.CancelDash();
        ApplyKnockback();
        bool horizontal = damage < Mathf.Max(1, _heavyHitDamageThreshold);
        EffectManager.Instance?.PlayPlayerHit(transform.position, horizontal);
    }
    private void HandleDie()
    {
        _chargeJump.CancelChargeJump();
        _attack.CancelAttack();
        _dash.CancelDash();
        _move.SetMovementLocked(_health, true);
        _hitUntil = 0f;
        RestoreColors();
        EffectManager.Instance?.PlayPlayerDeath(transform.position);
        _controller.ChangeState(PlayerState.Death);
    }
    private void Update()
    {
        if (_isInvincible)
        {
            _invincibilityTimer -= Time.deltaTime;
            if (_invincibilityTimer <= 0f)
            {
                _isInvincible = false;
            }
        }
        if (_knockbackLocking && Time.time >= _moveUnlockAt)
        {
            ReleaseKnockbackLock();
        }
        if (!IsHit)
        {
            if (_flashOn)
            {
                RestoreColors();
            }
            return;
        }
        _flashTimer += Time.deltaTime;
        if (_flashTimer < _flashInterval)
        {
            return;
        }
        _flashTimer = 0f;
        _flashOn = !_flashOn;
        ApplyFlash(_flashOn);
    }
    private void ApplyFlash(bool on)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] == null)
            {
                continue;
            }
            if (on)
            {
                _renderers[i].color = _flashColor;
            }
            else
            {
                _renderers[i].color = _originalColors[i];
            }
        }
    }
    private void RestoreColors()
    {
        _flashOn = false;
        ApplyFlash(false);
    }
}
