using System.Collections;
using UnityEngine;

namespace Game.Effects
{
    /// <summary>
    /// 플레이어 피격 연출 오케스트레이터 (할로우 나이트 스타일).
    /// 타임라인:
    ///   [정지 구간] 히트스톱 + 검은 스플랫 등장 + 흰 균열선 깜빡임 + 화면 어둡게
    ///   [해제 순간] 스플랫 확장/소멸, 균열 페이드, 검은 덩어리·흰 파편 방사 폭발, 카메라 셰이크
    ///   [여운]      화면 밝기 서서히 복구
    /// 정지 구간에서도 진행돼야 하므로 모든 시간 계산은 unscaled time 기준이다.
    /// </summary>
    /// <remarks>
    /// 이 오브젝트는 씬 루트에 둔다(플레이어 자식 X). 플레이어의 Flip(scale -1)이나 이동에 영향받지 않게 하기 위함.
    /// </remarks>
    public sealed class PlayerHitEffect : MonoBehaviour
    {
        [SerializeField] private PlayerHitEffectSettings settings;

        [Header("Visual Parts (빌더가 자동 연결)")]
        [SerializeField] private SpriteRenderer splatRenderer;
        [SerializeField] private SpriteRenderer crackRenderer;
        [SerializeField] private SpriteRenderer screenDimRenderer;
        [SerializeField] private RadialBurstEmitter shadeEmitter;
        [SerializeField] private RadialBurstEmitter shardEmitter;

        [Header("Scene References (선택)")]
        [Tooltip("비워두면 Camera.main 사용.")]
        [SerializeField] private Camera targetCamera;
        [Tooltip("비워두면 히트스톱 없이 연출만 재생.")]
        [SerializeField] private HitStopController hitStop;
        [Tooltip("비워두면 카메라 셰이크 생략.")]
        [SerializeField] private CameraShaker cameraShaker;

        private Coroutine _routine;
        private Vector3 _splatBaseScale;
        private Vector3 _crackBaseScale;

        /// <summary>현재 연출이 재생 중인지 여부.</summary>
        public bool IsPlaying => _routine != null;

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            ApplySettings();
            AttachScreenDimToCamera();
            HideAll();
        }

        private void OnDisable()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            HideAll();
        }

        private void OnDestroy()
        {
            // Awake에서 카메라 밑으로 옮겼으므로 직접 정리.
            if (screenDimRenderer != null && screenDimRenderer.transform.parent != transform)
            {
                Destroy(screenDimRenderer.gameObject);
            }
        }

        /// <summary>
        /// 피격 연출을 재생한다. 재생 중이면 처음부터 다시 시작한다.
        /// </summary>
        /// <param name="worldPosition">피격 지점(보통 플레이어 몸 중심).</param>
        public void Play(Vector3 worldPosition)
        {
            if (settings == null)
            {
                Debug.LogError($"[{nameof(PlayerHitEffect)}] Settings가 비어 있음.", this);
                return;
            }

            if (_routine != null)
            {
                StopCoroutine(_routine);
            }

            _routine = StartCoroutine(PlayRoutine(worldPosition));
        }

        /// <summary>Play 모드에서 Inspector 우클릭 메뉴로 바로 확인하기 위한 테스트용.</summary>
        [ContextMenu("Play Test (Play Mode)")]
        private void PlayTest()
        {
            if (Application.isPlaying)
            {
                ApplySettings(); // Settings 수정사항 즉시 반영
                Play(transform.position);
            }
        }

        /// <summary>
        /// Settings 값을 렌더러 크기/정렬과 파티클 모듈에 반영한다.
        /// 런타임에 Settings를 바꿨다면 호출해서 다시 적용할 수 있다.
        /// </summary>
        public void ApplySettings()
        {
            if (settings == null)
            {
                return;
            }

            float scale = settings.globalScale;
            _splatBaseScale = ScaleForWorldSize(splatRenderer, settings.splatSize * scale);
            _crackBaseScale = ScaleForWorldSize(crackRenderer, settings.crackSize * scale);

            SetSorting(splatRenderer, settings.splatSortingOrder);
            SetSorting(crackRenderer, settings.crackSortingOrder);
            SetSorting(screenDimRenderer, settings.dimSortingOrder);

            shadeEmitter.Configure(settings.shadeBurst, settings.sortingLayerName, settings.particleSortingOrder);
            shardEmitter.Configure(settings.shardBurst, settings.sortingLayerName, settings.particleSortingOrder);
        }

        private IEnumerator PlayRoutine(Vector3 hitPosition)
        {
            PlayerHitEffectSettings s = settings;
            float scale = s.globalScale;

            Vector3 splatPosition = hitPosition + (Vector3)(s.splatOffset * scale);
            transform.position = splatPosition;
            crackRenderer.transform.localPosition = s.crackOffset * scale;
            crackRenderer.flipX = Random.value < 0.5f; // 매번 같은 모양으로 보이지 않게
            FitScreenDimToCamera();

            if (hitStop != null)
            {
                hitStop.Request(s.freezeDuration, s.freezeTimeScale);
            }

            splatRenderer.enabled = true;
            crackRenderer.enabled = true;
            screenDimRenderer.enabled = true;

            // ---- 1. 정지 구간 ------------------------------------------------
            for (float t = 0f; t < s.freezeDuration; t += Time.unscaledDeltaTime)
            {
                float pop = s.splatPopDuration > 0f ? Mathf.Clamp01(t / s.splatPopDuration) : 1f;
                splatRenderer.transform.localScale =
                    _splatBaseScale * Mathf.Lerp(s.splatPopStartScale, 1f, pop);
                SetTint(splatRenderer, s.splatColor, 1f);

                bool flickerLow = Mathf.Repeat(t, s.crackFlickerInterval * 2f) >= s.crackFlickerInterval;
                SetTint(crackRenderer, s.crackColor, flickerLow ? s.crackFlickerMinAlpha : 1f);
                crackRenderer.transform.localScale = _crackBaseScale;

                SetTint(screenDimRenderer, s.dimColor, s.dimDuringFreeze);
                yield return null;
            }

            // ---- 2. 해제 순간 ------------------------------------------------
            shadeEmitter.Emit(splatPosition, s.shadeBurst, scale);
            shardEmitter.Emit(splatPosition, s.shardBurst, scale);

            if (cameraShaker != null)
            {
                cameraShaker.Shake(s.shakeAmplitude * scale, s.shakeDuration, s.shakeFrequency);
            }

            // ---- 3. 해제 후 여운 ---------------------------------------------
            float releaseDuration = s.ReleasePhaseDuration;
            for (float t = 0f; t < releaseDuration; t += Time.unscaledDeltaTime)
            {
                UpdateSplatRelease(s, t);
                UpdateCrackRelease(s, t);
                SetTint(screenDimRenderer, s.dimColor, EvaluateDimAfterRelease(s, t));
                yield return null;
            }

            HideAll();
            _routine = null;
        }

        private void UpdateSplatRelease(PlayerHitEffectSettings s, float t)
        {
            float k = Mathf.Clamp01(t / s.splatReleaseDuration);
            float easeOut = 1f - (1f - k) * (1f - k);
            splatRenderer.transform.localScale = _splatBaseScale * Mathf.Lerp(1f, s.splatReleaseScale, easeOut);
            SetTint(splatRenderer, s.splatColor, 1f - k);
        }

        private void UpdateCrackRelease(PlayerHitEffectSettings s, float t)
        {
            float k = Mathf.Clamp01(t / s.crackFadeDuration);
            Vector3 crackScale = _crackBaseScale;
            crackScale.x *= 1f - s.crackShrinkOnFade * k;
            crackRenderer.transform.localScale = crackScale;
            SetTint(crackRenderer, s.crackColor, 1f - k);
        }

        private static float EvaluateDimAfterRelease(PlayerHitEffectSettings s, float t)
        {
            if (t < s.dimBlendDuration)
            {
                return Mathf.Lerp(s.dimDuringFreeze, s.dimAfterRelease, t / s.dimBlendDuration);
            }

            float k = Mathf.Clamp01((t - s.dimBlendDuration) / s.dimRecoverDuration);
            return Mathf.Lerp(s.dimAfterRelease, 0f, k);
        }

        private void AttachScreenDimToCamera()
        {
            if (targetCamera == null || screenDimRenderer == null)
            {
                return;
            }

            // 카메라를 따라다녀야 하므로 카메라 자식으로 옮긴다.
            Transform dim = screenDimRenderer.transform;
            dim.SetParent(targetCamera.transform, false);
            dim.localRotation = Quaternion.identity;
            dim.localPosition = new Vector3(0f, 0f, targetCamera.nearClipPlane + 0.05f);
        }

        /// <summary>카메라 시야 전체(+셰이크 여유)를 덮도록 딤 스프라이트 크기를 맞춘다.</summary>
        private void FitScreenDimToCamera()
        {
            if (targetCamera == null || screenDimRenderer == null || screenDimRenderer.sprite == null)
            {
                return;
            }

            const float margin = 1.15f;
            float distance = screenDimRenderer.transform.localPosition.z;
            float viewHeight = targetCamera.orthographic
                ? targetCamera.orthographicSize * 2f
                : 2f * distance * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float viewWidth = viewHeight * targetCamera.aspect;

            Vector2 spriteSize = screenDimRenderer.sprite.bounds.size;
            screenDimRenderer.transform.localScale = new Vector3(
                viewWidth * margin / spriteSize.x,
                viewHeight * margin / spriteSize.y,
                1f);
        }

        private void HideAll()
        {
            if (splatRenderer != null) splatRenderer.enabled = false;
            if (crackRenderer != null) crackRenderer.enabled = false;
            if (screenDimRenderer != null) screenDimRenderer.enabled = false;
        }

        private void SetSorting(Renderer target, int order)
        {
            if (target == null)
            {
                return;
            }

            target.sortingLayerName = settings.sortingLayerName;
            target.sortingOrder = order;
        }

        private static Vector3 ScaleForWorldSize(SpriteRenderer target, Vector2 worldSize)
        {
            if (target == null || target.sprite == null)
            {
                return Vector3.one;
            }

            Vector2 spriteSize = target.sprite.bounds.size;
            return new Vector3(worldSize.x / spriteSize.x, worldSize.y / spriteSize.y, 1f);
        }

        /// <summary>Settings 색상에 알파 배율을 곱해 적용한다. (Color는 struct라 할당 없음)</summary>
        private static void SetTint(SpriteRenderer target, Color baseColor, float alpha)
        {
            baseColor.a *= alpha;
            target.color = baseColor;
        }
    }
}
