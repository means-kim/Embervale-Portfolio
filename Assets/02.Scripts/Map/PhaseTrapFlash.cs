using UnityEngine;

namespace Game.Traps
{
    public sealed class PhaseTrapFlash : MonoBehaviour
    {
        [SerializeField] private LineRenderer _path;
        [SerializeField] private LineRenderer _glow;
        [SerializeField] private BoxCollider2D _hitbox;
        [SerializeField, Min(0.01f)] private float _warningWidth = 0.18f;
        [SerializeField, Min(0.01f)] private float _damageWidth = 0.9f;
        [SerializeField, Min(0.01f)] private float _glowWidth = 2.4f;
        [SerializeField] private Color _warningColor = new Color(1f, 0.08f, 0.04f, 0.55f);
        [SerializeField] private Color _flashColor = Color.white;
        private Collider2D _playerCollider;
        private Health _playerHealth;
        private float _warningTime, _flashTime, _fadeTime, _elapsed;
        private int _damage;
        private bool _hit;
        private int _stage;
        public bool IsWarning => gameObject.activeSelf && _stage == 0;
        public bool IsFlashing => gameObject.activeSelf && _stage == 1;
        public Vector3 StartPoint { get; private set; }
        public Vector3 EndPoint { get; private set; }

        public void Fire(Vector3 start, Vector3 end,
            Collider2D player, float warning, float flash, float fade, int damage)
        {
            _playerCollider = player;
            if (player != null)
            {
                _playerHealth = player.GetComponentInParent<Health>();
            }
            else
            {
                _playerHealth = null;
            }

            StartPoint = start;
            EndPoint = end;
            Vector2 direction = end - start;
            float length = direction.magnitude;
            transform.SetPositionAndRotation((start + end) * 0.5f,
                Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
            transform.localScale = Vector3.one;
            FitLine(_path, length, _warningWidth);
            FitLine(_glow, length, _glowWidth);
            _hitbox.size = new Vector2(length, _damageWidth);
            _hitbox.offset = Vector2.zero;
            _hitbox.enabled = false;
            _warningTime = Mathf.Max(0.05f, warning);
            _flashTime = Mathf.Max(0.02f, flash);
            _fadeTime = Mathf.Max(0.02f, fade);
            _damage = Mathf.Max(0, damage);
            _elapsed = 0;
            _stage = 0;
            _hit = false;
            SetColor(_path, _warningColor);
            _glow.enabled = false;
            gameObject.SetActive(true);
        }

        private static void FitLine(LineRenderer renderer, float length, float width)
        {
            renderer.useWorldSpace = false;
            renderer.positionCount = 2;
            renderer.SetPosition(0, Vector3.left * length * 0.5f);
            renderer.SetPosition(1, Vector3.right * length * 0.5f);
            renderer.widthMultiplier = width;
        }

        private static void SetColor(LineRenderer renderer, Color color)
        {
            renderer.startColor = renderer.endColor = color;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_stage == 0)
            {
                var color = _warningColor;
                color.a = Mathf.Lerp(0.2f, _warningColor.a, Mathf.Clamp01(_elapsed / _warningTime));
                SetColor(_path, color);
                if (_elapsed < _warningTime) return;
                _elapsed = 0;
                _stage = 1;
                _path.widthMultiplier = _damageWidth;
                SetColor(_path, _flashColor);
                SetColor(_glow, new Color(_flashColor.r, _flashColor.g, _flashColor.b, 0.65f));
                _glow.enabled = true;
                _hitbox.enabled = true;
                Physics2D.SyncTransforms();
                TryHit();
            }
            else if (_stage == 1)
            {
                TryHit();
                if (_elapsed < _flashTime) return;
                _elapsed = 0;
                _stage = 2;
                _hitbox.enabled = false;
            }
            else
            {
                float alpha = 1f - Mathf.Clamp01(_elapsed / _fadeTime);
                SetColor(_path, new Color(_flashColor.r, _flashColor.g, _flashColor.b, alpha));
                SetColor(_glow, new Color(_flashColor.r, _flashColor.g, _flashColor.b, alpha * 0.65f));
                if (_elapsed >= _fadeTime) Release();
            }
        }

        private void TryHit()
        {
            if (_hit || _playerHealth == null || _playerHealth.IsDead || _playerCollider == null
                || !_playerCollider.enabled || !_playerCollider.gameObject.activeInHierarchy)
            {
                return;
            }

            if (!Physics2D.Distance(_hitbox, _playerCollider).isOverlapped)
            {
                return;
            }

            _hit = true;
            _playerHealth.TakeDamage(_damage);
        }

        public void Release()
        {
            _hitbox.enabled = false;
            _glow.enabled = false;
            _playerCollider = null;
            _playerHealth = null;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (_hitbox != null)
            {
                _hitbox.enabled = false;
            }
        }
    }
}