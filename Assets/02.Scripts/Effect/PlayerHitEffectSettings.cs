using System;
using UnityEngine;

namespace Game.Effects
{
    /// <summary>
    /// 플레이어 피격 이펙트의 모든 튜닝 값을 담는 단일 데이터 소스(SSoT).
    /// 기본값은 "카메라 Orthographic Size 10 (화면 세로 20유닛)" 기준으로 원본 영상을 측정해 환산한 값이다.
    /// 프로젝트의 카메라 스케일이 다르면 <see cref="globalScale"/>만 조절하면 된다.
    /// </summary>
    [CreateAssetMenu(menuName = "Effects/Player Hit Effect Settings", fileName = "PlayerHitEffectSettings")]
    public sealed class PlayerHitEffectSettings : ScriptableObject
    {
        [Header("Global")]
        [Header("크기/속도/셰이크에 일괄 곱해지는 배율. 카메라 스케일 차이를 여기서 흡수한다.")]
        [Min(0.01f)]
        public float globalScale = 1f;

        [Header("Colors (텍스처는 흰색 마스크, 여기서 틴트)")]
        [Header("스플랫 색. 기본: 붉은색.")] public Color splatColor = new Color(0.58f, 0.05f, 0.08f, 1f);
        [Header("균열선 색.")]
        public Color crackColor = new Color(1f, 0.99f, 0.94f, 1f);
        [Header("화면 딤(비네트) 색. 알파는 dim 값이 별도로 곱해진다.")]
        public Color dimColor = new Color(0.30f, 0.02f, 0.04f, 1f);

        [Header("Hit Stop (정지 구간)")]
        [Header("피격 직후 게임이 멈추는 시간(초, unscaled). 원본 ≈ 8프레임@30fps.")]
        [Min(0f)]
        public float freezeDuration = 0.27f;
        [Header("정지 구간 동안의 Time.timeScale. 0이면 완전 정지.")]
        [Range(0f, 1f)] public float freezeTimeScale = 0f;

        [Header("Splat (검은 방사형 그림자)")]
        [Header("비워두면 프리팹의 기본 스프라이트(원형) 사용. 가로 버전은 HitSplatHorizontal.")]
        public Sprite splatSprite;
        [Header("스플랫의 월드 크기(가로가 더 넓은 타원).")]
        public Vector2 splatSize = new Vector2(13f, 9.2f);
        public Vector2 splatOffset = Vector2.zero;
        [Header("등장 시 작은 크기에서 1.0으로 튀어나오는 시간.")]
        [Min(0f)] public float splatPopDuration = 0.066f;
        [Range(0.1f, 1f)] public float splatPopStartScale = 0.85f;
        [Header("정지 해제 후 커지면서 사라지는 시간.")]
        [Min(0.01f)] public float splatReleaseDuration = 0.10f;
        [Header("해제 시 최종 배율(x, y). 가로 버전은 x로 늘리고 y로 눌러 찢어지는 느낌을 준다.")]
        public Vector2 splatReleaseScale = new Vector2(1.7f, 1.7f);

        [Header("Crack (흰 번개 균열선)")]
        public Vector2 crackSize = new Vector2(40f, 8.3f);
        [Header("스플랫 중심 기준 오프셋. 원본은 살짝 아래에 위치.")]
        public Vector2 crackOffset = new Vector2(0f, -1.9f);
        [Min(0.01f)] public float crackFadeDuration = 0.14f;
        [Header("정지 구간 동안 깜빡이는 주기(초).")]
        [Min(0.005f)] public float crackFlickerInterval = 1f / 30f;
        [Range(0f, 1f)] public float crackFlickerMinAlpha = 0.85f;
        // 페이드 아웃 동안 가로로 줄어드는 비율
        [Range(0f, 1f)] public float crackShrinkOnFade = 0.3f;

        [Header("Screen Dim (화면 어두워짐)")]
        [Range(0f, 1f)] public float dimDuringFreeze = 0.55f;
        [Range(0f, 1f)] public float dimAfterRelease = 0.35f;
        [Min(0.01f)] public float dimBlendDuration = 0.15f;
        [Min(0.01f)] public float dimRecoverDuration = 1.2f;

        [Header("Particle Bursts (해제 순간 폭발)")]
        public BurstSettings shadeBurst = new BurstSettings
        {
            color = new Color(0.74f, 0.08f, 0.10f, 1f), // 붉은 덩어리
            count = 150,
            speedRange = new Vector2(14f, 78f),
            sizeRange = new Vector2(0.22f, 0.78f),
            lifetimeRange = new Vector2(1.0f, 2.0f),
        };

        public BurstSettings shardBurst = new BurstSettings
        {
            color = Color.white,
            count = 12,
            speedRange = new Vector2(14f, 78f),
            sizeRange = new Vector2(0.18f, 0.62f),
            lifetimeRange = new Vector2(1.0f, 2.0f),
        };

        [Header("Camera Shake")]
        [Min(0f)] public float shakeAmplitude = 0.28f;
        [Header("피격 셰이크의 지속시간. 기존 다른 셰이크의 지속시간은 변경하지 않는다.")]
        [Min(0.01f)] public float shakeDuration = 0.25f;
        // 기존 에셋 값 보존용. 파형은 Cinemachine ImpulseSource에서 설정한다.
        [HideInInspector, Min(1f)] public float shakeFrequency = 30f;

        [Header("Sorting (프로젝트 레이어 구조에 맞게 조정)")]
        public string sortingLayerName = "GA1_HD_FX";
        public int dimSortingOrder = 100;
        public int splatSortingOrder = 110;
        public int crackSortingOrder = 120;
        public int particleSortingOrder = 130;

        /// <summary>정지 해제 이후 연출이 완전히 끝날 때까지 걸리는 시간.</summary>
        public float ReleasePhaseDuration =>
            Mathf.Max(splatReleaseDuration, crackFadeDuration, dimBlendDuration + dimRecoverDuration);
    }

    /// <summary>파티클이 퍼지는 방향 분포.</summary>
    public enum BurstDirection
    {
        /// <summary>360도 원형으로 퍼짐.</summary>
        Radial,
        /// <summary>좌/우 수평 원뿔(±spreadAngle)로만 퍼짐.</summary>
        Horizontal,
    }

    /// <summary>
    /// 파티클 버스트 한 종류의 파라미터.
    /// 속도/크기는 <see cref="PlayerHitEffectSettings.globalScale"/>이 곱해진다.
    /// </summary>
    [Serializable]
    public sealed class BurstSettings
    {
        [Header("파티클 색(텍스처는 흰색 마스크).")]
        public Color color = Color.white;
        [Min(0)] public int count = 100;
        [Header("초기 속도 범위(유닛/초). 강한 drag로 빠르게 감속한다.")]
        public Vector2 speedRange = new Vector2(14f, 78f);
        public Vector2 sizeRange = new Vector2(0.2f, 0.8f);
        public Vector2 lifetimeRange = new Vector2(1f, 2f);
        public BurstDirection direction = BurstDirection.Radial;
        // Horizontal 모드에서 수평선 기준 퍼짐 반각(도).
        [Range(0f, 90f)] public float spreadAngle = 20f;

        // 세로 방향 속도 배율. 원본은 가로로 더 넓게 퍼진다
        [Range(0f, 1f)] public float verticalSquash = 0.75f;

        // 선형 감속 계수. v(t) = v0 · e^(-drag·t)
        [Min(0f)] public float drag = 5f;

        // 발생 위치 랜덤 반경
        [Min(0f)] public float spawnJitter = 0.33f;

        // 초당 회전 각도 범위(±deg)
        [Min(0f)] public float angularSpeed = 90f;

        // 수명 중 이 비율부터 알파가 0으로 감소
        [Range(0f, 1f)] public float fadeStart = 0.6f;
    }
}