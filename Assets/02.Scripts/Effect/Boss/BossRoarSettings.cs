using UnityEngine;

namespace Game.Boss.Roar
{
    [CreateAssetMenu(fileName = "BossRoarSettings", menuName = "Game/Boss/Roar Settings")]
    public class BossRoarSettings : ScriptableObject
    {
        [Header("Rings (측정: 0.176s 간격, 초당 5.2 화면높이, 반치폭 0.12)")]
        [Tooltip("링 생성 간격(초)")]
        [Min(0.03f)] public float ringInterval = 0.176f;
        [Tooltip("링 확장 속도 (화면 높이/초)")]
        [Min(0.1f)] public float ringSpeed = 5.2f;
        [Tooltip("링 띠 두께(반치폭, 화면 높이 단위)")]
        [Range(0.01f, 0.4f)] public float thickness = 0.10f;
        [Tooltip("생성 직후 반지름")]
        [Range(0f, 0.3f)] public float startRadius = 0.05f;
        [Tooltip("링 기본 세기 (Additive 밝기)")]
        [Range(0f, 3f)] public float ringIntensity = 0.5f;
        [Tooltip("첫 링 세기 배율 (포효 첫 타격감)")]
        [Range(1f, 3f)] public float firstRingBoost = 1.6f;
        [Tooltip("링 세기 랜덤 편차")]
        [Range(0f, 0.5f)] public float intensityJitter = 0.15f;

        [Header("Ring Shape")]
        [Range(8, 128)] public int spikeCount = 96;
        [Tooltip("스파이크 길이 (화면 높이 단위)")]
        [Range(0f, 0.5f)] public float spikeLength = 0.08f;
        [Tooltip("0 = 매끈한 원, 1 = 많이 끊긴 원")]
        [Range(0f, 1f)] public float breakup = 0.75f;

        [Header("Colors (기본값 = Ember 프리셋. 다른 색은 BossRoarPresets 참고)")]
        [ColorUsage(false, true)] public Color coreColor = new Color(0.92f, 0.97f, 1.0f);
        [ColorUsage(false, true)] public Color edgeColor = new Color(1.0f, 0.42f, 0.12f);
        public Color tintColor = new Color(0.03f, 0.06f, 0.12f);
        [Tooltip("0 = 빛만 더함, 1 = 링이 화면을 덮어 가림 (검은 링 프리셋용)")]
        [Range(0f, 1f)] public float ringOcclusion = 0f;
        [ColorUsage(false, true)] public Color flashColor = new Color(1.0f, 0.85f, 0.65f);

        [Header("Screen")]
        [Tooltip("포효 중 화면 어둡게 (0~1)")]
        [Range(0f, 1f)] public float tintAmount = 0.45f;
        [Range(0f, 1f)] public float vignette = 0.7f;
        [Tooltip("링이 생길 때마다 더해지는 화면 밝기 펄스")]
        [Range(0f, 1f)] public float pulsePerRing = 0.05f;
        [Tooltip("포효 시작 순간 플래시")]
        [Range(0f, 2f)] public float startFlash = 0.3f;
        [Tooltip("플래시 감쇠 속도 (1/초)")]
        [Min(0.1f)] public float flashDecay = 9f;

        [Header("Envelope")]
        [Min(0f)] public float fadeIn = 0.12f;
        [Min(0f)] public float fadeOut = 0.45f;

        [Header("Camera Shake (화면 높이 단위, 측정: 포효 중 잔진동 약 1.2%)")]
        [Range(0f, 0.1f)] public float shakeOnStart = 0.035f;
        [Range(0f, 0.05f)] public float shakeSustain = 0.011f;
        [Min(1f)] public float shakeFrequency = 28f;
    }
}
