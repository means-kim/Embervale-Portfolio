using UnityEngine;

/// <summary>Body-perimeter emission with world-space souls; no hand tracking or combat logic.</summary>
[DisallowMultipleComponent]
public sealed class AirBossPhaseAura : MonoBehaviour
{
    [Header("Assets")]
    [SerializeField] private Material _soulMaterial;

    [SerializeField] private Material _arcMaterial;
    [SerializeField] private Material _mistMaterial;

    [Header("Body perimeter (boss local units)")]
    [SerializeField] private Vector2 _radius = new Vector2(16f, 19f);

    [SerializeField] private Vector2 _center = new Vector2(0f, 2f);
    [SerializeField, Range(0.4f, 4f)] private float _soulSize = 1.8f;
    [SerializeField] private Vector2 _soulSpawnGap = new Vector2(0.18f, 0.42f);
    [SerializeField, Min(1f)] private float _teleportClearDistance = 12f;
    [SerializeField, Range(0f, 1f)] private float _frostOpacity = 0.8f;

    [Header("Intermittent electricity")]
    [SerializeField, Range(0.03f, 0.3f)] private float _arcWidth = 0.12f;

    [SerializeField] private Vector2 _flashDuration = new Vector2(0.09f, 0.19f);
    [SerializeField] private Vector2 _flashGap = new Vector2(0.45f, 1.15f);
    [SerializeField, Range(0f, 1f)] private float _electricOpacity = 0.85f;

    [Header("Movement trail (world speed / distance)")]
    [SerializeField] private AirBossMovementTrail.Settings _movementTrail = new AirBossMovementTrail.Settings();

    private AirBossMovementTrail _trail;

    private SpriteRenderer _boss;
    private AirBossPhaseController _phase;
    private Health _health;
    private BossPhase2Cinematic _phase2Cinematic;
    private Transform _visualRoot;
    private float[] _seeds, _sizes;
    private Vector3[] _nodes;
    private ParticleSystem _souls, _motes;
    private Vector3 _previousPosition;
    private float _nextSoul, _nextMote;
    private ParticleSystem _mist;
    private Arc[] _arcs;
    private readonly System.Random _random = new System.Random(7349);
    private float _frost, _electric, _nextMist;
    private int _lastPhase;
    private bool _wasVisible;

    private sealed class Arc
    {
        public LineRenderer glow, core, branch;
        public readonly Vector3[] points = new Vector3[13];
        public readonly Vector3[] fork = new Vector3[4];
        public int from, to;
        public float until, next, redraw;
    }

    private void Awake()
    {
        _boss = GetComponentInParent<SpriteRenderer>();
        _phase = GetComponentInParent<AirBossPhaseController>();
        _health = GetComponentInParent<Health>();
        _phase2Cinematic = FindFirstObjectByType<BossPhase2Cinematic>(FindObjectsInactive.Include);
        if (_boss == null || _soulMaterial == null || _arcMaterial == null || _mistMaterial == null)
        {
            Debug.LogWarning("Phase aura is missing its boss or visual assets.", this);
            enabled = false;
            return;
        }

        _visualRoot = new GameObject("Runtime Phase Effects").transform;
        _visualRoot.SetParent(transform, false);
        const int count = 12;
        _seeds = new float[count];
        _sizes = new float[count];
        _nodes = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            _seeds[i] = Range(1f, 900f);
            _sizes[i] = Range(0.65f, 1.35f);
        }

        _souls = MakeSoulParticles("Blue Souls", 32, false);
        _motes = MakeSoulParticles("Soul Motes", 64, true);
        _arcs = new Arc[5];
        for (int i = 0; i < _arcs.Length; i++)
        {
            _arcs[i] = new Arc
            {
                glow = MakeLine("Arc Glow " + i),
                core = MakeLine("Arc Core " + i),
                branch = MakeLine("Arc Branch " + i),
                next = Time.time + i * 0.17f
            };
        }

        MakeMist();
        _trail = new AirBossMovementTrail(_visualRoot, transform, _boss, _mistMaterial, _soulMaterial, _arcMaterial);
        Hide();
    }

    private LineRenderer MakeLine(string name)
    {
        var go = new GameObject(name, typeof(LineRenderer));
        go.transform.SetParent(_visualRoot, false);
        var line = go.GetComponent<LineRenderer>();
        line.sharedMaterial = _arcMaterial;
        line.useWorldSpace = false;
        line.alignment = LineAlignment.View;
        line.numCornerVertices = 1;
        line.numCapVertices = 1;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.enabled = false;
        return line;
    }

    private ParticleSystem MakeSoulParticles(string name, int limit, bool motes)
    {
        var go = new GameObject(name, typeof(ParticleSystem));
        go.transform.SetParent(_visualRoot, false);
        var system = go.GetComponent<ParticleSystem>();
        system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = system.main;
        main.playOnAwake = false;
        main.maxParticles = limit;
        main.startSpeed = 0f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.scalingMode = ParticleSystemScalingMode.Shape;
        var emission = system.emission;
        emission.enabled = false;
        var shape = system.shape;
        shape.enabled = false;
        var noise = system.noise;
        noise.enabled = true;
        noise.strength = motes ? 0.22f : 0.45f;
        noise.frequency = 0.4f;
        noise.scrollSpeed = 0.22f;
        var color = system.colorOverLifetime;
        color.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(0.35f, 0.65f, 1f), 1f) },
            new[]
            {
                new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.2f), new GradientAlphaKey(0.65f, 0.6f),
                new GradientAlphaKey(0f, 1f)
            });
        color.color = gradient;
        var size = system.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0.65f, 1f, 1.25f));
        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = _soulMaterial;
        renderer.sortingLayerID = _boss.sortingLayerID;
        renderer.sortingOrder = _boss.sortingOrder - 1;
        renderer.maxParticleSize = 1f;
        return system;
    }

    private void EmitSoul(Vector3 localPosition, bool mote, bool charged = false)
    {
        float scale = Mathf.Max(Mathf.Abs(transform.lossyScale.x), 0.01f);
        var system = mote ? _motes : _souls;
        system.Emit(new ParticleSystem.EmitParams
        {
            position = transform.TransformPoint(localPosition),
            velocity = new Vector3(Range(-0.55f, 0.55f), Range(0.65f, 1.8f), 0f) * scale,
            startSize = (mote ? Range(0.15f, 0.4f) : _soulSize * Range(0.7f, 1.4f)) * scale,
            startLifetime = mote ? Range(1.1f, 2.2f) : Range(1.6f, 3.2f),
            rotation = Range(-35f, 35f),
            startColor = new Color(charged ? 0.85f : 0.5f, 0.8f, 1f, _frost * _frostOpacity * _boss.color.a)
        }, 1);
    }

    private void MakeMist()
    {
        var go = new GameObject("Drifting Frost", typeof(ParticleSystem));
        go.transform.SetParent(_visualRoot, false);
        _mist = go.GetComponent<ParticleSystem>();
        _mist.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = _mist.main;
        main.playOnAwake = false;
        main.loop = true;
        main.maxParticles = 48;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1.8f, 3f);
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(4f, 8f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.scalingMode = ParticleSystemScalingMode.Shape;
        var emission = _mist.emission;
        emission.enabled = false;
        var shape = _mist.shape;
        shape.enabled = false;
        var velocity = _mist.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.y = 0.5f;
        var noise = _mist.noise;
        noise.enabled = true;
        noise.strength = 0.35f;
        noise.frequency = 0.35f;
        noise.scrollSpeed = 0.15f;
        var color = _mist.colorOverLifetime;
        color.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.2f, 0.25f), new GradientAlphaKey(0f, 1f) });
        color.color = gradient;
        var size = _mist.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0.6f, 1f, 1.6f));
        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = _mistMaterial;
        renderer.sortingLayerID = _boss.sortingLayerID;
        renderer.sortingOrder = _boss.sortingOrder - 1;
        renderer.maxParticleSize = 1f;
    }

    private void LateUpdate()
    {
        if (_visualRoot == null) return;
        // CurrentPhase remains 1 throughout the phase-2 cinematic. Suppress from
        // the transition request itself, including sprite-swap disappearance clips.
        bool transitioning = (_phase != null && _phase.IsTransition) ||
                             (_phase2Cinematic != null && _phase2Cinematic.IsPlaying);
        bool visible = !transitioning && _boss.enabled && _boss.sprite != null && _boss.gameObject.activeInHierarchy &&
                       _boss.color.a > 0.01f && (_health == null || !_health.IsDead);
        if (!visible)
        {
            if (_wasVisible) Hide();
            return;
        }

        if (!_wasVisible)
        {
            _visualRoot.gameObject.SetActive(true);
            _mist.Play();
            _souls.Play();
            _motes.Play();
            _trail.Play();
            _previousPosition = transform.position;
            _wasVisible = true;
        }

        if (Vector3.Distance(transform.position, _previousPosition) > _teleportClearDistance)
        {
            _mist.Clear();
            _souls.Clear();
            _motes.Clear();
            _trail.Clear();
            _previousPosition = transform.position;
        }

        Vector3 previousPosition = _previousPosition;
        _previousPosition = transform.position;
        int phase = _phase == null ? 1 : _phase.CurrentPhase;
        bool frost = phase != 2, electricity = phase >= 2;
        if (phase != _lastPhase)
        {
            _lastPhase = phase;
            _trail.Clear();
            if (!frost)
            {
                _mist.Clear();
                _souls.Clear();
                _motes.Clear();
            }

            foreach (var arc in _arcs)
            {
                arc.until = 0f;
                arc.next = Time.time + Range(0.05f, 0.5f);
            }
        }

        _frost = Mathf.MoveTowards(_frost, frost ? 1f : 0f, Time.deltaTime * 2f);
        _electric = Mathf.MoveTowards(_electric, electricity ? 1f : 0f, Time.deltaTime * 3f);
        _trail.Tick(previousPosition, phase, _radius, _center, _movementTrail);
        for (int i = 0; i < _nodes.Length; i++)
        {
            float seed = _seeds[i];
            float clock = Time.time * (0.1f + _sizes[i] * 0.07f);
            float angle = i * Mathf.PI * 2f / _nodes.Length + (Mathf.PerlinNoise(seed, clock) - 0.5f) * 1.8f;
            float radial = 0.68f + Mathf.PerlinNoise(seed + 19f, clock * 0.73f) * 0.48f;
            _nodes[i] = new Vector3(_center.x + Mathf.Cos(angle) * _radius.x * radial,
                _center.y + Mathf.Sin(angle) * _radius.y * radial + (Mathf.PerlinNoise(seed + 47f, clock) - 0.5f) * 3f,
                0f);
        }

        foreach (var arc in _arcs)
        {
            UpdateArc(arc, electricity, phase == 3);
        }

        if (frost && Time.time >= _nextSoul)
        {
            _nextSoul = Time.time + Range(_soulSpawnGap.x, _soulSpawnGap.y);
            EmitSoul(_nodes[_random.Next(_nodes.Length)], false);
        }

        if (frost && Time.time >= _nextMote)
        {
            _nextMote = Time.time + Range(0.06f, 0.15f);
            EmitSoul(_nodes[_random.Next(_nodes.Length)] + new Vector3(Range(-2f, 2f), Range(-2f, 2f)), true);
        }

        if (frost && Time.time >= _nextMist)
        {
            _nextMist = Time.time + Range(0.3f, 0.65f);
            float scale = Mathf.Max(Mathf.Abs(transform.lossyScale.x), 0.01f);
            _mist.Emit(new ParticleSystem.EmitParams
            {
                position = transform.TransformPoint(_nodes[_random.Next(_nodes.Length)]),
                startSize = Range(4f, 8f) * scale,
                startColor = new Color(1f, 1f, 1f, _frost * _boss.color.a)
            }, 1);
        }
    }

    private void UpdateArc(Arc arc, bool electricity, bool hybrid)
    {
        if (electricity && Time.time >= arc.next)
        {
            arc.from = _random.Next(_nodes.Length);
            // Avoid stacking multiple flashes on the same pair of emission points.
            for (int attempt = 0; attempt < _nodes.Length; attempt++)
            {
                bool occupied = false;
                foreach (var other in _arcs)
                {
                    if (other != arc && Time.time < other.until && other.from == arc.from)
                    {
                        occupied = true;
                        break;
                    }
                }

                if (!occupied) break;
                arc.from = (arc.from + 1) % _nodes.Length;
            }

            arc.to = (arc.from + 1) % _nodes.Length;
            arc.until = Time.time + Range(_flashDuration.x, _flashDuration.y);
            arc.next = arc.until + Range(_flashGap.x, _flashGap.y);
            arc.redraw = 0f;
            if (hybrid)
            {
                EmitSoul(_nodes[arc.from], false, true);
                EmitSoul(_nodes[arc.to], true, true);
            }
        }

        bool active = electricity && Time.time < arc.until;
        arc.glow.enabled = arc.core.enabled = arc.branch.enabled = active;
        if (!active) return;
        if (Time.time >= arc.redraw)
        {
            arc.redraw = Time.time + 0.045f;
            Vector3 from = _nodes[arc.from], to = _nodes[arc.to];
            Vector3 tangent = (to - from).normalized;
            Vector3 normal = new Vector3(-tangent.y, tangent.x);
            for (int i = 0; i < arc.points.Length; i++)
            {
                float t = (float)i / (arc.points.Length - 1);
                arc.points[i] = Vector3.Lerp(from, to, t) +
                                normal * (i == 0 || i == arc.points.Length - 1 ? 0f : Range(-0.7f, 0.7f));
            }

            for (int i = 0; i < arc.fork.Length; i++)
            {
                arc.fork[i] = arc.points[6] + normal * (i * 0.7f) + tangent * (i == 0 ? 0f : Range(-0.6f, 0.6f));
            }

            arc.glow.positionCount = arc.core.positionCount = arc.points.Length;
            arc.branch.positionCount = arc.fork.Length;
            arc.glow.SetPositions(arc.points);
            arc.core.SetPositions(arc.points);
            arc.branch.SetPositions(arc.fork);
        }

        float alpha = _electric * _electricOpacity * _boss.color.a;
        float scale = Mathf.Max(Mathf.Abs(transform.lossyScale.x), 0.01f);
        SetLine(arc.glow, new Color(0.32f, 0.32f, 1f, alpha * 0.3f), _arcWidth * scale * 4f);
        SetLine(arc.core, new Color(0.72f, 0.86f, 1f, alpha), _arcWidth * scale);
        SetLine(arc.branch, new Color(0.48f, 0.65f, 1f, alpha * 0.65f), _arcWidth * scale * 0.6f);
    }

    private void SetLine(LineRenderer line, Color color, float width)
    {
        line.startColor = line.endColor = color;
        line.widthMultiplier = width;
        line.sortingLayerID = _boss.sortingLayerID;
        line.sortingOrder = _boss.sortingOrder - 1;
    }

    private float Range(float min, float max) => Mathf.Lerp(min, max, (float)_random.NextDouble());

    private void Hide()
    {
        if (_visualRoot == null) return;
        _trail.Stop();
        _mist.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        foreach (var arc in _arcs)
        {
            arc.glow.enabled = arc.core.enabled = arc.branch.enabled = false;
            arc.until = 0f;
            arc.next = Time.time + Range(0.05f, 0.5f);
        }

        _souls.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _motes.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _visualRoot.gameObject.SetActive(false);
        _frost = _electric = 0f;
        _wasVisible = false;
    }

    private void OnEnable()
    {
        if (_phase != null) _phase.TransitionRequested += OnTransitionRequested;
    }

    private void OnTransitionRequested(int nextPhase) => Hide();

    private void OnDisable()
    {
        if (_phase != null) _phase.TransitionRequested -= OnTransitionRequested;
        Hide();
    }
}
