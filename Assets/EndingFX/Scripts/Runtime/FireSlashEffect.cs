using System;
using UnityEngine;

namespace EndingFX
{
    /// <summary>
    /// 불 슬래시 플립북 재생기.
    /// - 피크 프레임에서 잠깐 멈추는 홀드(타격감)
    /// - 스케일 펀치(작게 시작 → 살짝 커졌다 → 원래 크기)
    /// - 피크 순간 슬래시 모양 그대로 불티를 흩뿌림(선택)
    /// Animator 대신 직접 재생하는 이유: 프레임 7장짜리 단발 이펙트라 상태머신이 필요 없고,
    /// 각도/반전/홀드 시간을 호출마다 바꿔야 해서 코드 제어가 더 단순하다.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class FireSlashEffect : MonoBehaviour
    {
        [SerializeField] private FlipbookClip clip;

        [Tooltip("피크 프레임을 추가로 유지하는 시간(초). 타격 정지감")]
        [SerializeField, Min(0f)] private float peakHoldSeconds = 0.05f;

        [Tooltip("정규화 시간(0~1) → 스케일 배율")]
        [SerializeField] private AnimationCurve scaleOverTime = new AnimationCurve(
            new Keyframe(0f, 0.8f), new Keyframe(0.3f, 1.08f), new Keyframe(0.55f, 1f), new Keyframe(1f, 1.03f));

        [Tooltip("피크 순간 슬래시 모양에서 불티를 뿌릴 파티클(Shape = SpriteRenderer). 비워두면 생략")]
        [SerializeField] private ParticleSystem emberShed;
        [SerializeField, Min(0)] private int emberShedCount = 24;

        [Tooltip("Time.timeScale 을 0으로 만드는 히트스톱 중에도 재생하려면 체크")]
        [SerializeField] private bool useUnscaledTime;

        private SpriteRenderer spriteRenderer;
        private float elapsed;
        private float baseScale = 1f;
        private int currentFrame = -1;
        private bool peaked;

        /// <summary>피크 프레임에 도달했을 때(타격 순간) 호출.</summary>
        public event Action<FireSlashEffect> Peaked;

        /// <summary>재생이 끝났을 때 호출. 풀 반환 용도.</summary>
        public event Action<FireSlashEffect> Finished;

        /// <summary>현재 재생 중인 클립.</summary>
        public FlipbookClip Clip => clip;

        private float TotalDuration => clip.FrameCount * clip.FrameDuration + peakHoldSeconds;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// 슬래시를 재생한다.
        /// </summary>
        /// <param name="position">월드 위치(슬래시 중심).</param>
        /// <param name="angleDegrees">Z축 회전. 매 타격마다 바꿔주면 연속 베기처럼 보인다.</param>
        /// <param name="flipX">좌우 반전(역방향 베기).</param>
        /// <param name="scale">기본 크기 배율.</param>
        public void Play(Vector3 position, float angleDegrees, bool flipX, float scale = 1f)
        {
            if (clip == null || clip.FrameCount == 0)
            {
                Debug.LogWarning($"[{nameof(FireSlashEffect)}] FlipbookClip 이 비어있음", this);
                return;
            }

            transform.SetPositionAndRotation(position, Quaternion.Euler(0f, 0f, angleDegrees));
            spriteRenderer.flipX = flipX;
            spriteRenderer.enabled = true;
            baseScale = scale;
            elapsed = 0f;
            currentFrame = -1;
            peaked = false;

            gameObject.SetActive(true);
            enabled = true;
            Tick(0f);
        }

        private void Update()
        {
            Tick(useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
        }

        private void Tick(float deltaTime)
        {
            elapsed += deltaTime;

            float total = TotalDuration;
            if (elapsed >= total)
            {
                // 슬래시는 끝났지만 흩뿌린 불티(자식 파티클)는 아직 날아가는 중일 수 있다.
                // 오브젝트를 바로 끄면 불티가 같이 사라지므로 스프라이트만 숨기고 기다린다.
                spriteRenderer.enabled = false;
                if (emberShed != null && emberShed.particleCount > 0)
                {
                    return;
                }

                enabled = false;
                gameObject.SetActive(false);
                Finished?.Invoke(this);
                return;
            }

            ApplyFrame(EvaluateFrameIndex(elapsed));

            float s = baseScale * scaleOverTime.Evaluate(elapsed / total);
            transform.localScale = new Vector3(s, s, 1f);
        }

        /// <summary>피크 홀드 구간을 반영해서 경과 시간 → 프레임 인덱스를 계산.</summary>
        private int EvaluateFrameIndex(float time)
        {
            float frameDuration = clip.FrameDuration;
            int peak = clip.PeakFrameIndex;
            float peakEnd = (peak + 1) * frameDuration + peakHoldSeconds;

            int index = time < peakEnd
                ? Mathf.Min(peak, (int)(time / frameDuration))
                : peak + 1 + (int)((time - peakEnd) / frameDuration);

            return Mathf.Min(index, clip.FrameCount - 1);
        }

        private void ApplyFrame(int index)
        {
            if (index == currentFrame)
            {
                return; // 같은 프레임이면 스프라이트 재할당 생략
            }

            currentFrame = index;
            spriteRenderer.sprite = clip.GetFrame(index);

            if (!peaked && index >= clip.PeakFrameIndex)
            {
                peaked = true;
                ShedEmbers();
                Peaked?.Invoke(this);
            }
        }

        private void ShedEmbers()
        {
            if (emberShed == null || emberShedCount <= 0)
            {
                return;
            }

            // Shape 가 이 SpriteRenderer 를 참조하므로 "지금 프레임"의 불꽃 모양에서 불티가 나온다.
            emberShed.Emit(emberShedCount);
        }
    }
}
