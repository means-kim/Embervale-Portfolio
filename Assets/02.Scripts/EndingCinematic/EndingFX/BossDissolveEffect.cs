using UnityEngine;
using UnityEngine.Serialization;
namespace EndingFX
{
    /// <summary>
    /// 보스 소멸 연출.
    /// 1) <see cref="BeginGlow"/>: 보스가 하얗게 달아오르며 몸에서 불티가 피어오르고 떨린다.
    /// 2) <see cref="Dissolve"/>: 현재 보스 모양 그대로 영혼 입자를 한 번에 뿌리고,
    ///    실제 보스 Animator의 사망 프레임마다 입자를 흩날린다.
    ///    애니메이터가 없으면 보스 렌더러를 즉시 숨긴다.
    ///
    /// 프리팹을 보스 SpriteRenderer 오브젝트의 자식(localPosition 0)으로 넣어서 사용한다.
    /// </summary>
    public sealed class BossDissolveEffect : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Glowing,
            Dissolving,
            Dissolved
        }

        [Header("비워두면 부모의 SpriteRenderer 를 사용")]
        [FormerlySerializedAs("bossRenderer")]
        [SerializeField] private SpriteRenderer _bossRenderer;

        [Header("보스 위에 겹쳐 그릴 가산 글로우 스프라이트(머티리얼: Additive)")]
        [FormerlySerializedAs("glowOverlay")]
        [SerializeField] private SpriteRenderer _glowOverlay;

        [Header("달아오르는 동안 보스 몸에서 계속 피어오르는 불티")]
        [FormerlySerializedAs("glowEmbers")]
        [SerializeField] private ParticleSystem _glowEmbers;

        [Header("소멸 순간 보스 모양으로 한 번에 뿌릴 입자 묶음")]
        [FormerlySerializedAs("dissolveBurst")]
        [SerializeField] private ImpactBurst _dissolveBurst;

        [Header("산산조각 애니메이션 프레임마다 조각 모양에서 흩날리는 입자")]
        [FormerlySerializedAs("deathWisps")]
        [SerializeField] private ParticleSystem _deathWisps;
        [FormerlySerializedAs("wispsPerFrame")]
        [SerializeField, Min(0)] private int _wispsPerFrame = 45;

        [Header("Shape 를 보스 스프라이트로 바인딩할 파티클 시스템들")]
        [FormerlySerializedAs("spriteShapedSystems")]
        [SerializeField] private ParticleSystem[] _spriteShapedSystems = new ParticleSystem[0];

        [FormerlySerializedAs("glowRampSeconds")]
        [SerializeField, Min(0.01f)] private float _glowRampSeconds = 1.2f;
        [FormerlySerializedAs("glowOverlayMaxAlpha")]
        [SerializeField, Range(0f, 1f)] private float _glowOverlayMaxAlpha = 0.25f;
        [FormerlySerializedAs("glowEmberRate")]
        [SerializeField, Min(0f)] private float _glowEmberRate = 60f;

        [Header("완전히 달아올랐을 때 보스 떨림 크기(유닛)")]
        [FormerlySerializedAs("glowTremble")]
        [SerializeField, Min(0f)] private float _glowTremble = 0.03f;

        private State _state = State.Idle;
        private float _glow; // 0~1
        private Animator _mecAnimator;
        private Sprite _lastSprite;
        private Vector3 _originalPosition;
        private bool _originalRendererEnabled;
        public bool IsDissolved => _state == State.Dissolved;

        public void BindBoss(SpriteRenderer renderer, Animator animator)
        {
            _bossRenderer = renderer;
            _mecAnimator = animator;
            _originalPosition = renderer.transform.localPosition;
            _originalRendererEnabled = renderer.enabled;
            _lastSprite = renderer.sprite;
            BindSpriteShapes();
        }

        private void Awake()
        {
            if (_bossRenderer == null && transform.parent != null)
            {
                _bossRenderer = transform.parent.GetComponent<SpriteRenderer>();
            }

            if (_bossRenderer == null)
            {
                enabled = false;
                return;
            }

            BindSpriteShapes();
            BindBoss(_bossRenderer, _bossRenderer.GetComponent<Animator>());
            ApplyGlow(0f);
        }

        /// <summary>보스가 달아오르기 시작. 마지막 일격 직후 호출.</summary>
        public void BeginGlow()
        {
            if (_glowOverlay != null)
            {
                _glowOverlay.sprite = _bossRenderer.sprite;
                _glowOverlay.flipX = _bossRenderer.flipX;
                _glowOverlay.enabled = true;
            }

            _state = State.Glowing;
        }

        /// <summary>보스를 입자로 흩어지게 한다.</summary>
        public void Dissolve()
        {
            // 렌더러를 끄거나 프레임을 바꾸기 전에 Emit 해야 "현재" 보스 모양이 Shape 로 사용된다.
            if (_dissolveBurst != null)
            {
                _dissolveBurst.Play(1f);
            }

            ApplyGlow(0f);
            if (_glowOverlay != null)
            {
                _glowOverlay.enabled = false;
            }

            if (_mecAnimator != null)
            {
                _state = State.Dissolving;
                _lastSprite = _bossRenderer.sprite;
            }
            else
            {
                _bossRenderer.enabled = false;
                _state = State.Dissolved;
            }
        }

        /// <summary>연출 전 상태로 되돌린다. (재시작/디버그용)</summary>
        public void ResetEffect()
        {
            _state = State.Idle;
            if (_bossRenderer != null)
            {
                _bossRenderer.enabled = _originalRendererEnabled;
                _bossRenderer.transform.localPosition = _originalPosition;
            }

            ApplyGlow(0f);
            if (_glowOverlay != null)
            {
                _glowOverlay.enabled = false;
            }

            if (_glowEmbers != null)
            {
                _glowEmbers.Clear(false);
            }

            if (_deathWisps != null)
            {
                _deathWisps.Clear(false);
            }

            if (_dissolveBurst != null)
            {
                _dissolveBurst.Clear();
            }
        }

        private void Update()
        {
            if (_state != State.Glowing)
            {
                return;
            }

            ApplyGlow(Mathf.MoveTowards(_glow, 1f, Time.unscaledDeltaTime / _glowRampSeconds));
            _bossRenderer.transform.localPosition =
                _originalPosition + (Vector3)(Random.insideUnitCircle * (_glow * _glowTremble));
        }

        private void LateUpdate()
        {
            if (_bossRenderer == null) return;
            if (_bossRenderer.sprite != _lastSprite)
            {
                _lastSprite = _bossRenderer.sprite;
                HandleBossFrameChanged(_lastSprite);
            }

            if (_state == State.Dissolving && _mecAnimator != null)
            {
                var info = _mecAnimator.GetCurrentAnimatorStateInfo(0);
                if (info.IsName("Death") && info.normalizedTime >= 1 && !_mecAnimator.IsInTransition(0))
                {
                    _bossRenderer.enabled = false;
                    _state = State.Dissolved;
                }
            }
        }

        private void HandleBossFrameChanged(Sprite sprite)
        {
            // 달아오르는 중에 프레임이 바뀌어도 글로우가 같은 모양을 덮도록 동기화
            if (_glowOverlay != null && _glowOverlay.enabled)
            {
                _glowOverlay.sprite = sprite;
            }

            if (_state == State.Dissolving && _deathWisps != null && _wispsPerFrame > 0)
            {
                // Shape 가 보스 렌더러를 참조하므로 방금 바뀐 조각 모양에서 입자가 나온다
                _deathWisps.Emit(_wispsPerFrame);
            }
        }

        private void ApplyGlow(float value)
        {
            _glow = value;
            if (value <= 0 && _bossRenderer != null)
            {
                _bossRenderer.transform.localPosition = _originalPosition;
            }
            if (_glowOverlay != null)
            {
                Color c = _glowOverlay.color;
                c.a = _glow * _glowOverlayMaxAlpha;
                _glowOverlay.color = c;
            }

            if (_glowEmbers != null)
            {
                ParticleSystem.EmissionModule emission = _glowEmbers.emission;
                emission.rateOverTime = _glow * _glowEmberRate;
                if (_glow > 0f && !_glowEmbers.isPlaying)
                {
                    _glowEmbers.Play();
                }
            }
        }

        private void BindSpriteShapes()
        {
            for (int i = 0; i < _spriteShapedSystems.Length; i++)
            {
                if (_spriteShapedSystems[i] == null)
                {
                    continue;
                }

                ParticleSystem.ShapeModule shape = _spriteShapedSystems[i].shape;
                shape.shapeType = ParticleSystemShapeType.SpriteRenderer;
                shape.spriteRenderer = _bossRenderer;
            }
        }
    }
}