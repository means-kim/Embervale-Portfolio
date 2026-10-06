using UnityEngine;
using UnityEngine.Serialization;
public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance = null;
    public static EffectManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindFirstObjectByType<EffectManager>();
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
    [FormerlySerializedAs("_enemyHitVfx")]
    [SerializeField] private GameObject _bossHitVfx;
    [FormerlySerializedAs("_enemyDeathVfx")]
    [SerializeField] private GameObject _bossDeathVfx;
    [Header("VFX 프리팹 (맵)")]
    [SerializeField] private GameObject _checkpointVfx;
    [Header("VFX 프리팹 (레벨·벽)")]
    [SerializeField] private GameObject _levelUpVfx;
    [SerializeField] private GameObject _wallSlideVfx;
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
        if (_instance == this) _instance = null;
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
    public void PlayPlayerHit(Vector3 position) => Spawn(_playerHitVfx, position);
    public void PlayPlayerDeath(Vector3 position) => Spawn(_playerDeathVfx, position);
    public void PlayJump(Vector3 position) => Spawn(_jumpVfx, position);
    public void PlayDoubleJump(Vector3 position) => Spawn(_doubleJumpVfx, position);
    public GameObject PlayDash(Vector3 position, int direction = 1) => Spawn(_dashVfx, position, direction);
    public void PlayFootstep(Vector3 position) => Spawn(_footstepVfx, position);
    public void PlayBossHit(Vector3 position) => Spawn(_bossHitVfx, position);
    public void PlayBossDeath(Vector3 position) => Spawn(_bossDeathVfx, position);
    public void PlayCheckpoint(Vector3 position) => Spawn(_checkpointVfx, position);
    public void PlayLevelUp(Vector3 position) => Spawn(_levelUpVfx, position);
    public void PlayWallSlide(Vector3 position) => Spawn(_wallSlideVfx, position);
    public void PlayRespawn(Vector3 position) => Spawn(_respawnVfx, position);
    public void PlayClick(Vector3 position) => Spawn(_clickVfx, position);
    private GameObject Spawn(GameObject prefab, Vector3 position, int direction = 1)
    {
        if (prefab == null) return null;
        GameObject spawned = Instantiate(prefab, position, Quaternion.identity);
        if (direction < 0) spawned.GetComponent<VFXFlip>()?.Apply(direction);
        return spawned;
    }
}
