using UnityEngine;

namespace HyunseoTest
{
    [RequireComponent(typeof(PlayerDamageHitbox), typeof(Rigidbody2D))]
    public class PlayerAttackInstance : MonoBehaviour
    {
        [SerializeField, Min(0)] private float _hitDelay;
        [SerializeField, Min(0.05f)] private float _hitDuration = 0.22f;
        [Tooltip("플레이어 행동 종료/피격 후에도 이미 생성된 공격을 유지한다. 화염구에 사용.")]
        [SerializeField] private bool _independent;
        [Tooltip("Melee: only the owner's animation events may enable damage. Timer fields apply to projectiles.")]
        [SerializeField] private bool _animationDriven;
        [Tooltip("Rotate the effect and hitbox locally before horizontal facing mirroring.")]
        [SerializeField] private float _localAttackAngle;
        [Tooltip("Visual lifetime from swing start. Does not extend damage.")]
        [SerializeField, Min(0.05f)] private float _swingVisualDuration = 0.32f;
        [SerializeField, Min(0f)] private float _swingFadeDuration = 0.1f;
        private SpriteRenderer[] _swingRenderers;
        private Color[] _swingColors;
        private bool _visualStarted;
        private float _visualAge;
        private PlayerDamageHitbox _hitbox;
        private float _age;
        private bool _initialized;
        private bool _ended;
        public int Direction { get; private set; } = 1;
        public Transform Owner { get; private set; }
        private Vector3 _ownerOffset;
        private void LateUpdate()
        {
            if (_initialized && !_independent && !_ended && Owner != null)
                transform.position = Owner.position + _ownerOffset;
        }
        private void Awake()
        {
            _hitbox = GetComponent<PlayerDamageHitbox>();
            var body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0;
            _hitbox.SetActive(false);
            _swingRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            _swingColors = System.Array.ConvertAll(_swingRenderers, r => r.color);
            if (_animationDriven) ShowSwing(false);
        }
        public bool Initialize(int damage, int direction, Transform owner)
        {
            if (!_hitbox.IsConfigured)
            {
                Debug.LogError($"공격 판정 콜라이더를 명시적으로 연결하세요: {name}", this);
                return false;
            }
            Owner = owner;
            _ownerOffset = owner != null ? transform.position - owner.position : Vector3.zero;
            Direction = direction < 0 ? -1 : 1;
            var scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Direction;
            transform.localScale = scale;
            // Mirror the whole rotated attack in world X: left facing must still strike upwards.
            transform.rotation = Quaternion.Euler(0f, 0f, _localAttackAngle * Direction);
            _age = 0;
            _initialized = true;
            _hitbox.Initialize(damage, owner);
            _hitbox.SetActive(!_animationDriven && _hitDelay <= 0);
            return true;
        }
        private void Update()
        {
            AdvanceSwingVisual(Time.deltaTime);
            if (!_initialized || _ended || _animationDriven) return;
            _age += Time.deltaTime;
            if (_age >= Mathf.Max(0, _hitDelay) + Mathf.Max(0.05f, _hitDuration)) StopDamage();
            else if (_age >= _hitDelay && !_hitbox.IsActive) _hitbox.SetActive(true);
        }
        public void EndPlayerAction()
        {
            if (!_independent) StopDamage();
            if (_animationDriven && !_visualStarted) Destroy(gameObject);
        }
        public void OpenAnimationWindow()
        {
            if (!_animationDriven || !_initialized || _ended || _visualStarted) return;
            _visualStarted = true;
            _visualAge = 0f;
            _hitbox.SetActive(true);
            ShowSwing(true);
            var animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play(0, 0, 0f);
                animator.Update(0f);
                var clips = animator.GetCurrentAnimatorClipInfo(0);
                float stateSpeed = Mathf.Abs(animator.GetCurrentAnimatorStateInfo(0).speed);
                if (clips.Length > 0 && stateSpeed > 0f)
                    animator.speed = clips[0].clip.length / (stateSpeed * Mathf.Max(0.05f, _swingVisualDuration));
            }
        }
        private void AdvanceSwingVisual(float deltaTime)
        {
            if (!_animationDriven || !_visualStarted) return;
            _visualAge += Mathf.Max(0f, deltaTime);
            float remaining = _swingVisualDuration - _visualAge;
            float alpha = _swingFadeDuration > 0f ? Mathf.Clamp01(remaining / _swingFadeDuration) : 1f;
            for (int i = 0; i < _swingRenderers.Length; i++)
            {
                if (_swingRenderers[i] == null) continue;
                var color = _swingColors[i]; color.a *= alpha;
                _swingRenderers[i].color = color;
            }
            if (remaining <= 0f)
            {
                StopDamage();
                ShowSwing(false);
                _visualStarted = false;
                Destroy(gameObject);
            }
        }
        private void ShowSwing(bool visible)
        {
            foreach (var renderer in _swingRenderers)
                if (renderer != null) renderer.enabled = visible;
        }
        public void StopDamage()
        {
            _ended = true;
            _hitbox.SetActive(false);
        }
        public bool TryGetHitBounds(out Bounds bounds) => _hitbox.TryGetBounds(out bounds);
    }

}
