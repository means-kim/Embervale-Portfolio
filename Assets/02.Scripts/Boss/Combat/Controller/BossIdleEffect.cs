using UnityEngine;

public class BossIdleEffect : MonoBehaviour
{
    [SerializeField] private BossEffect _effectPrefab;
    [SerializeField] private Transform _effectPoint;

    public void PlayIdleEffect()
    {
        if (EffectManager.Instance == null) return;

        EffectManager.Instance.PlayBossEffect(_effectPrefab, _effectPoint);
    }
}