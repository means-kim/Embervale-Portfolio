using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Traps
{
    public sealed class PhaseTrapSpawner : MonoBehaviour
    {
        [SerializeField] private PhaseTrapPool _pool;
        [SerializeField] private AirBossPhaseController _phase;
        [SerializeField] private AirBossController _boss;
        [SerializeField] private Health _bossHealth;
        [SerializeField] private Collider2D _player;
        [SerializeField] private CinematicManager _cinematic;
        [SerializeField] private PolygonCollider2D _lowerArea;
        [SerializeField] private PolygonCollider2D _upperArea;
        [SerializeField] private Transform[] _lowerSpawns = new Transform[0];
        [SerializeField] private Transform[] _upperSpawns = new Transform[0];
        [SerializeField, Min(0f)] private float _startDelay = 1.2f;
        [Header("난이도별 공격 시작 간격 (초, X: 최소 / Y: 최대)")]
        [SerializeField] private Vector2 _easyInterval = new Vector2(2.6f, 4.4f);
        [FormerlySerializedAs("_interval")]
        [SerializeField] private Vector2 _normalInterval = new Vector2(1.8f, 2.6f);
        [SerializeField] private Vector2 _hardInterval = new Vector2(0.8f, 1.8f);
        [SerializeField, Min(0.05f)] private float _warningTime = 0.8f;
        [SerializeField, Min(0.02f)] private float _flashTime = 0.1f;
        [SerializeField, Min(0.02f)] private float _fadeTime = 0.18f;
        [SerializeField, Min(1f)] private float _maxLength = 240f;
        [SerializeField, Min(0f)] private float _pastPlayer = 60f;
        [SerializeField, Min(0)] private int _damage = 20;
        private bool _armed, _wasRunning, _test;
        private float _nextFire;
        private Transform _lastSpawn;
        public bool IsRunning => CanRun();

        private void OnEnable()
        {
            if (_phase != null)
            {
                _phase.TransitionRequested += HandleTransition;
            }

            if (_bossHealth != null)
            {
                _bossHealth.OnDie += ResetTraps;
            }
        }

        private void OnDisable()
        {
            if (_phase != null)
            {
                _phase.TransitionRequested -= HandleTransition;
            }

            if (_bossHealth != null)
            {
                _bossHealth.OnDie -= ResetTraps;
            }

            ResetTraps();
        }

        public void ArmForBattle()
        {
            _armed = true;
            _test = false;
            _wasRunning = false;
        }

        public void ResetTraps()
        {
            _armed = _test = _wasRunning = false;
            _lastSpawn = null;
            if (_pool != null)
            {
                _pool.ReleaseAll();
            }
        }

        private void HandleTransition(int phase)
        {
            _test = false;
            _wasRunning = false;
            if (_pool != null)
            {
                _pool.ReleaseAll();
            }
        }

        private bool CanRun()
        {
            if (_boss != null && _boss.IsStaggered)
            {
                return false;
            }

            if (_player == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    _player = player.GetComponent<Collider2D>();
                }
            }

            if (_pool == null || _player == null || !_player.enabled || !_player.gameObject.activeInHierarchy)
            {
                return false;
            }

            var health = _player.GetComponentInParent<Health>();
            if (health == null || health.IsDead)
            {
                return false;
            }

            if (_test)
            {
                return true;
            }

            return _armed && _phase != null && _phase.isActiveAndEnabled && _phase.CurrentPhase >= 2
                   && !_phase.IsTransition && _boss != null && _boss.isActiveAndEnabled
                   && _bossHealth != null && !_bossHealth.IsDead
                   && (_cinematic == null || !_cinematic.IsPlaying);
        }

        private void Update()
        {
            bool running = CanRun();
            if (!running)
            {
                if (_wasRunning)
                {
                    _pool.ReleaseAll();
                }

                _wasRunning = false;
                return;
            }

            if (!_wasRunning)
            {
                _nextFire = Time.time + _startDelay;
                _wasRunning = true;
            }

            if (Time.time < _nextFire || _pool.ActiveCount > 0)
            {
                return;
            }

            FireOnce();
            Vector2 interval = GetInterval(GameDifficulty.Current);
            _nextFire = Time.time + Random.Range(interval.x, interval.y);
        }

        private Vector2 GetInterval(Difficulty difficulty)
        {
            Vector2 interval = difficulty switch
            {
                Difficulty.Easy => _easyInterval,
                Difficulty.Hard => _hardInterval,
                _ => _normalInterval
            };
            float minimum = Mathf.Max(0.1f, Mathf.Min(interval.x, interval.y));
            float maximum = Mathf.Max(minimum, Mathf.Max(interval.x, interval.y));
            return new Vector2(minimum, maximum);
        }

        public bool FireOnce()
        {
            if (!Application.isPlaying || !CanRun())
            {
                return false;
            }

            Vector3 target = _player.bounds.center;
            bool upper = _upperArea != null && _upperArea.OverlapPoint(target);
            if (!_test && !upper && _lowerArea != null && !_lowerArea.OverlapPoint(target))
            {
                return false;
            }

            var spawns = upper && _upperSpawns.Length > 0 ? _upperSpawns : _lowerSpawns;
            if (spawns.Length == 0)
            {
                return false;
            }

            int index = Random.Range(0, spawns.Length);
            // Avoid repeating the same location if an alternative is available.
            if (spawns.Length > 1 && spawns[index] == _lastSpawn)
            {
                index = (index + Random.Range(1, spawns.Length)) % spawns.Length;
            }

            var spawn = spawns[index];
            if (spawn == null || !spawn.gameObject.activeInHierarchy)
            {
                return false;
            }

            Vector3 start = spawn.position;
            start.z = target.z;
            Vector3 direction = target - start;
            float distance = direction.magnitude;
            if (distance < 0.1f || distance > _maxLength)
            {
                return false;
            }

            var item = _pool.Acquire();
            if (item == null)
            {
                return false;
            }

            Vector3 end = start + direction.normalized * Mathf.Min(_maxLength, distance + _pastPlayer);
            item.Fire(start, end, _player, _warningTime, _flashTime, _fadeTime, _damage, _boss);
            _lastSpawn = spawn;
            return true;
        }

        [ContextMenu("Test/Start Trap Loop")]
        public void StartTestLoop()
        {
            if (Application.isPlaying)
            {
                _test = true;
                _wasRunning = false;
            }
        }

        [ContextMenu("Test/Stop And Clear Traps")]
        private void TestStop() => ResetTraps();

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            foreach (var spawn in _lowerSpawns)
            {
                if (spawn != null)
                {
                    Gizmos.DrawWireSphere(spawn.position, 0.6f);
                }
            }

            foreach (var spawn in _upperSpawns)
            {
                if (spawn != null)
                {
                    Gizmos.DrawWireSphere(spawn.position, 0.6f);
                }
            }
        }
    }
}
