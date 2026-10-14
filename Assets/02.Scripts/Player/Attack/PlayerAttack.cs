using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _comboResetDelay = 0.6f;
    [SerializeField, Min(0), Tooltip("공중 공격권 초기화 사이에 허용하는 공격 횟수")]
    private int _maxAirAttackCount = 3;
    [Header("공중 마지막 타격 보너스")]
    [SerializeField, Min(0), Tooltip("공중 마지막 타격 이펙트가 발생하는 공격에 추가할 피해량. 업그레이드·난이도 계산 후 더합니다.")]
    private int _airFinisherBonusDamage = 10;
    [SerializeField, Min(0), Tooltip("공중 마지막 타격이 적중하면 기존 Heal On Hit에 추가할 체력 회복량. 빗나가면 회복하지 않습니다.")]
    private int _airFinisherBonusHeal = 10;
    [Header("공중 공격권 초기화")]
    [SerializeField, Tooltip("1단 및 2단 이상 점프가 실제 발동하면 공중 공격권을 초기화합니다.")]
    private bool _resetAirAttackCountOnJump = false;
    [SerializeField, Tooltip("공중 Shift 대시가 실제 발동하면 공중 공격권을 초기화합니다.")]
    private bool _resetAirAttackCountOnAirDash = false;
    [SerializeField, Tooltip("투사체를 잡은 뒤 차징 점프로 실제 발사되면 공중 공격권을 초기화합니다.")]
    private bool _resetAirAttackCountOnChargeJump = false;
    private int _airAttackCount;
    public int CurrentAirAttackCount => _airAttackCount;
    [SerializeField, Min(0f), Tooltip("지상 공격 시 수평 이동 차단 시간(초). 0이면 차단 없음.")]
    private float _groundHorizontalLockDuration;
    [SerializeField, Min(0f), Tooltip("공중 공격 시 수평 이동 차단 시간(초). 0이면 차단 없음.")]
    private float _airHorizontalLockDuration;
    [SerializeField, Min(0f), Tooltip("공중 공격 시 수직 위치 고정 시간(초). 0이면 고정 없음.")]
    private float _airVerticalLockDuration;
    [SerializeField, Range(0f, 1f), Tooltip("공격 중 적용할 중력 배율. 0.5면 기존 중력의 50%.")]
    private float _attackGravityScaleMultiplier = 0.5f;
    [SerializeField, Min(0f)] private float _comboInputBuffer = 0.2f;
    [SerializeField, Min(0.01f), Tooltip("공격 오프셋과 인스턴스 크기에 일괄 적용되는 배율")]
    private float _attackScale = 1f;
    public float AttackScale
    {
        get => _attackScale;
        set => _attackScale = Mathf.Max(0.01f, value);
    }
    private PlayerAttackDataSO[] _combo;
    private int _nextCombo;
    private int _healOnHit;
    private float _comboExpires = -1f;
    private bool _queuedCombo;
    private bool _isComboAttack;
    public void RequestCombo(params PlayerAttackDataSO[] combo)
    {
        _combo = combo;
        if (IsAttacking)
        {
            float remaining = _windupDuration + _recoveryDuration - _elapsed;
            if (_isComboAttack && remaining <= _comboInputBuffer)
            {
                _queuedCombo = true;
            }
            return;
        }
        StartNextCombo();
    }
    private void StartNextCombo()
    {
        if (_combo == null || _combo.Length == 0)
        {
            return;
        }
        int index = _nextCombo;
        if (Time.time > _comboExpires)
        {
            index = 0;
        }
        if (!TryStartAttack(_combo[index]))
        {
            return;
        }
        _isComboAttack = true;
        _nextCombo = (index + 1) % _combo.Length;
    }
    private PlayerMove _move;
    private Game.Upgrades.PlayerUpgrades _upgrades;
    private Animator _animator;
    private float _previousAnimatorSpeed;
    private float _attackSpeed = 1f;
    private float _windupDuration;
    private float _recoveryDuration;
    private int _damage;
    private PlayerJump _jump;
    private PlayerController _controller;
    private Health _health;
    private PlayerAttackMotion _motion;
    private PlayerAttackDataSO _current;
    private PlayerAttackInstance _instance;
    private float _elapsed;
    private int _direction;
    private bool _spawned;
    private bool _currentIsThirdComboHit;
    public event Action OnAttackStarted;
    public event Action OnAttackEnded;
    public bool IsAttacking { get; private set; }
    public string CurrentAttackClip
    {
        get
        {
            if (_current != null)
            {
                return _current.AnimClipName;
            }
            return "FirstAttack";
        }
    }
    private void Awake()
    {
        PlayerRig rig = PlayerRig.Resolve(gameObject);
        _move = rig.Move;
        _upgrades = rig.Upgrades;
        _animator = rig.Animator;
        _jump = rig.Jump;
        _controller = rig.Controller;
        _health = rig.Health;
        _motion = new PlayerAttackMotion(this, _move, rig.Body);
    }
    private bool IsGrounded => _jump.IsGrounded;
    private void ResetAirAttackCount() => _airAttackCount = 0;
    public void ResetAirAttackCountOnJump() { if (_resetAirAttackCountOnJump) ResetAirAttackCount(); }
    public void ResetAirAttackCountOnAirDash() { if (_resetAirAttackCountOnAirDash) ResetAirAttackCount(); }
    public void ResetAirAttackCountOnChargeJump() { if (_resetAirAttackCountOnChargeJump) ResetAirAttackCount(); }
    private void OnEnable()
    {
        _jump.OnLanded += ResetAirAttackCount;
        _jump.OnLanded += CancelVerticalLock;
        _jump.OnJumpStarted += CancelVerticalLock;
    }
    private void OnDisable()
    {
        _jump.OnLanded -= ResetAirAttackCount;
        _jump.OnLanded -= CancelVerticalLock;
        _jump.OnJumpStarted -= CancelVerticalLock;
        CancelAttack();
        _motion.UnlockHorizontal();
        CancelVerticalLock();
    }
    private void CancelVerticalLock() => _motion.CancelVertical();

    public bool TryStartAttack(PlayerAttackDataSO definition)
    {
        if (!isActiveAndEnabled || GameManager.BlocksPlayerInput || IsAttacking || definition == null || !_controller.CanAttack)
        {
            return false;
        }
        if (!IsGrounded && _airAttackCount >= _maxAirAttackCount)
        {
            return false;
        }
        if (definition.AttackPrefab == null || string.IsNullOrWhiteSpace(definition.AnimClipName))
        {
            Debug.LogError($"공격 프리팹 또는 애니메이션이 비어 있습니다: {definition.name}", definition);
            return false;
        }
        int healthCost = Mathf.Max(0, definition.HealthCost);
        if (healthCost > 0)
        {
            if (_health.CurrentHealth <= healthCost)
            {
                return false;
            }
            _health.SpendHealth(healthCost);
        }
        _current = definition;
        _attackSpeed = 1f;
        if (_upgrades != null)
        {
            _attackSpeed = Mathf.Max(0.01f, _upgrades.AttackSpeedMultiplier);
        }
        _damage = definition.Damage;
        _healOnHit = Mathf.Max(0, definition.HealOnHit);
        if (_upgrades != null)
        {
            _damage = _upgrades.GetAttackDamage(definition.Damage);
        }
        _damage = Mathf.RoundToInt(_damage * GameDifficulty.AttackMultiplier);
        _windupDuration = Mathf.Max(0f, definition.WindupDuration) / _attackSpeed;
        _recoveryDuration = Mathf.Max(0.05f, definition.RecoveryDuration) / _attackSpeed;
        _elapsed = 0;
        _spawned = false;
        _direction = _move.FacingDirection;
        IsAttacking = true;
        bool startedInAir = !IsGrounded;
        if (startedInAir)
        {
            _airAttackCount++;
        }
        _currentIsThirdComboHit = startedInAir && _maxAirAttackCount > 0 && _airAttackCount >= _maxAirAttackCount;
        if (_currentIsThirdComboHit)
        {
            _damage += Mathf.Max(0, _airFinisherBonusDamage);
            _healOnHit += Mathf.Max(0, _airFinisherBonusHeal);
        }
        float horizontalLockDuration = _groundHorizontalLockDuration;
        if (startedInAir)
        {
            horizontalLockDuration = _airHorizontalLockDuration;
        }
        _motion.Begin(startedInAir, horizontalLockDuration / _attackSpeed, _airVerticalLockDuration / _attackSpeed, _attackGravityScaleMultiplier);
        _isComboAttack = false;
        _previousAnimatorSpeed = _animator.speed;
        _animator.speed = _previousAnimatorSpeed * _attackSpeed;
        OnAttackStarted?.Invoke();
        if (IsAttacking && definition.WindupDuration <= 0)
        {
            SpawnAttack();
        }
        return true;
    }
    private void Update()
    {
        _motion.Tick(Time.deltaTime);
        if (!IsAttacking)
        {
            return;
        }
        _elapsed += Time.deltaTime;
        if (!_spawned && _elapsed >= _windupDuration)
        {
            SpawnAttack();
        }
        if (IsAttacking && _elapsed >= _windupDuration + _recoveryDuration)
        {
            FinishAttack(true);
        }
    }
    private void FixedUpdate() => _motion.FixedTick(IsGrounded, Time.fixedDeltaTime);
    private void SpawnAttack()
    {
        _spawned = true;
        Vector3 position = transform.position + new Vector3(_direction * _current.SpawnOffset.x * _attackScale, _current.SpawnOffset.y * _attackScale);
        _instance = Instantiate(_current.AttackPrefab, position, Quaternion.identity);
        _instance.transform.localScale *= _attackScale;
        if (!_instance.Initialize(_damage, _direction, transform, _attackSpeed, _currentIsThirdComboHit, _healOnHit))
        {
            Destroy(_instance.gameObject);
            _instance = null;
            FinishAttack();
        }
    }
    public void CancelAttack()
    {
        FinishAttack();
        // 공격이 이미 끝났더라도 이전 위치 고정·중력·이동 잠금은 남을 수 있다.
        _motion.EndAttack();
        _motion.CancelVertical();
        _motion.UnlockHorizontal();
    }
    public void InterruptForDash() => InterruptForMovement();
    public void InterruptForMovement()
    {
        CancelAttack();
        _motion.UnlockHorizontal();
        CancelVerticalLock();
    }
    public void ResetForRespawn()
    {
        CancelAttack();
        _combo = null;
        _nextCombo = 0;
        _comboExpires = -1f;
        _queuedCombo = false;
        _isComboAttack = false;
        _airAttackCount = 0;
        _currentIsThirdComboHit = false;
        CancelVerticalLock();
        _motion.UnlockHorizontal();
    }
    private void FinishAttack(bool completed = false)
    {
        if (!IsAttacking)
        {
            _motion.EndAttack();
            return;
        }
        bool continueCombo = completed && _isComboAttack && _queuedCombo;
        _queuedCombo = false;
        _comboExpires = -1f;
        if (_isComboAttack)
        {
            _comboExpires = Time.time + _comboResetDelay;
        }
        IsAttacking = false;
        _animator.speed = _previousAnimatorSpeed;
        _motion.EndAttack();
        if (_instance != null)
        {
            _instance.EndPlayerAction();
        }
        _instance = null;
        OnAttackEnded?.Invoke();
        if (continueCombo)
        {
            StartNextCombo();
        }
    }
}
