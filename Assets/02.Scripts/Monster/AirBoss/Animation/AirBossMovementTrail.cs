using System;
using UnityEngine;

// Owned by AirBossPhaseAura so visibility and phase changes share one lifecycle.
internal sealed class AirBossMovementTrail
{
    [Serializable]
    public sealed class Settings
    {
        public bool enabled = true;
        [Min(0f)] public float minimumSpeed = 2f;
        [Min(0.1f)] public float fullSpeed = 18f;
        [Min(0.1f)] public float spacing = 0.65f;
        public Vector2 lifetime = new Vector2(0.4f, 0.8f);
        [Range(0f, 1f)] public float opacity = 0.45f;
        [Min(0.1f)] public float size = 7f;
    }

    private readonly Transform _anchor;
    private readonly SpriteRenderer _boss;
    private readonly ParticleSystem _mist;
    private readonly ParticleSystem _embers;
    private readonly LineRenderer[] _sparks = new LineRenderer[8];
    private readonly float[] _sparkEnd = new float[8];
    private readonly float[] _sparkLife = new float[8];
    private readonly float[] _sparkAlpha = new float[8];
    private readonly System.Random _random = new System.Random(9237);
    private float _distance;
    private int _nextSpark;

    public AirBossMovementTrail(Transform root, Transform anchor, SpriteRenderer boss, Material mist, Material soul, Material electric)
    {
        _anchor = anchor;
        _boss = boss;
        _mist = MakeParticles(root, "Movement Mist", mist, 64);
        _embers = MakeParticles(root, "Movement Embers", soul, 32);
        for (int i = 0; i < _sparks.Length; i++)
        {
            var line = new GameObject("Movement Spark " + i, typeof(LineRenderer)).GetComponent<LineRenderer>();
            line.transform.SetParent(root, false);
            line.sharedMaterial = electric;
            line.useWorldSpace = true;
            line.positionCount = 5;
            line.numCapVertices = 1;
            line.enabled = false;
            line.sortingLayerID = boss.sortingLayerID;
            line.sortingOrder = boss.sortingOrder - 1;
            _sparks[i] = line;
        }
    }

    private ParticleSystem MakeParticles(Transform root, string name, Material material, int limit)
    {
        var system = new GameObject(name, typeof(ParticleSystem)).GetComponent<ParticleSystem>();
        system.transform.SetParent(root, false);
        system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = system.main;
        main.playOnAwake = false;
        main.maxParticles = limit;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.scalingMode = ParticleSystemScalingMode.Shape;
        main.startSpeed = 0f;
        var emission = system.emission;
        emission.enabled = false;
        var shape = system.shape;
        shape.enabled = false;
        var color = system.colorOverLifetime;
        color.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.12f), new GradientAlphaKey(0f, 1f) });
        color.color = gradient;
        var size = system.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0.75f, 1f, 1.35f));
        var noise = system.noise;
        noise.enabled = true;
        noise.strength = 0.3f;
        noise.frequency = 0.4f;
        noise.scrollSpeed = 0.2f;
        var renderer = system.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = material;
        renderer.sortingLayerID = _boss.sortingLayerID;
        renderer.sortingOrder = _boss.sortingOrder - 1;
        renderer.maxParticleSize = 1f;
        return system;
    }

    public void Play()
    {
        _mist.Play();
        _embers.Play();
    }

    public void Clear()
    {
        _mist.Clear();
        _embers.Clear();
        _distance = 0f;
        for (int i = 0; i < _sparks.Length; i++)
        {
            _sparks[i].enabled = false;
            _sparkEnd[i] = 0f;
        }
    }

    public void Stop()
    {
        Clear();
        _mist.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _embers.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void Tick(Vector3 previous, int phase, Vector2 radius, Vector2 center, Settings settings)
    {
        if (!settings.enabled)
        {
            Clear();
            return;
        }

        for (int i = 0; i < _sparks.Length; i++)
        {
            float remaining = _sparkEnd[i] - Time.time;
            _sparks[i].enabled = remaining > 0f;
            if (remaining <= 0f) continue;
            var color = new Color(0.48f, 0.72f, 1f, _sparkAlpha[i] * Mathf.Clamp01(remaining / _sparkLife[i]) * _boss.color.a);
            _sparks[i].startColor = _sparks[i].endColor = color;
        }

        Vector3 delta = _anchor.position - previous;
        delta.z = 0f;
        float distance = delta.magnitude;
        if (Time.deltaTime <= 0f) return;
        float speed = distance / Time.deltaTime;
        if (distance < 0.0001f || speed < settings.minimumSpeed)
        {
            _distance = 0f;
            return;
        }

        float strength = Mathf.InverseLerp(settings.minimumSpeed, Mathf.Max(settings.minimumSpeed + 0.1f, settings.fullSpeed), speed);
        float spacing = Mathf.Max(0.1f, settings.spacing) * Mathf.Lerp(1.7f, 0.7f, strength);
        Vector3 direction = delta / distance;
        Vector3 side = new Vector3(-direction.y, direction.x, 0f);
        float scale = Mathf.Max(Mathf.Abs(_anchor.lossyScale.x), 0.01f);
        Vector3 origin = _anchor.TransformPoint(center) - _anchor.position;
        float rear = Mathf.Min(radius.x, radius.y) * scale * 0.55f;
        _distance = Mathf.Min(_distance, spacing);
        float offset = spacing - _distance;
        int emitted = 0;
        // Interpolate along the travelled segment; cap catch-up work after a slow frame.
        for (; offset <= distance && emitted < 6; offset += spacing)
        {
            Vector3 position = previous + direction * offset + origin - direction * rear + side * Range(-rear * 0.6f, rear * 0.6f);
            float life = Mathf.Lerp(Mathf.Max(0.05f, settings.lifetime.x), Mathf.Max(0.05f, settings.lifetime.y), strength);
            float alpha = settings.opacity * Mathf.Lerp(0.4f, 1f, strength) * _boss.color.a;
            if (phase != 2)
            {
                _mist.Emit(new ParticleSystem.EmitParams
                {
                    position = position,
                    velocity = -direction * scale * 0.35f,
                    startLifetime = life,
                    startSize = settings.size * scale * Range(0.7f, 1.2f),
                    rotation = Range(0f, 360f),
                    startColor = new Color(0.7f, 0.9f, 1f, alpha)
                }, 1);
            }

            if (_random.NextDouble() < 0.35f)
            {
                _embers.Emit(new ParticleSystem.EmitParams
                {
                    position = position,
                    velocity = side * Range(-0.5f, 0.5f),
                    startLifetime = phase == 2 ? life * 0.5f : life,
                    startSize = scale * Range(0.4f, 1.1f),
                    startColor = new Color(0.55f, 0.8f, 1f, alpha)
                }, 1);
            }

            if (phase >= 2 && _random.NextDouble() < 0.45f)
            {
                int slot = _nextSpark++ % _sparks.Length;
                var line = _sparks[slot];
                for (int j = 0; j < 5; j++)
                {
                    line.SetPosition(j, position - direction * (j * scale * 0.6f) + side * Range(-0.3f, 0.3f) * scale);
                }

                line.widthMultiplier = 0.065f * scale;
                line.startColor = line.endColor = new Color(0.48f, 0.72f, 1f, alpha);
                line.enabled = true;
                _sparkLife[slot] = Range(0.1f, 0.22f);
                _sparkEnd[slot] = Time.time + _sparkLife[slot];
                _sparkAlpha[slot] = alpha;
            }

            emitted++;
        }

        _distance = emitted == 6 ? 0f : Mathf.Repeat(_distance + distance, spacing);
    }

    private float Range(float min, float max) => Mathf.Lerp(min, max, (float)_random.NextDouble());
}
