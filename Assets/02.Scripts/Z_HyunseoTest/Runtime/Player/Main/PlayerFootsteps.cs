using UnityEngine;
namespace HyunseoTest
{
    // Copied responsibility: follow animation phase instead of an unrelated timer.
    [RequireComponent(typeof(PlayerMove), typeof(PlayerJump), typeof(PlayerWall))]
    public class PlayerFootsteps : MonoBehaviour
    {
        [SerializeField] private FootstepSFXBank _groundBank;
        [SerializeField] private FootstepSFXBank _platformBank;
        [SerializeField] private AudioSource _groundSource;
        [SerializeField] private AudioSource _platformSource;
        [SerializeField] private bool _playOnLand;
        [Tooltip("Run_00 / Run_04 contact phases")]
        [SerializeField] private Vector2 _runContacts = new Vector2(0f, 0.48f);
        private PlayerMove _move;
        private PlayerJump _jump;
        private PlayerWall _wall;
        private PlayerAttack _attack;
        private Animator _animator;
        private Rigidbody2D _body;
        private int _lastState;
        private float _lastPhase = -0.001f;
        private float _attackStride;
        private float _runDuration = 0.5208333f;
        private bool _stepping;
        private void Awake()
        {
            _move = GetComponent<PlayerMove>(); _jump = GetComponent<PlayerJump>();
            _wall = GetComponent<PlayerWall>(); _attack = GetComponent<PlayerAttack>();
            _animator = GetComponent<Animator>(); _body = GetComponent<Rigidbody2D>();
            foreach (var clip in _animator.runtimeAnimatorController.animationClips)
                if (clip.name == "Run") _runDuration = clip.length;
            if (_groundSource == null) _groundSource = GetComponent<AudioSource>();
            if (_platformSource == null) _platformSource = _groundSource;
        }
        private void OnEnable() { if (_jump != null) _jump.OnLanded += Land; }
        private void OnDisable() { if (_jump != null) _jump.OnLanded -= Land; Stop(); }
        private void Land() { if (_playOnLand) PlayStepOnce(); }
        private void Stop()
        {
            _groundSource?.Stop();
            if (_platformSource != _groundSource) _platformSource?.Stop();
            _stepping = false; _lastState = 0; _lastPhase = -0.001f; _attackStride = 0f;
        }
        private void LateUpdate()
        {
            bool wall = _wall.IsClimbing && Mathf.Abs(_body.linearVelocity.y) > 0.01f;
            bool run = _jump.IsGrounded && _move.HasMoveInput && _move.IsMoving && !_move.MovementLocked;
            if (!wall && !run) { if (_stepping) Stop(); return; }
            var state = _animator.GetCurrentAnimatorStateInfo(0);
            bool visibleGait = wall ? state.IsName("WallClimb") : state.IsName("Run");
            if (visibleGait)
            {
                float phase = state.normalizedTime;
                if (!_stepping || state.shortNameHash != _lastState || phase < _lastPhase)
                    _lastPhase = Mathf.Floor(phase) - 0.001f;
                float second = wall ? 0.5f : _runContacts.y;
                if (Crossed(_lastPhase, phase, _runContacts.x) || Crossed(_lastPhase, phase, second)) Play(wall);
                _lastPhase = phase; _lastState = state.shortNameHash;
                _attackStride = phase % 1f;
            }
            else if (_attack.IsAttacking)
            {
                // Attack sprites have no running gait: preserve run cadence during translation.
                float next = _attackStride + Time.deltaTime / Mathf.Max(0.01f, _runDuration);
                if (Crossed(_attackStride, next, 0f) || Crossed(_attackStride, next, _runContacts.y)) Play(wall);
                _attackStride = next % 1f;
                _lastState = 0;
            }
            _stepping = true;
        }
        private static bool Crossed(float before, float after, float phase)
            => Mathf.FloorToInt(after - phase) > Mathf.FloorToInt(before - phase);
        private void Play(bool wall)
        {
            bool platform = wall || _jump.IsOnPlatform;
            var bank = platform ? _platformBank : _groundBank;
            var source = platform ? _platformSource : _groundSource;
            if (bank == null || source == null) return;
            bank.PlayOn(source);
        }
        public void PlayStepOnce() => Play(false);
        public void PlayGroundStepOnce() => Play(false);
        public void PlayPlatformStepOnce() => Play(true);
    }
}
