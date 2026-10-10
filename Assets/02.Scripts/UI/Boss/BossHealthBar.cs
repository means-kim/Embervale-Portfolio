using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Boss
{
    /// <summary>
    /// 보스 체력바 View.
    /// 메인 게이지(빨강)는 피격 즉시 줄고, 잔상 게이지(노랑)는 잠시 뒤 따라 줄어든다.
    /// </summary>
    /// <remarks>
    /// - 체력 계산은 하지 않는다. 보스 쪽 체력 로직이 <see cref="SetHealth"/>로 값만 넘겨준다.
    /// - 애니메이션 중일 때만 Update가 돌고, 다 끝나면 컴포넌트를 꺼서 매 프레임 비용을 없앤다.
    /// - 보스 연출(히트스톱, 페이즈 전환)에서 timeScale을 0으로 만들어도 UI는 움직이도록 기본값은 unscaled time.
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class BossHealthBar : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Image Type = Filled / Horizontal / Origin Left")]
        [SerializeField] private Image _mainFill;
        [Tooltip("Image Type = Filled / Horizontal / Origin Left. mainFill보다 뒤(위쪽 형제)에 배치")]
        [SerializeField] private Image _delayedFill;
        [Tooltip("보스 이름 표시용 (선택). 비워두면 이름을 표시하지 않는다.")]
        [SerializeField] private TMP_Text _bossNameLabel;

        [Header("Colors")]
        // 스프라이트에 음영(최대 밝기 약 0.8~1.0)이 들어가 있어서 기본색을 조금 밝게 잡았다.
        [SerializeField] private Color _mainColor = new Color(0.74f, 0.17f, 0.14f, 1f);
        [SerializeField] private Color _delayedColor = new Color(0.98f, 0.68f, 0.28f, 1f);
        [SerializeField] private Color _hitFlashColor = new Color(1.00f, 0.95f, 0.92f, 1f);

        [Header("Timing")]
        [Tooltip("피격 후 잔상이 줄어들기 시작할 때까지의 대기 시간. 연타 중에는 계속 리셋된다.")]
        [SerializeField, Min(0f)] private float _catchUpDelay = 0.45f;
        [Tooltip("잔상이 줄어드는 속도 (초당 비율, 1 = 전체 바)")]
        [SerializeField, Min(0.01f)] private float _catchUpSpeed = 0.6f;
        [Tooltip("회복 시 메인 게이지가 차오르는 속도 (초당 비율)")]
        [SerializeField, Min(0.01f)] private float _healSpeed = 1.5f;
        [SerializeField, Min(0f)] private float _hitFlashDuration = 0.08f;
        [SerializeField, Min(0f)] private float _fadeDuration = 0.4f;
        [SerializeField] private bool _useUnscaledTime = true;
        [SerializeField] private bool _startHidden = true;

        private CanvasGroup _canvasGroup;
        private float _maxHealth = 1f;
        private float _targetRatio = 1f;
        private float _mainRatio = 1f;
        private float _delayedRatio = 1f;
        private float _catchUpTimer;
        private float _flashTimer;
        private float _targetAlpha;

        /// <summary>현재 표시 목표 체력 비율 (0~1).</summary>
        public float TargetRatio => _targetRatio;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _targetAlpha = _startHidden ? 0f : 1f;
            _canvasGroup.alpha = _targetAlpha;

            ApplyColors(0f);
            ApplyFills();
            enabled = false;
        }

        /// <summary>
        /// 전투 시작 시 최대/현재 체력을 설정하고 게이지를 애니메이션 없이 맞춘다.
        /// </summary>
        /// <param name="max">최대 체력 (0보다 커야 함).</param>
        /// <param name="current">현재 체력. 생략하면 최대 체력.</param>
        public void Initialize(float max, float? current = null)
        {
            _maxHealth = Mathf.Max(max, 0.0001f);
            _targetRatio = ToRatio(current ?? max);
            _mainRatio = _targetRatio;
            _delayedRatio = _targetRatio;
            _catchUpTimer = 0f;
            _flashTimer = 0f;

            ApplyColors(0f);
            ApplyFills();
        }

        /// <summary>보스 이름을 표시한다. 이름 라벨이 연결되지 않았으면 무시한다.</summary>
        /// <param name="bossName">표시할 보스 이름.</param>
        public void SetBossName(string bossName)
        {
            if (_bossNameLabel != null)
            {
                _bossNameLabel.text = bossName;
            }
        }

        /// <summary>현재 체력을 반영한다. 감소면 피격 연출, 증가면 회복 연출.</summary>
        /// <param name="current">현재 체력 (0~최대 체력).</param>
        public void SetHealth(float current)
        {
            float newRatio = ToRatio(current);
            if (Mathf.Approximately(newRatio, _targetRatio))
            {
                return;
            }

            if (newRatio < _targetRatio)
            {
                // 피격: 메인은 즉시, 잔상은 대기 후 따라감
                _mainRatio = newRatio;
                _catchUpTimer = _catchUpDelay;
                _flashTimer = _hitFlashDuration;
            }
            else
            {
                // 회복: 잔상(노랑)이 먼저 목표까지 차고 메인이 따라 차오름
                _delayedRatio = Mathf.Max(_delayedRatio, newRatio);
            }

            _targetRatio = newRatio;
            enabled = true;
        }

        /// <summary>페이드 인으로 표시한다. (보스 등장 연출 등)</summary>
        public void Show() => FadeTo(1f);

        /// <summary>페이드 아웃으로 숨긴다. (보스 처치, 페이즈 전환 등)</summary>
        public void Hide() => FadeTo(0f);

        private void FadeTo(float alpha)
        {
            _targetAlpha = alpha;
            if (_fadeDuration <= 0f)
            {
                _canvasGroup.alpha = alpha;
                return;
            }

            enabled = true;
        }

        private void Update()
        {
            float deltaTime;
            if (_useUnscaledTime)
            {
                deltaTime = Time.unscaledDeltaTime;
            }
            else
            {
                deltaTime = Time.deltaTime;
            }

            UpdateFade(deltaTime);
            UpdateMain(deltaTime);
            UpdateDelayed(deltaTime);
            UpdateFlash(deltaTime);

            ApplyFills();

            if (IsSettled())
            {
                enabled = false;
            }
        }

        private void UpdateFade(float deltaTime)
        {
            if (_fadeDuration <= 0f)
            {
                return;
            }

            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, _targetAlpha, deltaTime / _fadeDuration);
        }

        private void UpdateMain(float deltaTime)
        {
            if (_mainRatio < _targetRatio)
            {
                _mainRatio = Mathf.MoveTowards(_mainRatio, _targetRatio, _healSpeed * deltaTime);
            }
        }

        private void UpdateDelayed(float deltaTime)
        {
            if (_delayedRatio <= _mainRatio)
            {
                _delayedRatio = _mainRatio;
                return;
            }

            // 회복 중에는 잔상이 목표값에 고정되어 있어야 하므로 줄이지 않는다.
            if (_mainRatio < _targetRatio)
            {
                return;
            }

            if (_catchUpTimer > 0f)
            {
                _catchUpTimer -= deltaTime;
                return;
            }

            _delayedRatio = Mathf.MoveTowards(_delayedRatio, _mainRatio, _catchUpSpeed * deltaTime);
        }

        private void UpdateFlash(float deltaTime)
        {
            if (_flashTimer <= 0f)
            {
                return;
            }

            _flashTimer = Mathf.Max(0f, _flashTimer - deltaTime);


            float flashWeight;

            if (_hitFlashDuration > 0f)
            {
                flashWeight = _flashTimer / _hitFlashDuration;
            }
            else
            {
                flashWeight = 0f;
            }
            ApplyColors(flashWeight);
        }

        private bool IsSettled()
        {
            return Mathf.Approximately(_mainRatio, _targetRatio)
                && Mathf.Approximately(_delayedRatio, _mainRatio)
                && _flashTimer <= 0f
                && Mathf.Approximately(_canvasGroup.alpha, _targetAlpha);
        }

        private float ToRatio(float health)
        {
            return Mathf.Clamp01(health / _maxHealth);
        }

        private void ApplyFills()
        {
            _mainFill.fillAmount = _mainRatio;
            _delayedFill.fillAmount = _delayedRatio;
        }

        private void ApplyColors(float flashWeight)
        {
            _mainFill.color = Color.Lerp(_mainColor, _hitFlashColor, flashWeight);
            _delayedFill.color = _delayedColor;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Inspector에서 색을 바꾸면 에디터에서도 바로 확인되도록
            if (_mainFill != null)
            {
                _mainFill.color = _mainColor;
            }
            if (_delayedFill != null)
            {
                _delayedFill.color = _delayedColor;
            }
        }
#endif
    }
}
