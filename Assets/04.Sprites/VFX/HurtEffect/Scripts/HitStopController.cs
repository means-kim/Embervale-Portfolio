using UnityEngine;

namespace Game.Effects
{
    /// <summary>
    /// Time.timeScale을 이용한 히트스톱(프레임 정지) 담당.
    /// timeScale은 전역 상태라 소유자를 하나로 모아야 충돌이 없다 → 씬에 하나만 두고 참조로 주입해서 쓴다.
    /// </summary>
    public sealed class HitStopController : MonoBehaviour
    {
        private float _remaining;
        private float _restoreTimeScale = 1f;
        private float _appliedTimeScale;
        private bool _isActive;

        /// <summary>현재 히트스톱이 진행 중인지 여부.</summary>
        public bool IsActive => _isActive;

        /// <summary>
        /// 히트스톱을 요청한다. 이미 진행 중이면 남은 시간 중 더 긴 쪽을 유지한다.
        /// </summary>
        /// <param name="duration">정지 시간(초, unscaled).</param>
        /// <param name="timeScale">정지 중 적용할 timeScale (0 = 완전 정지).</param>
        public void Request(float duration, float timeScale)
        {
            if (duration <= 0f)
            {
                return;
            }

            if (!_isActive)
            {
                _restoreTimeScale = Time.timeScale;
                _isActive = true;
            }

            _remaining = Mathf.Max(_remaining, duration);
            _appliedTimeScale = timeScale;
            Time.timeScale = timeScale;
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            // timeScale이 0이어도 unscaledDeltaTime은 흐른다.
            _remaining -= Time.unscaledDeltaTime;
            if (_remaining <= 0f)
            {
                End();
            }
        }

        private void OnDisable()
        {
            if (_isActive)
            {
                End();
            }
        }

        private void End()
        {
            _isActive = false;
            _remaining = 0f;

            // 정지 중에 일시정지 메뉴 등 다른 시스템이 timeScale을 바꿨다면 덮어쓰지 않는다.
            if (Mathf.Approximately(Time.timeScale, _appliedTimeScale))
            {
                Time.timeScale = _restoreTimeScale;
            }
        }
    }
}
