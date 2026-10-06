using UnityEngine;
using Unity.Cinemachine;

namespace HyunseoTest
{
    [RequireComponent(typeof(Rigidbody2D), typeof(global::Health), typeof(PlayerLevel))]
    public class Player : MonoBehaviour
    {
        public event System.Action OnRestarted;
        [SerializeField] private Transform _checkPointTransform;
        private Rigidbody2D _rigidbody2D;
        private Vector3 _respawnPosition;
        private RespawnPoint _respawnPoint;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _respawnPosition = _checkPointTransform != null ? _checkPointTransform.position : transform.position;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Border")) PlayerDeath();
        }

        private void PlayerDeath()
        {
            if (global::Respone.Instance != null)
            {
                global::Respone.Instance.ResponePlayer(this, _respawnPosition);
            }
            else
            {
                MoveToCheckpoint(_respawnPosition);
            }
        }

        public void MoveToCheckpoint(Vector3 position)
        {
            Vector3 positionDelta = position - transform.position;
            GetComponent<PlayerAttack>()?.CancelAttack();
            GetComponent<PlayerDash>()?.CancelDash();
            GetComponent<PlayerJump>()?.ResetForRespawn();
            GetComponent<PlayerWall>()?.ResetForRespawn();
            GetComponent<PlayerMove>()?.ResetForRespawn();

            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0f;
            transform.position = position;

            // Teleporting does not cross room gates. Update camera tracking explicitly.
            foreach (CinemachineCamera camera in FindObjectsByType<CinemachineCamera>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                camera.OnTargetObjectWarped(transform, positionDelta);
            }

            if (_respawnPoint != null &&
                (position - _respawnPoint.transform.position).sqrMagnitude < 0.0001f)
            {
                _respawnPoint.RestoreView(transform);
            }
        }

        public void RestartAt(Vector3 position)
        {
            _respawnPoint?.PrepareForRetry();
            MoveToCheckpoint(position);
            GetComponent<global::Health>().ReviveFull();
            GetComponent<PlayerHit>()?.ResetForRespawn();
            GetComponent<PlayerDash>()?.ResetForRespawn();
            GetComponent<PlayerAttack>()?.ResetForRespawn();
            GetComponent<PlayerController>()?.ResetForRespawn();
            OnRestarted?.Invoke();
        }

        public void SetRespawnPoint(Vector3 position)
        {
            _respawnPosition = position;
            _respawnPoint = null;
        }

        public void SetRespawnPoint(RespawnPoint point)
        {
            if (point == null) return;
            _respawnPoint = point;
            _respawnPosition = point.transform.position;
        }

        [ContextMenu("Respone Test/Take 100 Damage")]
        private void TestTakeDamage()
        {
            if (Application.isPlaying) GetComponent<global::Health>().TakeDamage(100);
        }

        [ContextMenu("Respone Test/Die")]
        private void TestDie()
        {
            if (Application.isPlaying)
                GetComponent<global::Health>().TakeDamage(int.MaxValue);
        }

        [ContextMenu("Respone Test/Return With Damage")]
        private void TestReturn()
        {
            if (Application.isPlaying && global::Respone.Instance != null)
                global::Respone.Instance.ResponePlayer(this, _respawnPosition);
        }

        [ContextMenu("Respone Test/Restart At Checkpoint")]
        public void RestartAtCheckpoint()
        {
            if (Application.isPlaying) RestartAt(_respawnPosition);
        }
    }
}