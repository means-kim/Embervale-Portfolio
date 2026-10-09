using UnityEngine;

public class OutGameFrostPiece : MonoBehaviour
{
    private Vector3 _direction;
    private float _speed;
    private float _lifetime;
    private float _elapsed;
    private SpriteRenderer _sr;
    private Color _startColor;
    private float _startScale;

    public void Setup(Vector3 direction, float speed, float lifetime, float scale)
    {
        _direction = direction;
        _speed = speed;
        _lifetime = lifetime;
        _elapsed = 0f;
        _startScale = scale;

        transform.localScale = Vector3.one * scale;
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        _sr = GetComponent<SpriteRenderer>();
        if (_sr != null)
        {
            _startColor = _sr.color;
        }
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;

        if (_elapsed >= _lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // 직진 이동
        transform.position += _direction * (_speed * Time.deltaTime);

        // 알파 페이드인(앞 15%) 후 페이드아웃 + 약간 커지기
        float t = _elapsed / _lifetime;
        if (_sr != null)
        {
            float fadeIn = Mathf.Clamp01(_elapsed / Mathf.Max(0.001f, _lifetime * 0.15f));
            Color c = _startColor;
            c.a = _startColor.a * fadeIn * (1f - t);
            _sr.color = c;
        }

        float growFactor = Mathf.Lerp(1f, 1.3f, t);
        transform.localScale = Vector3.one * (_startScale * growFactor);
    }
}
