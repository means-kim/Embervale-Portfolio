using UnityEngine;

public class EliteMonsterIdleEffect : MonoBehaviour
{
    [SerializeField] private EliteMonsterEffect _effectPrefab;
    [SerializeField] private Transform _effectPoint;

    public void PlayIdleEffect()
    {
        if (EffectManager.Instance == null) return;

        EffectManager.Instance.PlayBossEffect(_effectPrefab, _effectPoint);
    }
}