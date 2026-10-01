using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance = null;
    public static EffectManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<EffectManager>();
            return _instance;
        }
    }

    [Header("VFX 프리팹 (플레이어)")]
    [SerializeField] private GameObject _playerHitVfx;
    [SerializeField] private GameObject _playerDeathVfx;
    [SerializeField] private GameObject _jumpVfx;
    [SerializeField] private GameObject _doubleJumpVfx;
    [SerializeField] private GameObject _dashVfx;
    [SerializeField] private GameObject _footstepVfx;

    [Header("VFX 프리팹 (적)")]
    [SerializeField] private GameObject _enemyHitVfx;
    [SerializeField] private GameObject _enemyDeathVfx;

    [Header("VFX 프리팹 (플레이어 공격 5종)")]
    [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("_attackBasicVfx")] private GameObject _attackSideAttackVfx;
    [SerializeField] private GameObject _attackStabVfx;
    [SerializeField] private GameObject _attackHorizontalVfx;
    [SerializeField] private GameObject _attackDownVfx;
    [SerializeField] private GameObject _attackUpVfx;

    [Header("VFX 프리팹 (맵)")]
    [SerializeField] private GameObject _checkpointVfx;

    [Header("VFX 프리팹 (레벨·벽)")]
    [SerializeField] private GameObject _levelUpVfx;
    [SerializeField] private GameObject _wallSlideVfx;
    [SerializeField] private GameObject _wallClimbUpVfx;
    [SerializeField] private GameObject _wallClimbDownVfx;

    [Header("VFX 프리팹 (기타)")]
    [SerializeField] private GameObject _respawnVfx;
    [SerializeField] private GameObject _clickVfx;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
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
        if (prefab == null) return;
        if (point == null) return;

        EliteMonsterEffect eliteMonsterEffect = Instantiate(prefab, point);

        eliteMonsterEffect.transform.localPosition = Vector3.zero;
        eliteMonsterEffect.transform.localRotation = Quaternion.identity;
    }

    // 아래 단발 VFX는 인스펙터에 프리팹을 넣으면 해당 타이밍에 자동 소환된다.
    // 비어 있는 칸은 무시된다. 사운드는 프리팹에 직접 달아 두고,
    // 소환된 프리팹의 수명(재생 후 삭제)은 프리팹 쪽 OneShotVFX가 관리한다.
    // OneShotVFX._lifeTime이 0이면 애니/사운드/파티클 중 가장 긴 것 + _extraDelay 후 파괴된다.
    // CFXR_Effect 프리팹은 clearBehavior=None으로 두고 OneShotVFX에 수명을 맡긴다.
    public void PlayPlayerHit(Vector3 position) => Spawn(_playerHitVfx, position);
    public void PlayPlayerDeath(Vector3 position) => Spawn(_playerDeathVfx, position);
    public void PlayJump(Vector3 position) => Spawn(_jumpVfx, position);
    public void PlayDoubleJump(Vector3 position) => Spawn(_doubleJumpVfx, position);
    public GameObject PlayDash(Vector3 position, int direction = 1) => Spawn(_dashVfx, position, direction);
    public void PlayFootstep(Vector3 position) => Spawn(_footstepVfx, position);
    public void PlayEnemyHit(Vector3 position) => Spawn(_enemyHitVfx, position);
    public void PlayEnemyDeath(Vector3 position) => Spawn(_enemyDeathVfx, position);
    public GameObject PlayAttackSideAttack(Vector3 position, int direction = 1) => Spawn(_attackSideAttackVfx, position, direction);
    public GameObject PlayAttackStab(Vector3 position, int direction = 1) => Spawn(_attackStabVfx, position, direction);
    public GameObject PlayAttackHorizontal(Vector3 position, int direction = 1) => Spawn(_attackHorizontalVfx, position, direction);
    public GameObject PlayAttackDown(Vector3 position, int direction = 1) => Spawn(_attackDownVfx, position, direction);
    public GameObject PlayAttackUp(Vector3 position, int direction = 1) => Spawn(_attackUpVfx, position, direction);
    public void PlayCheckpoint(Vector3 position) => Spawn(_checkpointVfx, position);
    public void PlayLevelUp(Vector3 position) => Spawn(_levelUpVfx, position);
    public void PlayWallSlide(Vector3 position) => Spawn(_wallSlideVfx, position);
    public void PlayWallClimbUp(Vector3 position) => Spawn(_wallClimbUpVfx, position);
    public void PlayWallClimbDown(Vector3 position) => Spawn(_wallClimbDownVfx, position);
    public void PlayRespawn(Vector3 position) => Spawn(_respawnVfx, position);
    public void PlayClick(Vector3 position) => Spawn(_clickVfx, position);

    private GameObject Spawn(GameObject prefab, Vector3 position, int direction = 1)
    {
        if (prefab == null) return null;
        GameObject spawned = Instantiate(prefab, position, Quaternion.identity);
        // VFXFlip이 붙은 프리팹만 왼쪽 스폰 시 뒤집는다. 없으면 기존처럼 그대로.
        if (direction < 0)
            spawned.GetComponent<VFXFlip>()?.Apply(direction);
        return spawned;
    }
}
