using Game.Effects;
using UnityEngine;

[DefaultExecutionOrder(200)]
public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance = null;
    public static EffectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<EffectManager>();
            }

            return _instance;
        }
    }
    [Header("VFX 프리팹 (플레이어)")]
    [SerializeField] private GameObject _playerHitVfx;
    [SerializeField] private GameObject _playerHitHorizontalVfx;
    [SerializeField] private bool _useHorizontalPlayerHit;
    private readonly HitEffectService _hitEffects = new();
    [SerializeField] private GameObject _playerDeathVfx;
    [SerializeField] private GameObject _jumpVfx;
    [SerializeField] private GameObject _doubleJumpVfx;
    [SerializeField] private GameObject _landVfx;
    [SerializeField] private GameObject _dashVfx;
    [SerializeField] private GameObject _footstepVfx;
    [Header("VFX 프리팹 (차지 점프)")]
    [Tooltip("조준 중 표시. PlayerChargeJump가 조준 종료 시 제거합니다.")]
    [SerializeField] private GameObject _chargeJumpAimVfx;
    [Tooltip("플레이어 발사 순간 표시. 프리팹의 오른쪽(+X)이 발사 방향입니다.")]
    [SerializeField] private GameObject _chargeJumpLaunchVfx;
    [Tooltip("반대 방향으로 발사되는 투사체에 표시. 오른쪽(+X)이 발사 방향입니다.")]
    [SerializeField] private GameObject _chargeJumpProjectileLaunchVfx;
    [Header("VFX 프리팹 (적)")]
    [SerializeField] private GameObject _bossHitVfx;
    [SerializeField] private SFXData _bossHitSound;
    [SerializeField] private GameObject _bossDeathVfx;
    [Header("VFX 프리팹 (맵)")]
    [SerializeField] private GameObject _checkpointVfx;
    [Header("VFX 프리팹 (레벨·벽)")]
    [SerializeField] private GameObject _levelUpVfx;
    [SerializeField] private GameObject _wallSlideVfx;
    [Header("VFX 프리팹 (기타)")]
    [SerializeField] private GameObject _respawnVfx;
    [SerializeField] private GameObject _clickVfx;

    [Header("그로기 발동 연출")]
    [SerializeField] private AirBossStagger _bossStagger;
    [SerializeField] private GameObject _staggerHitVfx;
    [SerializeField] private SFXData _staggerHitSound;

    private bool _wasStaggered;
    private Vector3 _lastBossHitPosition;
    private Vector2 _lastBossHitDirection;
    private int _lastBossHitFrame = -1;
    private int _pendingBossHitSounds;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        HitImpactFeedback.EnsureInstance();
    }

    private void OnDestroy()
    {
        _hitEffects.Clear();
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void OnDisable()
    {
        _pendingBossHitSounds = 0;
        StopAllCoroutines();
        _hitEffects.Clear();
    }

    private void OnEnable()
    {
        _wasStaggered = _bossStagger != null && _bossStagger.IsRunning;
        _lastBossHitFrame = -1;
        _pendingBossHitSounds = 0;
    }

    public void PlaySnowEffect(SnowEffectType type, Vector3 position, Vector3 scale, bool flipX = false)
    {
        GameObject effect = SnowEffectPool.Instance.GetEffect(type);
        effect.transform.position = position;
        SnowEffect snowEffect = effect.GetComponent<SnowEffect>();
        snowEffect.Play(type, SnowEffectPool.Instance, scale, flipX);
    }

    public void PlayBossEffect(EliteMonsterEffect prefab, Transform point)
    {
        if (prefab == null)
        {
            return;
        }

        if (point == null)
        {
            return;
        }

        EliteMonsterEffect eliteMonsterEffect = Instantiate(prefab, point);
        eliteMonsterEffect.transform.localPosition = Vector3.zero;
        eliteMonsterEffect.transform.localRotation = Quaternion.identity;
    }

    public void PlayPlayerHit(Vector3 position, System.Action onReleased = null) =>
        PlayPlayerHit(position, _useHorizontalPlayerHit, onReleased);

    public void PlayPlayerHit(Vector3 position, bool horizontal, System.Action onReleased = null)
    {
        GameObject prefab = horizontal && _playerHitHorizontalVfx != null ? _playerHitHorizontalVfx : _playerHitVfx;
        _hitEffects.PlayPlayerHit(this, prefab, position, onReleased);
    }

    public void PlayPlayerDeath(Vector3 position) => Spawn(_playerDeathVfx, position);
    public void PlayJump(Vector3 position) => Spawn(_jumpVfx, position);
    public void PlayDoubleJump(Vector3 position) => Spawn(_doubleJumpVfx, position);
    public void PlayLand(Vector3 position) => Spawn(_landVfx, position);
    public GameObject PlayDash(Vector3 position, int direction = 1) => Spawn(_dashVfx, position, direction);
    public GameObject PlayDash(Vector3 position, Vector2 direction)
    {
        var effect = Spawn(_dashVfx, position);
        if (effect != null && direction.sqrMagnitude > 0f)
            effect.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        return effect;
    }
    public void PlayFootstep(Vector3 position) => Spawn(_footstepVfx, position);

    public GameObject PlayChargeJumpAim(Vector3 position)
    {
        if (!isActiveAndEnabled) return null;
        var effect = Spawn(_chargeJumpAimVfx, position);
        if (effect == null) return null;
        foreach (var particle in effect.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = particle.main;
            main.useUnscaledTime = true;
        }
        foreach (var animator in effect.GetComponentsInChildren<Animator>(true))
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        return effect;
    }

    public GameObject PlayChargeJumpLaunch(Vector3 position, Vector2 direction) =>
        SpawnChargeJumpBurst(_chargeJumpLaunchVfx, position, direction);

    public GameObject PlayChargeJumpProjectileLaunch(Vector3 position, Vector2 direction) =>
        SpawnChargeJumpBurst(_chargeJumpProjectileLaunchVfx, position, direction);

    private GameObject SpawnChargeJumpBurst(GameObject prefab, Vector3 position, Vector2 direction)
    {
        if (!isActiveAndEnabled || prefab == null) return null;
        float angle = direction.sqrMagnitude > 0.0001f ? Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg : 0f;
        var effect = Instantiate(prefab, position, Quaternion.Euler(0f, 0f, angle));
        // 기존 VFX와 같은 수명 관리. 단순 프리팹도 기본 한 번 재생 후 정리합니다.
        if (effect.GetComponent<OneShotVFX>() == null) effect.AddComponent<OneShotVFX>();
        return effect;
    }

    private void LateUpdate()
    {
        bool running = _bossStagger != null
        && _bossStagger.isActiveAndEnabled
        && _bossStagger.IsRunning;

        bool enteredOnHit = !_wasStaggered && running
            && _lastBossHitFrame == Time.frameCount;

        if (enteredOnHit && _staggerHitVfx != null)
        {
            _hitEffects.PlayBossHit(
                this,
                _staggerHitVfx,
                _lastBossHitPosition,
                _lastBossHitDirection);
        }

        // 보스의 LateUpdate 진입 판정 후, 마지막 타격음 하나만 강화음으로 교체합니다.
        bool useStaggerSound = enteredOnHit && _staggerHitSound != null
            && _staggerHitSound.Clip != null;
        for (int i = 0; i < _pendingBossHitSounds; i++)
        {
            bool isStaggerHit = useStaggerSound && i == _pendingBossHitSounds - 1;
            AttackOneShotAudio.Play(isStaggerHit ? _staggerHitSound : _bossHitSound,
                isStaggerHit ? 1f : Random.Range(0.9f, 1.1f));
        }
        _pendingBossHitSounds = 0;
        _wasStaggered = running;
    }

    public void PlayBossHit(Vector3 position) => PlayBossHit(position, Vector2.right);

    public void PlayBossHit(Vector3 position, Vector2 direction)
    {
        if (!isActiveAndEnabled) return;
        _pendingBossHitSounds++;
        // AirBossStagger는 타격 처리 후 LateUpdate에서 그로기에 진입합니다.
        _lastBossHitPosition = position;
        _lastBossHitDirection = direction;
        _lastBossHitFrame = Time.frameCount;
        _hitEffects.PlayBossHit(this, _bossHitVfx, position, direction);
    }

    public void PlayBossDeath(Vector3 position) => Spawn(_bossDeathVfx, position);
    public void PlayCheckpoint(Vector3 position) => Spawn(_checkpointVfx, position);
    public void PlayLevelUp(Vector3 position) => Spawn(_levelUpVfx, position);
    public void PlayWallSlide(Vector3 position) => Spawn(_wallSlideVfx, position);
    public void PlayRespawn(Vector3 position) => Spawn(_respawnVfx, position);
    public void PlayClick(Vector3 position) => Spawn(_clickVfx, position);

    private GameObject Spawn(GameObject prefab, Vector3 position, int direction = 1)
    {
        if (prefab == null)
        {
            return null;
        }

        GameObject spawned = Instantiate(prefab, position, Quaternion.identity);
        if (direction < 0)
        {
            spawned.GetComponent<VFXFlip>()?.Apply(direction);
        }

        return spawned;
    }
}
