using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform _checkPointTransform;

    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Border"))
        {
            Debug.Log("보더 닿음");
            PlayerDeath();
        }
        else
        {
            Debug.Log(other.gameObject.name);
        }
    }

    private void PlayerDeath()
    {
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0f;
        transform.position = _checkPointTransform.position;
    }
}
