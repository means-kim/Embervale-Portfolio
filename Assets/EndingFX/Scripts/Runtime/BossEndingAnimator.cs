using System;
using UnityEngine;

namespace EndingFX
{
    /// <summary>
    /// 엔딩 구간 보스 스프라이트 제어.
    /// - Intro : 포효 시트(16f)를 1회 재생 → 마지막 프레임이 Damage 0번과 같아서 자연스럽게 이어진다.
    ///           포효 구간(클립의 Slow Section)은 느리게 재생되고, 그 시작부터 공격이 들어온다.
    ///           포효 중 피격은 동작을 끊지 않고 번쩍임/흔들림만 주며, 누적 피해는 포효가 끝날 때 반영된다.
    /// - Damage: 재생하지 않고 "누적 피해 진행도"로 프레임을 고른다. 때릴수록 균열이 커짐
    /// - Death : 산산조각 시트(16f)를 1회 재생 후 렌더러를 끈다
    /// 피격 시 하얀 플래시 + 흔들림, 달아오르는 동안 떨림(tremble)도 담당한다.
    ///
    /// Animator(Mecanim) 대신 직접 제어하는 이유: Damage 는 시간이 아니라 진행도로 프레임을 고르고,
    /// Death 재생 중 프레임이 바뀔 때마다 파티클을 뿌려야 해서 이벤트를 코드에서 받는 쪽이 단순하다.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class BossEndingAnimator : MonoBehaviour
    {
        [Header("Clips")]
        [SerializeField] private FlipbookClip introClip;
        [SerializeField] private FlipbookClip damageClip;
        [SerializeField] private FlipbookClip deathClip;

        [Header("Hit Reaction")]
        [Tooltip("보스 위에 겹치는 가산 스프라이트. 피격 순간 번쩍임")]
        [SerializeField] private SpriteRenderer hitFlash;
        [SerializeField, Min(0.01f)] private float hitFlashSeconds = 0.12f;
        [SerializeField, Min(0f)] private float hitShake = 0.08f;
        [SerializeField, Min(0f)] private float shakeRecoverPerSecond = 0.8f;

        private SpriteRenderer spriteRenderer;
        private Vector3 basePosition;
        private FlipbookClip playingClip;
        private Action onPlayComplete;
        private float playTime;     // 현재 프레임 안에서 흐른 시간
        private int playFrame;
        private float damageProgress;
        private float flash;
        private float shake;
        private float tremble;

        /// <summary>표시 스프라이트가 바뀔 때마다 호출. (Death 재생 중 파티클 연동용)</summary>
        public event Action<Sprite> FrameChanged;

        /// <summary>Death 재생이 끝나고 보스가 사라졌을 때 호출.</summary>
        public event Action DeathFinished;

        /// <summary>인트로 전체 길이(초). 슬로우 구간 포함.</summary>
        public float IntroDuration => introClip == null ? 0f : introClip.TotalDuration;

        /// <summary>
        /// 인트로 시작부터 포효 순간(슬로우 구간 시작)까지의 시간(초).
        /// 이 시점부터 공격을 시작한다. 슬로우 구간이 없으면 인트로가 끝난 뒤.
        /// </summary>
        public float IntroAttackStartTime =>
            introClip == null ? 0f : introClip.HasSlowSection ? introClip.SlowSectionStartTime : introClip.TotalDuration;

        /// <summary>사망 애니메이션 길이(초).</summary>
        public float DeathDuration => deathClip == null ? 0f : deathClip.TotalDuration;

        /// <summary>인트로(포효) 재생 중인지.</summary>
        public bool IsPlayingIntro => playingClip != null && playingClip == introClip;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            basePosition = transform.localPosition;
        }

        /// <summary>대기 상태(피해 0)로 되돌린다.</summary>
        public void ResetToIdle()
        {
            playingClip = null;
            onPlayComplete = null;
            flash = shake = tremble = damageProgress = 0f;
            spriteRenderer.enabled = true;
            transform.localPosition = basePosition;
            ShowFrame(damageClip, 0);
            ApplyFlash();
        }

        /// <summary>포효 인트로 재생. 끝나면 그때까지 누적된 피해에 맞는 균열 프레임으로 넘어간다.</summary>
        public void PlayIntro()
        {
            PlayOnce(introClip, ShowDamageFrame);
        }

        /// <summary>
        /// 피격 반응. 번쩍임 + 흔들림, 그리고 진행도에 맞는 균열 프레임으로 교체(포효 중이면 포효 종료 시 반영).
        /// </summary>
        /// <param name="progress">0~1 누적 피해 진행도.</param>
        public void ApplyHit(float progress)
        {
            damageProgress = Mathf.Clamp01(progress);
            flash = 1f;
            shake = hitShake;

            // 포효 중에는 동작을 끊지 않는다. 균열은 포효가 끝날 때 ShowDamageFrame 에서 반영
            if (!IsPlayingIntro)
            {
                ShowDamageFrame();
            }
        }

        /// <summary>달아오르는 동안 지속되는 떨림 크기(유닛).</summary>
        public void SetTremble(float amount) => tremble = amount;

        /// <summary>산산조각 재생. 끝나면 렌더러를 끈다.</summary>
        public void PlayDeath()
        {
            tremble = 0f;
            flash = 0f;
            ApplyFlash();
            PlayOnce(deathClip, () =>
            {
                spriteRenderer.enabled = false;
                DeathFinished?.Invoke();
            });
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            TickPlayback(dt);
            TickReaction(dt);
        }

        private void TickPlayback(float dt)
        {
            if (playingClip == null)
            {
                return;
            }

            playTime += dt;

            // 프레임마다 길이가 다를 수 있으므로(슬로우 구간) 현재 프레임 길이를 넘을 때마다 한 칸씩 전진
            bool changed = false;
            while (playTime >= playingClip.GetFrameDuration(playFrame))
            {
                playTime -= playingClip.GetFrameDuration(playFrame);
                playFrame++;
                changed = true;

                if (playFrame >= playingClip.FrameCount)
                {
                    playingClip = null;
                    Action complete = onPlayComplete;
                    onPlayComplete = null;
                    complete?.Invoke();
                    return;
                }
            }

            if (changed)
            {
                ShowFrame(playingClip, playFrame);
            }
        }

        private void ShowDamageFrame()
        {
            int last = damageClip.FrameCount - 1;
            ShowFrame(damageClip, Mathf.RoundToInt(damageProgress * last));
        }

        private void TickReaction(float dt)
        {
            if (flash > 0f)
            {
                flash = Mathf.Max(0f, flash - dt / hitFlashSeconds);
                ApplyFlash();
            }

            shake = Mathf.MoveTowards(shake, 0f, shakeRecoverPerSecond * dt);
            float amplitude = shake + tremble;
            transform.localPosition = amplitude > 0f
                ? basePosition + (Vector3)(UnityEngine.Random.insideUnitCircle * amplitude)
                : basePosition;
        }

        private void PlayOnce(FlipbookClip clip, Action onComplete)
        {
            if (clip == null || clip.FrameCount == 0)
            {
                onComplete?.Invoke();
                return;
            }

            playingClip = clip;
            onPlayComplete = onComplete;
            playTime = 0f;
            playFrame = 0;
            ShowFrame(clip, 0);
        }

        private void ShowFrame(FlipbookClip clip, int index)
        {
            if (clip == null || clip.FrameCount == 0)
            {
                return;
            }

            Sprite sprite = clip.GetFrame(index);
            spriteRenderer.sprite = sprite;
            if (hitFlash != null)
            {
                hitFlash.sprite = sprite;
            }

            FrameChanged?.Invoke(sprite);
        }

        private void ApplyFlash()
        {
            if (hitFlash == null)
            {
                return;
            }

            hitFlash.enabled = flash > 0f;
            Color c = hitFlash.color;
            c.a = flash;
            hitFlash.color = c;
        }

#if UNITY_EDITOR
        /// <summary>에디터 빌더에서 참조를 채울 때 사용.</summary>
        public void EditorSetup(FlipbookClip intro, FlipbookClip damage, FlipbookClip death, SpriteRenderer flashRenderer)
        {
            introClip = intro;
            damageClip = damage;
            deathClip = death;
            hitFlash = flashRenderer;
        }
#endif
    }
}
