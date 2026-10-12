using UnityEngine;

// Cosmetic pieces have no colliders or damage and cannot block the newly opened route.
public sealed class CeilingRockFragment : MonoBehaviour
{
    private Mesh _mesh;
    private Vector2 _velocity;
    private float _spin;
    private float _lifetime;
    private float _age;
    private Color[] _colors;

    public void Initialize(Mesh mesh, Vector2 velocity, float spin, float lifetime)
    {
        _mesh = mesh;
        _colors = mesh.colors;
        _velocity = velocity;
        _spin = spin;
        _lifetime = lifetime;
    }

    private void Update()
    {
        _age += Time.deltaTime;
        if (_age >= _lifetime)
        {
            Destroy(gameObject);
            return;
        }

        _velocity.y -= 25f * Time.deltaTime;
        transform.position += (Vector3)_velocity * Time.deltaTime;
        transform.Rotate(0, 0, _spin * Time.deltaTime);
        float alpha = Mathf.Clamp01((_lifetime - _age) / 0.45f);
        for (int i = 0; i < _colors.Length; i++)
        {
            _colors[i].a = alpha;
        }

        _mesh.colors = _colors;
    }

    private void OnDestroy()
    {
        if (_mesh != null)
        {
            Destroy(_mesh);
        }
    }
}