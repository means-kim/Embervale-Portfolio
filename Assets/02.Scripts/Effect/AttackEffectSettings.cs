using UnityEngine;

namespace Game.Effects
{
    // 섬광과 가시선 전용. 보스 몸체 번쩍임은 기존 EliteMonsterHit이 담당한다.
    [CreateAssetMenu(menuName = "Effects/Enemy Hit Effect Settings", fileName = "AttackEffectSettings")]
    public sealed class AttackEffectSettings : ScriptableObject
    {
        [Min(0.01f)] public float globalScale = 1f;
        public string sortingLayerName = "GA1_HD_FX";
        [Header("Hit Stop")]
        [Min(0f)] public float hitStopDuration = 0.05f;
        [Range(0f, 1f)] public float hitStopTimeScale = 0f;
        [Header("Impact Streak")]
        public float streakLength = 22f;
        public float streakWidth = 2f;
        public Vector2 streakTiltRange = new Vector2(5f, 20f);
        [Min(0.001f)] public float streakGrowDuration = 0.033f;
        [Min(0f)] public float streakHoldDuration = 0.12f;
        [Min(0.01f)] public float streakFadeDuration = 0.12f;
        [Range(0f, 1f)] public float streakThinOnFade = 0.8f;
        public Color streakColor = Color.white;
        public int streakSortingOrder = 150;
        [Header("Spikes")]
        [Min(0)] public int spikeCount = 3;
        public float spikeLength = 4.5f;
        public Vector2 spikeLengthJitter = new Vector2(0.7f, 1.1f);
        public float spikeWidth = 1.3f;
        public Vector2 spikeAngleRange = new Vector2(55f, 80f);
        public Vector2 spikeAreaMin = new Vector2(-3.5f, 1f);
        public Vector2 spikeAreaMax = new Vector2(-0.5f, 2.5f);
        [Min(0.01f)] public float spikeFadeDuration = 0.22f;
        public Color spikeColor = Color.white;
        [Header("Camera Shake")]
        [Min(0f)] public float shakeAmplitude = 0.08f;
        [Min(0.01f)] public float shakeDuration = 0.1f;
        public float ImpactDuration => Mathf.Max(streakHoldDuration + streakFadeDuration, spikeFadeDuration);
    }
}
