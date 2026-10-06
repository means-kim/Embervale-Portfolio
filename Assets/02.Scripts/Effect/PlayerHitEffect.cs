using System.Collections;
using UnityEngine;

namespace Game.Effects
{
    /// 플레이어 피격 연출 오케스트레이터
    /// 타임라인:
    ///   [정지 구간] 히트스톱 + 검은 스플랫 등장 + 흰 균열선 깜빡임 + 화면 어둡게
    ///   [해제 순간] 스플랫 확장/소멸, 균열 페이드, 검은 덩어리·흰 파편 방사 폭발, 카메라 셰이크
    ///   [여운]      화면 밝기 서서히 복구
    /// 정지 구간에서도 진행돼야 하므로 모든 시간 계산은 unscaled time 기준이다.
    /// 이 오브젝트는 씬 루트에 둔다(플레이어 자식 X). 플레이어의 Flip(scale -1)이나 이동에 영향받지 않게 하기 위함.
    public sealed class PlayerHitEffect : MonoBehaviour
    {
        [SerializeField] private PlayerHitEffectSettings _settings;

        [Header("Visual Parts (빌더가 자동 연결)")]
        [SerializeField] private SpriteRenderer _splatRenderer;
        [SerializeField] private SpriteRenderer _crackRenderer;
        [SerializeField] private SpriteRenderer _screenDimRenderer;
        [SerializeField] private RadialBurstEmitter _shadeEmitter;
        [SerializeField] private RadialBurstEmitter _shardEmitter;

        [Header("Scene References (선택)")]
        [SerializeField] private Camera _targetCamera;

        private Coroutine _routine;
        private Vector3 _splatBaseScale;
        private Vector3 _crackBaseScale;

        // 현재 연출이 재생 중인지 여부
        public bool IsPlaying => _routine != null;

        private void Awake()
        {
            if (_targetCamera == null)
            {
                _targetCamera = Camera.main;
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
            if (_screenDimRenderer != null && _screenDimRenderer.transform.parent != transform)
            {
                Destroy(_screenDimRenderer.gameObject);
            }
        }

        /// 피격 연출을 재생. 재생 중이면 처음부터 다시 시작
        /// <param name="worldPosition">피격 지점(보통 플레이어 몸 중심).</param>
        public void Play(Vector3 worldPosition, System.Action onReleased = null)
        {
            if (_settings == null)
            {
                Debug.LogError("PlayerHitEffect의 Settings가 연결되지 않았습니다.", this);
                onReleased?.Invoke();
                return;
            }

            if (_routine != null)
            {
                StopCoroutine(_routine);
            }

            _routine = StartCoroutine(PlayRoutine(worldPosition, onReleased));
        }

        /// Settings 값을 렌더러 크기/정렬과 파티클 모듈에 반영한다.
        /// 런타임에 Settings를 바꿨다면 호출해서 다시 적용할 수 있다.
        public void ApplySettings()
        {
            if (_settings == null)
            {
                return;
            }

            float scale = _settings.globalScale;
            if (_settings.splatSprite != null)
            {
                _splatRenderer.sprite = _settings.splatSprite;
            }

            _splatBaseScale = ScaleForWorldSize(_splatRenderer, _settings.splatSize * scale);
            _crackBaseScale = ScaleForWorldSize(_crackRenderer, _settings.crackSize * scale);

            SetSorting(_splatRenderer, _settings.splatSortingOrder);
            SetSorting(_crackRenderer, _settings.crackSortingOrder);
            SetSorting(_screenDimRenderer, _settings.dimSortingOrder);

            _shadeEmitter.Configure(_settings.shadeBurst, _settings.sortingLayerName, _settings.particleSortingOrder);
            _shardEmitter.Configure(_settings.shardBurst, _settings.sortingLayerName, _settings.particleSortingOrder);
        }

        private IEnumerator PlayRoutine(Vector3 hitPosition, System.Action onReleased)
        {
            PlayerHitEffectSettings s = _settings;
            float scale = s.globalScale;

            Vector3 splatPosition = hitPosition + (Vector3)(s.splatOffset * scale);
            transform.position = splatPosition;
            _crackRenderer.transform.localPosition = s.crackOffset * scale;
            _crackRenderer.flipX = Random.value < 0.5f; // 매번 같은 모양으로 보이지 않게
            FitScreenDimToCamera();

            HitImpactFeedback.EnsureInstance().RequestHitstop(s.freezeDuration, s.freezeTimeScale);

            _splatRenderer.enabled = true;
            _crackRenderer.enabled = true;
            _screenDimRenderer.enabled = true;

            // ---- 1. 정지 구간 ------------------------------------------------
            for (float t = 0f; t < s.freezeDuration; t += Time.unscaledDeltaTime)
            {
                float pop = s.splatPopDuration > 0f ? Mathf.Clamp01(t / s.splatPopDuration) : 1f;
                _splatRenderer.transform.localScale =
                    _splatBaseScale * Mathf.Lerp(s.splatPopStartScale, 1f, pop);
                SetTint(_splatRenderer, s.splatColor, 1f);

                bool flickerLow = Mathf.Repeat(t, s.crackFlickerInterval * 2f) >= s.crackFlickerInterval;
                SetTint(_crackRenderer, s.crackColor, flickerLow ? s.crackFlickerMinAlpha : 1f);
                _crackRenderer.transform.localScale = _crackBaseScale;

                SetTint(_screenDimRenderer, s.dimColor, s.dimDuringFreeze);
                yield return null;
            }

            // ---- 2. 해제 순간 ------------------------------------------------
            _shadeEmitter.Emit(splatPosition, s.shadeBurst, scale);
            _shardEmitter.Emit(splatPosition, s.shardBurst, scale);

            CameraManager.ShakeCamera(s.shakeAmplitude * scale, s.shakeDuration);
            onReleased?.Invoke();

            // ---- 3. 해제 후 여운 ---------------------------------------------
            float releaseDuration = s.ReleasePhaseDuration;
            for (float t = 0f; t < releaseDuration; t += Time.unscaledDeltaTime)
            {
                UpdateSplatRelease(s, t);
                UpdateCrackRelease(s, t);
                SetTint(_screenDimRenderer, s.dimColor, EvaluateDimAfterRelease(s, t));
                yield return null;
            }

            HideAll();
            _routine = null;
        }

        private void UpdateSplatRelease(PlayerHitEffectSettings s, float t)
        {
            float k = Mathf.Clamp01(t / s.splatReleaseDuration);
            float easeOut = 1f - (1f - k) * (1f - k);
            var release = new Vector3(
                Mathf.Lerp(1f, s.splatReleaseScale.x, easeOut),
                Mathf.Lerp(1f, s.splatReleaseScale.y, easeOut),
                1f);
            _splatRenderer.transform.localScale = Vector3.Scale(_splatBaseScale, release);
            SetTint(_splatRenderer, s.splatColor, 1f - k);
        }

        private void UpdateCrackRelease(PlayerHitEffectSettings s, float t)
        {
            float k = Mathf.Clamp01(t / s.crackFadeDuration);
            Vector3 crackScale = _crackBaseScale;
            crackScale.x *= 1f - s.crackShrinkOnFade * k;
            _crackRenderer.transform.localScale = crackScale;
            SetTint(_crackRenderer, s.crackColor, 1f - k);
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
            if (_targetCamera == null || _screenDimRenderer == null)
            {
                return;
            }

            // 카메라를 따라다녀야 하므로 카메라 자식으로 옮긴다.
            Transform dim = _screenDimRenderer.transform;
            dim.SetParent(_targetCamera.transform, false);
            dim.localRotation = Quaternion.identity;
            dim.localPosition = new Vector3(0f, 0f, _targetCamera.nearClipPlane + 0.05f);
        }

        /// <summary>카메라 시야 전체(+셰이크 여유)를 덮도록 딤 스프라이트 크기를 맞춘다.</summary>
        private void FitScreenDimToCamera()
        {
            if (_targetCamera == null || _screenDimRenderer == null || _screenDimRenderer.sprite == null)
            {
                return;
            }

            const float margin = 1.15f;
            float distance = _screenDimRenderer.transform.localPosition.z;
            float viewHeight = _targetCamera.orthographic
                ? _targetCamera.orthographicSize * 2f
                : 2f * distance * Mathf.Tan(_targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float viewWidth = viewHeight * _targetCamera.aspect;

            Vector2 spriteSize = _screenDimRenderer.sprite.bounds.size;
            _screenDimRenderer.transform.localScale = new Vector3(
                viewWidth * margin / spriteSize.x,
                viewHeight * margin / spriteSize.y,
                1f);
        }

        private void HideAll()
        {
            if (_splatRenderer != null) _splatRenderer.enabled = false;
            if (_crackRenderer != null) _crackRenderer.enabled = false;
            if (_screenDimRenderer != null) _screenDimRenderer.enabled = false;
        }

        private void SetSorting(Renderer target, int order)
        {
            if (target == null)
            {
                return;
            }

            target.sortingLayerName = _settings.sortingLayerName;
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
