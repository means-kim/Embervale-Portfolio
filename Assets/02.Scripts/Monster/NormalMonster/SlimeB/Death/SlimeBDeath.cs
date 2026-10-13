using UnityEngine;
using System.Collections;

public class SlimeBDeath : MonoBehaviour
{
    [SerializeField] private float _deathAnimationTime = 0.5f;
    [SerializeField] private float _fadeDuration = 0.4f;

    private Health _health;
    private Rigidbody2D _rigidbody;
    private PolygonCollider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private bool _deathStarted;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<PolygonCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void BeginDeath()
    {
        if (!_health.IsDead) return;

        _collider.enabled = false;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0;
        _rigidbody.simulated = false;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // 사망 애니메이션이 재생될 시간을 기다립니다.
        yield return new WaitForSeconds(_deathAnimationTime);

        float startAlpha = _spriteRenderer.color.a;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;

            Color color = _spriteRenderer.color;
            color.a = Mathf.Lerp(
                startAlpha,
                0f,
                Mathf.Clamp01(elapsed / _fadeDuration)
            );

            _spriteRenderer.color = color;
            yield return null;
        }

        Destroy(gameObject);
    }
}