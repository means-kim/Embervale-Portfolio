using UnityEngine;
namespace HyunseoTest
{
    [RequireComponent(typeof(global::Health))]
    public class PlayerHit : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _hitDuration = 0.35f;
        [SerializeField, Min(0.01f)] private float _flashInterval = 0.08f;
        [SerializeField] private Color _flashColor = new Color(1f, 0.25f, 0.25f, 1f);
        [Header("피격 넉백")]
        [SerializeField, Min(0f)] private float _knockbackForce = 4f;
        [SerializeField] private float _knockbackUp = 2f;
        [SerializeField, Min(0f)] private float _knockbackLockTime = 0.15f;
        public bool IsHit => Time.time < _hitUntil;
        private global::Health _health;
        private PlayerController _controller;
        private PlayerDash _dash;
        private PlayerAttack _attack;
        private PlayerMove _move;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer[] _renderers;
        private Color[] _originalColors;
        private float _hitUntil;
        private float _flashTimer;
        private bool _flashOn;
        private float _moveUnlockAt;
        private bool _knockbackLocking;
        private void Awake()
        {
            _health = GetComponent<global::Health>();
            _controller = GetComponent<PlayerController>();
            _dash = GetComponent<PlayerDash>();
            _attack = GetComponent<PlayerAttack>();
            _move = GetComponent<PlayerMove>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _renderers = GetComponentsInChildren<SpriteRenderer>(true);
            _originalColors = new Color[_renderers.Length];
            for (int i = 0; i < _renderers.Length; i++) _originalColors[i] = _renderers[i].color;
        }
        private void OnEnable()
        {
            _health.OnHit += HandleHit;
            _health.OnDie += HandleDie;
        }
        private void OnDisable()
        {
            _health.OnHit -= HandleHit;
            _health.OnDie -= HandleDie;
            RestoreColors();
            ReleaseKnockbackLock();
        }

        private void ApplyKnockback()
        {
            if (_rigidbody == null) return;
            int facing = _move != null ? _move.FacingDirection : 1;
            float xPush = -facing * _knockbackForce;
            if (_move != null && _knockbackLockTime > 0f)
            {
                _move.SetMovementLocked(this, true);
                _moveUnlockAt = Time.time + _knockbackLockTime;
                _knockbackLocking = true;
            }
            _rigidbody.linearVelocity = new Vector2(xPush, Mathf.Max(_rigidbody.linearVelocity.y, _knockbackUp));
        }

        private void ReleaseKnockbackLock()
        {
            if (!_knockbackLocking) return;
            _knockbackLocking = false;
            if (_move != null) _move.SetMovementLocked(this, false);
        }
        public void ResetForRespawn()
        {
            _hitUntil = 0f;
            _flashTimer = 0f;
            _moveUnlockAt = 0f;
            ReleaseKnockbackLock();
            if (_move != null) _move.SetMovementLocked(_health, false);
            RestoreColors();
        }

        private void HandleHit()
        {
            if (_health.IsDead) return;
            if (_attack != null) _attack.CancelAttack();
            _hitUntil = Time.time + _hitDuration;
            _flashTimer = 0f;
            _flashOn = false;
            if (_dash != null) _dash.CancelDash();
            ApplyKnockback();
            ;
        }
        private void HandleDie()
        {
            if (_attack != null) _attack.CancelAttack();
            if (_dash != null) _dash.CancelDash();
            if (_move != null) _move.SetMovementLocked(_health, true);
            _hitUntil = 0f;
            RestoreColors();
            ;
            if (_controller != null) _controller.ChangeState(PlayerState.Death);
        }
        private void Update()
        {
            if (_knockbackLocking && Time.time >= _moveUnlockAt) ReleaseKnockbackLock();
            if (!IsHit)
            {
                if (_flashOn) RestoreColors();
                return;
            }
            _flashTimer += Time.deltaTime;
            if (_flashTimer < _flashInterval) return;
            _flashTimer = 0f;
            _flashOn = !_flashOn;
            ApplyFlash(_flashOn);
        }
        private void ApplyFlash(bool on)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] == null) continue;
                _renderers[i].color = on ? _flashColor : _originalColors[i];
            }
        }
        private void RestoreColors()
        {
            _flashOn = false;
            ApplyFlash(false);
        }
    }

}
