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


    public void PlaySnowEffect(SnowEffectType type, Vector3 position, bool flipX = false)
    {
        GameObject effect = SnowEffectPool.Instance.GetEffect(type);
        effect.transform.position = position;

        SnowEffect snowEffect = effect.GetComponent<SnowEffect>();
        snowEffect.Play(type, SnowEffectPool.Instance, flipX);
    }

}
