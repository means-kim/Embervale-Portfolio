using UnityEngine;

namespace EndingFX
{
    /// <summary>
    /// 보스 소멸 연출.
    /// 1) <see cref="BeginGlow"/>: 보스가 하얗게 달아오르며 몸에서 불티가 피어오르고 떨린다. (영상 8.5~13.5초 구간)
    /// 2) <see cref="Dissolve"/>: 현재 보스 모양 그대로 영혼 입자를 한 번에 뿌리고,
    ///    <see cref="BossEndingAnimator"/> 가 있으면 산산조각 애니메이션을 재생하면서 프레임마다 조각 모양에서 입자를 흩날린다.
    ///    애니메이터가 없으면 원본 영상처럼 하드컷으로 보스를 지운다.
    ///
    /// 프리팹을 보스 SpriteRenderer 오브젝트의 자식(localPosition 0)으로 넣어서 사용한다.
    /// </summary>
    public sealed class BossDissolveEffect : MonoBehaviour
    {
        private enum State { Idle, Glowing, Dissolving, Dissolved }

        [Tooltip("비워두면 부모의 SpriteRenderer 를 사용")]
        [SerializeField] private SpriteRenderer bossRenderer;

        [Tooltip("비워두면 부모의 BossEndingAnimator 를 사용. 없으면 하드컷 소멸")]
        [SerializeField] private BossEndingAnimator bossAnimator;

        [Tooltip("보스 위에 겹쳐 그릴 가산 글로우 스프라이트(머티리얼: Additive)")]
        [SerializeField] private SpriteRenderer glowOverlay;

        [Tooltip("달아오르는 동안 보스 몸에서 계속 피어오르는 불티")]
        [SerializeField] private ParticleSystem glowEmbers;

        [Tooltip("소멸 순간 보스 모양으로 한 번에 뿌릴 입자 묶음")]
        [SerializeField] private ImpactBurst dissolveBurst;

        [Tooltip("산산조각 애니메이션 프레임마다 조각 모양에서 흩날리는 입자")]
        [SerializeField] private ParticleSystem deathWisps;
        [SerializeField, Min(0)] private int wispsPerFrame = 45;

        [Tooltip("Shape 를 보스 스프라이트로 바인딩할 파티클 시스템들")]
        [SerializeField] private ParticleSystem[] spriteShapedSystems = new ParticleSystem[0];

        [SerializeField, Min(0.01f)] private float glowRampSeconds = 1.2f;
        [SerializeField, Range(0f, 1f)] private float glowOverlayMaxAlpha = 0.25f;
        [SerializeField, Min(0f)] private float glowEmberRate = 60f;

        [Tooltip("완전히 달아올랐을 때 보스 떨림 크기(유닛)")]
        [SerializeField, Min(0f)] private float glowTremble = 0.03f;

        private State state = State.Idle;
        private float glow;   // 0~1

        private void Awake()
        {
            if (bossRenderer == null && transform.parent != null)
            {
                bossRenderer = transform.parent.GetComponent<SpriteRenderer>();
            }

            if (bossAnimator == null && transform.parent != null)
            {
                bossAnimator = transform.parent.GetComponent<BossEndingAnimator>();
            }

            if (bossRenderer == null)
            {
                Debug.LogError($"[{nameof(BossDissolveEffect)}] 보스 SpriteRenderer 를 찾을 수 없음", this);
                enabled = false;
                return;
            }

            BindSpriteShapes();
            ApplyGlow(0f);
        }

        private void OnEnable()
        {
            if (bossAnimator != null)
            {
                bossAnimator.FrameChanged += HandleBossFrameChanged;
                bossAnimator.DeathFinished += HandleDeathFinished;
            }
        }

        private void OnDisable()
        {
            if (bossAnimator != null)
            {
                bossAnimator.FrameChanged -= HandleBossFrameChanged;
                bossAnimator.DeathFinished -= HandleDeathFinished;
            }
        }

        /// <summary>보스가 달아오르기 시작. 마지막 일격 직후 호출.</summary>
        public void BeginGlow()
        {
            if (glowOverlay != null)
            {
                glowOverlay.sprite = bossRenderer.sprite;
                glowOverlay.flipX = bossRenderer.flipX;
                glowOverlay.enabled = true;
            }

            state = State.Glowing;
        }

        /// <summary>보스를 입자로 흩어지게 한다.</summary>
        public void Dissolve()
        {
            // 렌더러를 끄거나 프레임을 바꾸기 전에 Emit 해야 "현재" 보스 모양이 Shape 로 사용된다.
            if (dissolveBurst != null)
            {
                dissolveBurst.Play(1f);
            }

            ApplyGlow(0f);
            if (glowOverlay != null)
            {
                glowOverlay.enabled = false;
            }

            if (bossAnimator != null)
            {
                state = State.Dissolving;
                bossAnimator.PlayDeath();
            }
            else
            {
                bossRenderer.enabled = false;
                state = State.Dissolved;
            }
        }

        /// <summary>연출 전 상태로 되돌린다. (재시작/디버그용)</summary>
        public void ResetEffect()
        {
            state = State.Idle;
            bossRenderer.enabled = true;
            ApplyGlow(0f);
            if (glowOverlay != null)
            {
                glowOverlay.enabled = false;
            }

            if (glowEmbers != null)
            {
                glowEmbers.Clear(false);
            }

            if (deathWisps != null)
            {
                deathWisps.Clear(false);
            }

            if (dissolveBurst != null)
            {
                dissolveBurst.Clear();
            }
        }

        private void Update()
        {
            if (state != State.Glowing || glow >= 1f)
            {
                return;
            }

            ApplyGlow(Mathf.MoveTowards(glow, 1f, Time.deltaTime / glowRampSeconds));
        }

        private void HandleBossFrameChanged(Sprite sprite)
        {
            // 달아오르는 중에 프레임이 바뀌어도 글로우가 같은 모양을 덮도록 동기화
            if (glowOverlay != null && glowOverlay.enabled)
            {
                glowOverlay.sprite = sprite;
            }

            if (state == State.Dissolving && deathWisps != null && wispsPerFrame > 0)
            {
                // Shape 가 보스 렌더러를 참조하므로 방금 바뀐 조각 모양에서 입자가 나온다
                deathWisps.Emit(wispsPerFrame);
            }
        }

        private void HandleDeathFinished()
        {
            state = State.Dissolved;
        }

        private void ApplyGlow(float value)
        {
            glow = value;

            if (glowOverlay != null)
            {
                Color c = glowOverlay.color;
                c.a = glow * glowOverlayMaxAlpha;
                glowOverlay.color = c;
            }

            if (glowEmbers != null)
            {
                ParticleSystem.EmissionModule emission = glowEmbers.emission;
                emission.rateOverTime = glow * glowEmberRate;
                if (glow > 0f && !glowEmbers.isPlaying)
                {
                    glowEmbers.Play();
                }
            }

            if (bossAnimator != null)
            {
                bossAnimator.SetTremble(glow * glowTremble);
            }
        }

        private void BindSpriteShapes()
        {
            for (int i = 0; i < spriteShapedSystems.Length; i++)
            {
                if (spriteShapedSystems[i] == null)
                {
                    continue;
                }

                ParticleSystem.ShapeModule shape = spriteShapedSystems[i].shape;
                shape.shapeType = ParticleSystemShapeType.SpriteRenderer;
                shape.spriteRenderer = bossRenderer;
            }
        }

#if UNITY_EDITOR
        /// <summary>에디터 빌더에서 참조를 채울 때 사용.</summary>
        public void EditorSetup(SpriteRenderer overlay, ParticleSystem embers, ImpactBurst burst,
            ParticleSystem wisps, ParticleSystem[] shaped)
        {
            glowOverlay = overlay;
            glowEmbers = embers;
            dissolveBurst = burst;
            deathWisps = wisps;
            spriteShapedSystems = shaped;
        }
#endif
    }
}
