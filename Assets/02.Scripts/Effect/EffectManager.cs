using System.Collections;
using System.Collections.Generic;
using Game.Effects;
using UnityEngine;
using UnityEngine.Serialization;

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
    private readonly List<GameObject> _hitEffects = new();
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
        HitImpactFeedback.EnsureInstance();
    }

    private void OnDestroy()
    {
        ClearHitEffects();
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        ClearHitEffects();
    }

    private void ClearHitEffects()
    {
        foreach (GameObject effect in _hitEffects)
        {
            if (effect != null)
            {
                Destroy(effect);
            }
        }

        _hitEffects.Clear();
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
        if (!isActiveAndEnabled)
        {
            onReleased?.Invoke();
            return;
        }

        GameObject prefab = horizontal && _playerHitHorizontalVfx != null ? _playerHitHorizontalVfx : _playerHitVfx;
        GameObject effect = Spawn(prefab, position);
        if (effect != null && effect.TryGetComponent<PlayerHitEffect>(out var hitEffect))
        {
            TrackHitEffect(effect, hitEffect);
            hitEffect.Play(position, onReleased);
        }
        else
        {
            if (prefab == null)
            {
                Debug.LogWarning("EffectManager에 플레이어 피격 프리팹이 연결되지 않았습니다.", this);
            }
            HitImpactFeedback.EnsureInstance().RequestPlayerHitstop();
            if (effect != null)
            {
                TrackHitEffect(effect, null);
            }
            onReleased?.Invoke();
        }
    }

    public void PlayPlayerDeath(Vector3 position) => Spawn(_playerDeathVfx, position);
    public void PlayJump(Vector3 position) => Spawn(_jumpVfx, position);
    public void PlayDoubleJump(Vector3 position) => Spawn(_doubleJumpVfx, position);
    public void PlayLand(Vector3 position) => Spawn(_landVfx, position);
    public GameObject PlayDash(Vector3 position, int direction = 1) => Spawn(_dashVfx, position, direction);
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

    public void PlayBossHit(Vector3 position) => PlayBossHit(position, Vector2.right);

    public void PlayBossHit(Vector3 position, Vector2 direction)
    {
        if (!isActiveAndEnabled) return;
        GameObject effect = Spawn(_bossHitVfx, position);
        if (effect != null && effect.TryGetComponent<EnemyHitEffect>(out var enemyHit))
        {
            enemyHit.Play(position, direction);
        }
        else
        {
            HitImpactFeedback.EnsureInstance().RequestBossHitstop();
        }
        if (effect != null)
        {
            TrackHitEffect(effect, null);
        }
    }

    private void TrackHitEffect(GameObject effect, PlayerHitEffect hitEffect)
    {
        _hitEffects.Add(effect);
        foreach (ParticleSystem particle in effect.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = particle.main;
            main.useUnscaledTime = true;
        }

        foreach (Animator animator in effect.GetComponentsInChildren<Animator>(true))
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        StartCoroutine(CleanupHitEffect(effect, hitEffect));
    }

    private IEnumerator CleanupHitEffect(GameObject effect, PlayerHitEffect hitEffect)
    {
        yield return null;
        EnemyHitEffect enemyHit = effect != null ? effect.GetComponent<EnemyHitEffect>() : null;
        if (hitEffect == null && enemyHit == null)
        {
            yield return new WaitForSecondsRealtime(2f);
        }

        ParticleSystem[] particles;
        if (effect != null)
        {
            particles = effect.GetComponentsInChildren<ParticleSystem>(true);
        }
        else
        {
            particles = new ParticleSystem[0];
        }

        while (effect != null)
        {
            bool alive = false;
            if (enemyHit != null) alive = enemyHit.IsPlaying;
            if (hitEffect != null)
            {
                alive = hitEffect.IsPlaying;
            }

            foreach (ParticleSystem particle in particles)
            {
                if (particle != null && particle.IsAlive(true))
                {
                    alive = true;
                }
            }

            if (!alive)
            {
                break;
            }

            yield return null;
        }

        _hitEffects.Remove(effect);
        if (effect != null)
        {
            Destroy(effect);
        }
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
