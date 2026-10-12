using UnityEngine;

/// <summary>Visual-only aura. Does not alter attacks, health, movement or materials on the boss.</summary>
[DisallowMultipleComponent]
public sealed class AirBossDarkAura : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _boss;
    [SerializeField] private AirBossPhaseController _phase;
    [SerializeField] private Health _health;
    [SerializeField] private MeshRenderer _halo;
    [SerializeField] private ParticleSystem[] _particles;

    [Header("Appearance")]
    [SerializeField, Range(0f, 1f)] private float _opacity = 0.85f;

    [SerializeField] private Vector3 _haloSize = new Vector3(46f, 52f, 1f);
    [SerializeField] private Vector3 _haloOffset = new Vector3(0f, 2f, 0f);
    [SerializeField, Min(0f)] private float _followLag = 0.22f;
    [SerializeField, Range(0f, 0.5f)] private float _phaseExpansion = 0.09f;
    private MaterialPropertyBlock _block;
    private Vector3 _lastPosition;
    private Vector3 _lag;
    private float _fade;
    private bool _visible;
    private static readonly int Opacity = Shader.PropertyToID("_Opacity");

    private void OnEnable()
    {
        _block ??= new MaterialPropertyBlock();
        _lastPosition = transform.position;
        _lag = Vector3.zero;
        _fade = 0f;
        _visible = false;
        SetParticles(false);
        if (_halo != null) _halo.enabled = false;
    }

    private void LateUpdate()
    {
        if (_boss == null || _halo == null) return;
        bool visible = _boss.enabled && _boss.gameObject.activeInHierarchy && _boss.color.a > 0.01f;
        bool alive = _health == null || !_health.IsDead;
        bool emit = visible && alive;
        if (_visible != emit)
        {
            SetParticles(emit);
            _visible = emit;
        }

        _fade = Mathf.MoveTowards(_fade, emit ? 1f : 0f, Time.deltaTime * (emit ? 1.8f : 1.2f));
        // Hidden cinematic sprites must never leave a floating aura behind.
        if (!visible) _fade = 0f;
        _halo.enabled = visible && _fade > 0.001f;
        int phase = _phase == null ? 1 : _phase.CurrentPhase;
        float growth = 1f + (phase - 1) * _phaseExpansion;
        float breath = 1f + Mathf.Sin(Time.time * 1.1f) * 0.025f;
        Vector3 delta = transform.position - _lastPosition;
        _lastPosition = transform.position;
        // Clamp teleports/cinematic relocations, rather than stretching across the arena.
        _lag = delta.sqrMagnitude > 400f
            ? Vector3.zero
            : Vector3.Lerp(_lag - delta, Vector3.zero, 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.01f, _followLag)));
        _lag = Vector3.ClampMagnitude(_lag, 3f);
        _halo.transform.localPosition = _haloOffset + transform.InverseTransformVector(_lag);
        _halo.transform.localScale = Vector3.Scale(_haloSize, new Vector3(growth * breath, growth * breath, 1f));
        _halo.sortingLayerID = _boss.sortingLayerID;
        _halo.sortingOrder = _boss.sortingOrder - 3;
        _halo.GetPropertyBlock(_block);
        _block.SetFloat(Opacity, _opacity * _fade * _boss.color.a * (1f + 0.08f * (phase - 1)));
        _halo.SetPropertyBlock(_block);
    }

    private void SetParticles(bool play)
    {
        if (_particles == null) return;
        foreach (var particle in _particles)
        {
            if (particle == null) continue;
            if (play) particle.Play(true);
            else particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void OnDisable()
    {
        SetParticles(false);
        if (_halo != null) _halo.enabled = false;
        _visible = false;
    }
}