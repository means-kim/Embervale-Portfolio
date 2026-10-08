using System;
using System.Collections;
using UnityEngine;

namespace Game.Effects
{
    /// <summary>
    /// 바닥 심연 이펙트(AbyssFloor 셰이더)의 수면 높이를 제어한다.
    /// 페이즈 전환 때 심연이 차오르는 연출과, 이후 대미지 판정이 참조할 수면 위치를 제공한다.
    /// </summary>
    /// <remarks>
    /// 머티리얼 인스턴스를 만들지 않도록 MaterialPropertyBlock으로 값을 넣는다.
    /// (renderer.material 접근 시 머티리얼이 복제되어 배칭이 깨지고 누수 관리가 필요해짐)
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    public sealed class AbyssFloor : MonoBehaviour
    {
        private static readonly int SurfaceHeightId = Shader.PropertyToID("_SurfaceHeight");

        [Tooltip("시작 시 수면 높이 (Quad 기준 0=바닥, 1=윗변). 0이면 숨겨진 상태로 시작.")]
        [SerializeField, Range(0f, 1f)] private float _initialHeight = 0f;

        [Tooltip("차오르는 연출에 쓸 이징 커브 (0~1 구간).")]
        [SerializeField] private AnimationCurve _riseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Renderer _cachedRenderer;
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _heightRoutine;

        /// <summary>현재 수면 높이 (UV 0~1).</summary>
        public float CurrentHeight { get; private set; }

        /// <summary>
        /// 물결을 제외한 기준 수면의 월드 Y 좌표. 대미지 판정 등에서 사용한다.
        /// 회전하지 않은 Quad/Sprite를 전제로 한다.
        /// </summary>
        public float SurfaceWorldY
        {
            get
            {
                Bounds bounds = _cachedRenderer.bounds;
                return bounds.min.y + bounds.size.y * CurrentHeight;
            }
        }

        /// <summary>높이 변경 연출이 끝났을 때 호출된다.</summary>
        public event Action HeightTransitionCompleted;

        private void Awake()
        {
            _cachedRenderer = GetComponent<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
            SetHeight(_initialHeight);
        }

        private void OnDisable()
        {
            StopTransition();
        }

        /// <summary>수면 높이를 즉시 설정한다.</summary>
        /// <param name="height">UV 기준 높이 (0~1).</param>
        public void SetHeight(float height)
        {
            StopTransition();
            ApplyHeight(height);
        }

        /// <summary>
        /// 지정한 높이까지 수면을 부드럽게 이동시킨다. 진행 중인 연출은 취소된다.
        /// </summary>
        /// <param name="targetHeight">UV 기준 목표 높이 (0~1).</param>
        /// <param name="duration">이동 시간(초). 0 이하면 즉시 적용.</param>
        public void TransitionTo(float targetHeight, float duration)
        {
            StopTransition();

            if (duration <= 0f || !isActiveAndEnabled)
            {
                ApplyHeight(targetHeight);
                HeightTransitionCompleted?.Invoke();
                return;
            }

            _heightRoutine = StartCoroutine(TransitionRoutine(CurrentHeight, Mathf.Clamp01(targetHeight), duration));
        }

        private void StopTransition()
        {
            if (_heightRoutine == null)
            {
                return;
            }

            StopCoroutine(_heightRoutine);
            _heightRoutine = null;
        }

        private IEnumerator TransitionRoutine(float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = _riseCurve.Evaluate(Mathf.Clamp01(elapsed / duration));
                ApplyHeight(Mathf.LerpUnclamped(from, to, t));
                yield return null;
            }

            ApplyHeight(to);
            _heightRoutine = null;
            HeightTransitionCompleted?.Invoke();
        }

        private void ApplyHeight(float height)
        {
            CurrentHeight = Mathf.Clamp01(height);

            _cachedRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(SurfaceHeightId, CurrentHeight);
            _cachedRenderer.SetPropertyBlock(_propertyBlock);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Renderer targetRenderer = _cachedRenderer != null ? _cachedRenderer : GetComponent<Renderer>();
            if (targetRenderer == null)
            {
                return;
            }

            float height = Application.isPlaying ? CurrentHeight : _initialHeight;
            Bounds bounds = targetRenderer.bounds;
            float y = bounds.min.y + bounds.size.y * height;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(bounds.min.x, y, 0f), new Vector3(bounds.max.x, y, 0f));
        }
#endif
    }
}
