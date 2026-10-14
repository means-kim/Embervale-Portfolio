using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
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
    [Header("피격·회복 숫자")]
    [Tooltip("몸통 콜라이더 중심 기준 숫자 스폰 랜덤 반경.")]
    [SerializeField] private Vector2 _textSpawnRadius = new Vector2(0.5f, 0.6f);
    [SerializeField] private Vector3 _textSpawnOffset = Vector3.zero;
    public bool IsHit => Time.time < _hitUntil;
    private Health _health;
    private PlayerController _controller;
    private PlayerDash _dash;
    private PlayerAttack _attack;
    private PlayerMove _move;
    private PlayerChargeJump _chargeJump;
    private PlayerGlide _glide;
    private Rigidbody2D _rigidbody;
    private Collider2D _bodyCollider;
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
    private bool _deathHandled;
    private bool _deathPhysicsStopped;
    private bool _simulationBeforeDeath;
    private void Awake()
    {
        PlayerRig rig = PlayerRig.Resolve(gameObject);
        _health = rig.Health;
        _controller = rig.Controller;
        _dash = rig.Dash;
        _attack = rig.Attack;
        _move = rig.Move;
        _chargeJump = rig.ChargeJump;
        _glide = rig.Glide;
        _rigidbody = rig.Body;
        _bodyCollider = GetComponent<Collider2D>();
        _renderers = GetComponentsInChildren<SpriteRenderer>(true);
        _originalColors = new Color[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalColors[i] = _renderers[i].color;
        }
        // 연출이 이 컴포넌트를 꺼도 사망 처리는 유지한다.
        _health.OnDie += HandleDie;
    }
    private void OnEnable()
    {
        _health.OnDamaged += HandleHit;
        _health.OnSpent += HandleSpent;
        _health.OnHealed += HandleHealed;
        _health.OnRegenFlushed += HandleRegenFlushed;
        if (_health.IsDead)
        {
            HandleDie();
        }
    }
    private void OnDisable()
    {
        _health.OnDamaged -= HandleHit;
        _health.OnSpent -= HandleSpent;
        _health.OnHealed -= HandleHealed;
        _health.OnRegenFlushed -= HandleRegenFlushed;
        RestoreColors();
        ReleaseKnockbackLock();
    }
    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.OnDie -= HandleDie;
        }
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
        if (_deathPhysicsStopped)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
            _rigidbody.simulated = _simulationBeforeDeath;
            _deathPhysicsStopped = false;
        }
        _deathHandled = false;
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
        // 숫자는 무적·사망 여부와 관계없이 무조건 표시한다.
        EffectManager.Instance?.PlayPlayerDamageNumber(RandomTorsoPosition(), damage);
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
    // 차지 점프·스킬 등 체력 소모. 넉백·피격 이펙트 없이 숫자만 표시한다.
    private void HandleSpent(int amount)
    {
        if (_health.IsDead)
        {
            return;
        }
        EffectManager.Instance?.PlayPlayerDamageNumber(RandomTorsoPosition(), amount);
    }
    private void HandleHealed(int amount)
    {
        if (_health.IsDead)
        {
            return;
        }
        EffectManager.Instance?.PlayPlayerHealNumber(RandomTorsoPosition(), amount);
    }
    private void HandleRegenFlushed(int sum)
    {
        if (_health.IsDead)
        {
            return;
        }
        EffectManager.Instance?.PlayPlayerHealNumber(RandomTorsoPosition(), sum);
    }
    private Vector3 RandomTorsoPosition()
    {
        Vector3 center = _bodyCollider != null
            ? (Vector3)_bodyCollider.bounds.center
            : transform.position;
        center += _textSpawnOffset;
        center += new Vector3(
            Random.Range(-Mathf.Max(0f, _textSpawnRadius.x), Mathf.Max(0f, _textSpawnRadius.x)),
            Random.Range(-Mathf.Max(0f, _textSpawnRadius.y), Mathf.Max(0f, _textSpawnRadius.y)),
            0f);
        return center;
    }
    private void HandleDie()
    {
        if (_deathHandled)
        {
            return;
        }
        _deathHandled = true;
        _chargeJump.CancelChargeJump();
        _attack.CancelAttack();
        _dash.CancelDash();
        _glide.CancelGlide();
        _move.SetMovementLocked(_health, true);
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _simulationBeforeDeath = _rigidbody.simulated;
        _deathPhysicsStopped = true;
        _rigidbody.simulated = false;
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
