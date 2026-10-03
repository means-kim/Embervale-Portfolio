using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Boss.Roar
{
    /// <summary>
    /// 보스 포효 화면 이펙트 (링 + 틴트 + 플래시 + 카메라 흔들림).
    ///
    /// 구성: 게임 화면 위·HUD 아래 Canvas에 화면을 꽉 채운 Image 하나 + BossRoarRings 머티리얼.
    /// 화면을 읽지 않는 오버레이 방식이라 Built-in / URP 어디서나 동작한다.
    ///
    /// 사용:
    ///   roarEffect.Play(bossTransform, 4.8f);   // 보스 Animator의 Animation Event나 상태머신에서 호출
    ///   roarEffect.AddFlash(1.0f);               // 이름 타이틀 등장 순간 등
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class BossRoarScreenEffect : MonoBehaviour
    {
        private static readonly int RingsId = Shader.PropertyToID("_Rings");
        private static readonly int SeedsId = Shader.PropertyToID("_RingSeeds");
        private static readonly int RingCountId = Shader.PropertyToID("_RingCount");
        private static readonly int AspectId = Shader.PropertyToID("_Aspect");
        private static readonly int ThicknessId = Shader.PropertyToID("_Thickness");
        private static readonly int SpikeCountId = Shader.PropertyToID("_SpikeCount");
        private static readonly int SpikeLengthId = Shader.PropertyToID("_SpikeLength");
        private static readonly int BreakupId = Shader.PropertyToID("_Breakup");
        private static readonly int TintAmountId = Shader.PropertyToID("_TintAmount");
        private static readonly int VignetteId = Shader.PropertyToID("_Vignette");
        private static readonly int FlashId = Shader.PropertyToID("_Flash");
        private static readonly int CoreColorId = Shader.PropertyToID("_CoreColor");
        private static readonly int EdgeColorId = Shader.PropertyToID("_EdgeColor");
        private static readonly int TintColorId = Shader.PropertyToID("_TintColor");
        private static readonly int FlashColorId = Shader.PropertyToID("_FlashColor");
        private static readonly int OcclusionId = Shader.PropertyToID("_RingOcclusion");

        [Serializable] public class FloatEvent : UnityEvent<float> { }

        [Header("References")]
        [SerializeField] private BossRoarSettings _settings;
        [Tooltip("Game/BossRoarRings 셰이더를 쓰는 머티리얼 에셋 (빌드에서 셰이더가 빠지지 않도록 에셋으로 참조)")]
        [SerializeField] private Material _ringMaterial;
        [Tooltip("보스 위치를 화면 좌표로 바꿀 카메라 (비우면 Camera.main)")]
        [SerializeField] private Camera _worldCamera;
        [Tooltip("링 중심 오프셋 (보스 기준 월드 좌표, 예: 입 위치)")]
        [SerializeField] private Vector2 _centerOffset = new Vector2(0f, 1.0f);

        [Header("Camera Shake (선택)")]
        [Tooltip("CameraManager에 흔들림을 요청하는 간격(초)")]
        [SerializeField, Min(0.01f)] private float _shakeInterval = 0.1f;

        [Header("Events")]
        [Tooltip("엔벨로프 0~1. URP Volume / PPv2 weight에 연결하면 채도 감소(흑백화)까지 동기화됨")]
        [SerializeField] private FloatEvent _onEnvelope = new FloatEvent();
        [Tooltip("흔들림 진폭 (화면 높이 단위)")]
        [SerializeField] private FloatEvent _onShake = new FloatEvent();
        [SerializeField] private UnityEvent _onRoarFinished = new UnityEvent();

        [SerializeField] private bool _useUnscaledTime;

        private readonly RoarRingSimulator _simulator = new RoarRingSimulator();
        private readonly Vector4[] _ringBuffer = new Vector4[RoarRingSimulator.MaxRings];
        private readonly float[] _seedBuffer = new float[RoarRingSimulator.MaxRings];
        private Image _image;
        private Material _material;
        private Transform _target;
        private float _shakeTimer;
        private bool _wasActive;
        public enum BossPhase
        {
            Phase1,
            Phase2
        }

        public bool IsPlaying => _simulator.IsActive;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.raycastTarget = false;
            _image.enabled = false;

            _settings = _phase1Settings;

            if (_ringMaterial == null || _settings == null)
            {
                enabled = false;
                return;
            }
            _material = new Material(_ringMaterial);
            _image.material = _material;
            if (_worldCamera == null)
            {
                _worldCamera = Camera.main;
            }
            ApplyStaticSettings();
        }

        private void OnDestroy()
        {
            if (_material != null)
            {
                Destroy(_material);
            }
        }

        [SerializeField] private BossRoarSettings _phase1Settings;
        [SerializeField] private BossRoarSettings _phase2Settings;

        public void Play(Transform target, float duration, BossPhase phase)
        {
            BossRoarSettings selectedSettings;

            switch (phase)
            {
                case BossPhase.Phase1:
                    selectedSettings = _phase1Settings;
                    break;

                case BossPhase.Phase2:
                    selectedSettings = _phase2Settings;
                    break;

                default:
                    return;
            }

            if (selectedSettings == null)
            {
                return;
            }

            _settings = selectedSettings;
            _target = target;

            ApplyStaticSettings();
            _simulator.Play(_settings, duration);
            _shakeTimer = 0f;

            _image.enabled = true;
            _wasActive = true;
        }

        public void Stop() => _simulator.Stop();

        public void AddFlash(float amount) => _simulator.AddFlash(amount);

        private void ApplyStaticSettings()
        {
            if (_settings == null || _material == null)
            {
                return;
            }
            _material.SetFloat(ThicknessId, _settings.thickness);
            _material.SetFloat(SpikeCountId, _settings.spikeCount);
            _material.SetFloat(SpikeLengthId, _settings.spikeLength);
            _material.SetFloat(BreakupId, _settings.breakup);
            _material.SetFloat(VignetteId, _settings.vignette);
            _material.SetColor(CoreColorId, _settings.coreColor);
            _material.SetColor(EdgeColorId, _settings.edgeColor);
            _material.SetColor(TintColorId, _settings.tintColor);
            _material.SetColor(FlashColorId, _settings.flashColor);
            _material.SetFloat(OcclusionId, _settings.ringOcclusion);
        }

        private void LateUpdate()
        {
            if (!_wasActive)
            {
                return;
            }
            float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            float aspect = (float)Screen.width / Mathf.Max(1, Screen.height);
            Vector2 center = GetViewportCenter();
            float maxRadius = FarthestCorner(center, aspect) + _settings.thickness + _settings.spikeLength;

            _simulator.Tick(dt, maxRadius);

            int count = _simulator.Fill(center, maxRadius, _ringBuffer, _seedBuffer);
            _material.SetVectorArray(RingsId, _ringBuffer);
            _material.SetFloatArray(SeedsId, _seedBuffer);
            _material.SetFloat(RingCountId, count);
            _material.SetFloat(AspectId, aspect);
            _material.SetFloat(TintAmountId, _settings.tintAmount * _simulator.Envelope);
            _material.SetFloat(FlashId, _simulator.Flash);

            _onEnvelope.Invoke(_simulator.Envelope);
            _onShake.Invoke(_simulator.ShakeAmplitude);
            ApplyShake(_simulator.ShakeAmplitude);

            if (!_simulator.IsActive)
            {
                _wasActive = false;
                _image.enabled = false;
                ApplyShake(0f);
                _onEnvelope.Invoke(0f);
                _onRoarFinished.Invoke();
            }
        }

        private Vector2 GetViewportCenter()
        {
            if (_target == null || _worldCamera == null)
            {
                return new Vector2(0.5f, 0.5f);
            }
            Vector3 world = _target.position + (Vector3)_centerOffset;
            Vector3 vp = _worldCamera.WorldToViewportPoint(world);
            return new Vector2(vp.x, vp.y);
        }

        private static float FarthestCorner(Vector2 c, float aspect)
        {
            float dx = Mathf.Max(c.x, 1f - c.x) * aspect;
            float dy = Mathf.Max(c.y, 1f - c.y);
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        private void ApplyShake(float amplitude)
        {
            if (amplitude <= 0f)
            {
                _shakeTimer = 0f;
                return;
            }

            float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (dt <= 0f || _worldCamera == null)
            {
                return;
            }

            _shakeTimer -= dt;
            if (_shakeTimer > 0f)
            {
                return;
            }

            _shakeTimer = Mathf.Max(0.01f, _shakeInterval);
            float worldPerScreenHeight = _worldCamera.orthographic ? _worldCamera.orthographicSize * 2f : 10f;
            CameraManager.ShakeCamera(amplitude * worldPerScreenHeight);
        }
    }
}
