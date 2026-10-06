using UnityEngine;

namespace Game.Effects
{
    /// <summary>
    /// 카메라에 감쇠하는 노이즈 흔들림을 더한다.
    /// 매 프레임 "이전 오프셋 제거 → 새 오프셋 적용" 방식이라 카메라 추적 스크립트와 누적되지 않는다.
    /// 실행 순서를 늦춰서(10000) 추적 스크립트의 LateUpdate 이후에 오프셋이 적용되게 한다.
    /// </summary>
    [DefaultExecutionOrder(10000)]
    public sealed class CameraShaker : MonoBehaviour
    {
        private Vector3 _appliedOffset;
        private float _amplitude;
        private float _duration;
        private float _frequency = 30f;
        private float _elapsed;
        private float _seed;

        private bool IsShaking => _elapsed < _duration;

        /// <summary>
        /// 흔들림을 시작한다. 진행 중인 흔들림보다 약하면 무시한다.
        /// </summary>
        /// <param name="amplitude">최대 이동량(유닛).</param>
        /// <param name="duration">지속 시간(초, unscaled).</param>
        /// <param name="frequency">초당 노이즈 샘플링 빈도.</param>
        public void Shake(float amplitude, float duration, float frequency)
        {
            if (IsShaking && CurrentAmplitude() > amplitude)
            {
                return;
            }

            _amplitude = amplitude;
            _duration = duration;
            _frequency = frequency;
            _elapsed = 0f;
            _seed = Random.value * 100f;
        }

        private void Update()
        {
            // 다른 스크립트의 LateUpdate(카메라 추적)가 깨끗한 위치를 보도록 먼저 제거.
            RemoveOffset();
        }

        private void LateUpdate()
        {
            if (!IsShaking)
            {
                return;
            }

            _elapsed += Time.unscaledDeltaTime;

            float t = Time.unscaledTime * _frequency;
            float amp = CurrentAmplitude();
            var offset = new Vector3(
                (Mathf.PerlinNoise(_seed, t) * 2f - 1f) * amp,
                (Mathf.PerlinNoise(_seed + 37.1f, t) * 2f - 1f) * amp,
                0f);

            transform.localPosition += offset;
            _appliedOffset = offset;
        }

        private void OnDisable()
        {
            RemoveOffset();
            _elapsed = _duration;
        }

        private float CurrentAmplitude()
        {
            float k = 1f - Mathf.Clamp01(_elapsed / _duration);
            return _amplitude * k * k; // ease-out 감쇠
        }

        private void RemoveOffset()
        {
            if (_appliedOffset == Vector3.zero)
            {
                return;
            }

            transform.localPosition -= _appliedOffset;
            _appliedOffset = Vector3.zero;
        }
    }
}
