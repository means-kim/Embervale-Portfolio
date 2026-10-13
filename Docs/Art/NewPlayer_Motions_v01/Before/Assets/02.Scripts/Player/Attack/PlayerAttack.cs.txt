using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMove))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _comboResetDelay = 0.6f;
    [SerializeField, Min(0), Tooltip("공중 공격권 초기화 사이에 허용하는 공격 횟수. 평타·스킬 공통.")]
    private int _maxAirAttackCount = 3;
    [Header("공중 공격권 초기화")]
    [SerializeField, Tooltip("1단 및 2단 이상 점프가 실제 발동하면 공중 공격권을 초기화합니다.")]
    private bool _resetAirAttackCountOnJump = false;
    [SerializeField, Tooltip("공중 Shift 대시가 실제 발동하면 공중 공격권을 초기화합니다. 지상 대시는 제외합니다.")]
    private bool _resetAirAttackCountOnAirDash = false;
    [SerializeField, Tooltip("투사체를 잡은 뒤 차징 점프로 실제 발사되면 공중 공격권을 초기화합니다. 조준과 취소는 제외합니다.")]
    private bool _resetAirAttackCountOnChargeJump = false;
    private int _airAttackCount;
    public int MaxAirAttackCount => _maxAirAttackCount;
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
    [SerializeField, Min(0.01f), Tooltip("공격 오프셋과 인스턴스 크기에 일괄 적용되는 배율. 기준 1.")]
    private float _attackScale = 1f;
    public float AttackScale
    {
        get => _attackScale;
        set => _attackScale = Mathf.Max(0.01f, value);
    }
    private PlayerAttackDataSO[] _combo;
    private int _nextCombo;
    private float _comboExpires = -1f;
    private bool _queuedCombo;
    private bool _isComboAttack;
    public void RequestCombo(params PlayerAttackDataSO[] combo)
    {
        _combo = combo;
        if (IsAttacking)
        {
            float remaining = _windupDuration + _recoveryDuration - _elapsed;
            if (_isComboAttack && remaining <= _comboInputBuffer) _queuedCombo = true;
            return;
        }
        StartNextCombo();
    }
    private void StartNextCombo()
    {
        if (_combo == null || _combo.Length == 0) return;
        int index = Time.time > _comboExpires ? 0 : _nextCombo;
        if (!TryStartAttack(_combo[index])) return;
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
    private PlayerAttackMotion _motion;
    private PlayerAttackDataSO _current;
    private PlayerAttackInstance _instance;
    private float _elapsed;
    private int _direction;
    private bool _spawned;
    public event Action OnAttackStarted;
    public event Action OnAttackEnded;
    public bool IsAttacking { get; private set; }
    public string CurrentAttackClip => _current != null ? _current.AnimClipName : "LeftHorizontalAttack";
    private void Awake()
    {
        _move = GetComponent<PlayerMove>();
        _upgrades = GetComponent<Game.Upgrades.PlayerUpgrades>();
        _animator = GetComponent<Animator>();
        _jump = GetComponent<PlayerJump>();
        _motion = new PlayerAttackMotion(this, _move, GetComponent<Rigidbody2D>());
    }
    private bool IsGrounded => _jump != null && _jump.IsGrounded;
    private void ResetAirAttackCount() => _airAttackCount = 0;
    public void ResetAirAttackCountOnJump() { if (_resetAirAttackCountOnJump) ResetAirAttackCount(); }
    public void ResetAirAttackCountOnAirDash() { if (_resetAirAttackCountOnAirDash) ResetAirAttackCount(); }
    public void ResetAirAttackCountOnChargeJump() { if (_resetAirAttackCountOnChargeJump) ResetAirAttackCount(); }
    private void OnEnable()
    {
        if (_jump == null) _jump = GetComponent<PlayerJump>();
        if (_jump != null)
        {
            _jump.OnLanded += ResetAirAttackCount;
            _jump.OnLanded += CancelVerticalLock;
            _jump.OnJumpStarted += CancelVerticalLock;
        }
    }
    private void OnDisable()
    {
        if (_jump != null)
        {
            _jump.OnLanded -= ResetAirAttackCount;
            _jump.OnLanded -= CancelVerticalLock;
            _jump.OnJumpStarted -= CancelVerticalLock;
        }
        CancelAttack();
        _motion.UnlockHorizontal();
        CancelVerticalLock();
    }
    private void CancelVerticalLock() => _motion.CancelVertical();

    // 공격 허용 여부는 PlayerController가 결정한다. 입력이나 프리팹 종류를 판별하지 않는다.
    public bool TryStartAttack(PlayerAttackDataSO definition)
    {
        var controller = GetComponent<PlayerController>();
        if (!isActiveAndEnabled || IsAttacking || definition == null || controller == null || !controller.CanAttack)
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
            var health = GetComponent<Health>();
            // 자해로 죽지 않도록 체력이 코스트 이하이면 발동 불가.
            if (health == null || health.CurrentHealth <= healthCost) return false;
            health.SpendHealth(healthCost);
        }
        _current = definition;
        // 이번 공격의 수치를 고정하여 도중에 강화되어도 타격과 연출이 어긋나지 않게 합니다.
        _attackSpeed = _upgrades != null ? Mathf.Max(0.01f, _upgrades.AttackSpeedMultiplier) : 1f;
        _damage = _upgrades != null ? _upgrades.GetAttackDamage(definition.Damage) : definition.Damage;
        _damage = Mathf.RoundToInt(_damage * GameDifficulty.AttackMultiplier);
        _windupDuration = Mathf.Max(0f, definition.WindupDuration) / _attackSpeed;
        _recoveryDuration = Mathf.Max(0.05f, definition.RecoveryDuration) / _attackSpeed;
        _elapsed = 0;
        _spawned = false;
        _direction = _move.FacingDirection;
        IsAttacking = true;
        bool startedInAir = !IsGrounded;
        if (startedInAir) _airAttackCount++;
        float horizontalLockDuration = startedInAir ? _airHorizontalLockDuration : _groundHorizontalLockDuration;
        _motion.Begin(startedInAir, horizontalLockDuration / _attackSpeed, _airVerticalLockDuration / _attackSpeed, _attackGravityScaleMultiplier);
        _isComboAttack = false;
        if (_animator != null)
        {
            _previousAnimatorSpeed = _animator.speed;
            _animator.speed = _previousAnimatorSpeed * _attackSpeed;
        }
        OnAttackStarted?.Invoke();
        if (IsAttacking && definition.WindupDuration <= 0) SpawnAttack();
        return true;
    }
    private void Update()
    {
        _motion.Tick(Time.deltaTime);
        if (!IsAttacking) return;
        _elapsed += Time.deltaTime;
        if (!_spawned && _elapsed >= _windupDuration) SpawnAttack();
        if (IsAttacking && _elapsed >= _windupDuration + _recoveryDuration) FinishAttack(true);
    }
    private void FixedUpdate() => _motion.FixedTick(IsGrounded, Time.fixedDeltaTime);
    private void SpawnAttack()
    {
        _spawned = true;
        Vector3 position = transform.position + new Vector3(_direction * _current.SpawnOffset.x * _attackScale, _current.SpawnOffset.y * _attackScale);
        _instance = Instantiate(_current.AttackPrefab, position, Quaternion.identity);
        _instance.transform.localScale *= _attackScale;
        if (!_instance.Initialize(_damage, _direction, transform, _attackSpeed))
        {
            Destroy(_instance.gameObject);
            _instance = null;
            FinishAttack();
        }
    }
    public void CancelAttack() => FinishAttack();
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
        CancelVerticalLock();
        _motion.UnlockHorizontal();
    }
    private void FinishAttack(bool completed = false)
    {
        if (!IsAttacking) return;
        bool continueCombo = completed && _isComboAttack && _queuedCombo;
        _queuedCombo = false;
        _comboExpires = _isComboAttack ? Time.time + _comboResetDelay : -1f;
        IsAttacking = false;
        if (_animator != null)
        {
            _animator.speed = _previousAnimatorSpeed;
        }
        _motion.EndAttack(continueCombo);
        if (_instance != null) _instance.EndPlayerAction();
        _instance = null;
        // 수평락은 타이머 만료 시 해제된다. 콤보가 이어지면 중력 저하를 유지하고 다음 타를 바로 시작한다.
        OnAttackEnded?.Invoke();
        if (continueCombo) StartNextCombo();
    }
}
