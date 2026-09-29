using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private Transform _checkPointTransform;
    private Rigidbody2D _rigidbody2D;
    private Vector3 _respawnPosition;
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
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0f;
        transform.position = _respawnPosition;
    }
    public void SetRespawnPoint(Vector3 position) => _respawnPosition = position;
}
