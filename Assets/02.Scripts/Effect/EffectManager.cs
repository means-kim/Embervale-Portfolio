using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance = null;
    public static EffectManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void Start()
    {
        BackgroundMusic.PlayBgm(
            BackgroundMusic.BgmType.Snow,
            volume: 0.08f);
    }


    public void PlaySnowEffect(SnowEffectType type, Vector3 position, Vector3 scale, bool flipX = false)
    public void PlaySnowEffect(SnowEffectType type, Vector3 position, bool flipX = false)
    {
        GameObject effect = SnowEffectPool.Instance.GetEffect(type);
        effect.transform.position = position;

        SnowEffect snowEffect = effect.GetComponent<SnowEffect>();
        snowEffect.Play(type, SnowEffectPool.Instance, scale, flipX);
    }

    public void PlayBossEffect(BossEffect prefab, Transform point)
    {
        if (prefab == null) return;
        if (point == null) return;

        BossEffect bossEffect = Instantiate(prefab, point);

        bossEffect.transform.localPosition = Vector3.zero;
        bossEffect.transform.localRotation = Quaternion.identity;
    }
}
}