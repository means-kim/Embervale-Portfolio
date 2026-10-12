using System.Collections.Generic;
using UnityEngine;

// Attach to the ceiling's blocking collider. Player attack triggers supply the hits.
public sealed class AirComboBreakableCeiling : MonoBehaviour
{
    [SerializeField, Min(1)] private int _requiredHits = 6;
    [SerializeField] private GameObject _visualRoot;
    [SerializeField] private Collider2D[] _blockingColliders;
    [SerializeField] private SpriteRenderer _rock;
    [SerializeField] private LineRenderer[] _cracks;
    [Header("Rock impact feedback")]
    [SerializeField] private Material _dustMaterial;
    [SerializeField, Min(0f)] private float _hitShake = 0.18f;
    [SerializeField, Min(0.01f)] private float _impactDuration = 0.16f;
    [SerializeField, Min(0f)] private float _breakShake = 0.4f;
    [SerializeField, Min(2)] private int _fragmentCount = 9;
    private readonly HashSet<PlayerAttackInstance> _countedAttacks = new();
    private PlayerJump _player;
    private Health _playerHealth;
    private Color _baseColor;
    private float _flashTime;
    private Vector3 _restPosition;
    private float _shakeStrength;
    private float _breakDelay = -1f;
    private MaterialPropertyBlock _crackMask;
    public int HitCount { get; private set; }
    public int RequiredHits => _requiredHits;
    public bool IsBroken { get; private set; }

    private void Awake()
    {
        if (_rock != null) _baseColor = _rock.color;
        if (_visualRoot != null) _restPosition = _visualRoot.transform.localPosition;
        UpdateCracks();
    }

    private void OnDisable()
    {
        UnbindPlayer();
        RestoreVisual();
    }

    private void Update()
    {
        if (_breakDelay >= 0f)
        {
            _breakDelay -= Time.unscaledDeltaTime;
            if (_breakDelay <= 0f)
            {
                _breakDelay = -1f;
                Shatter();
            }
        }

        if (_rock == null || _flashTime <= 0f) return;
        _flashTime = Mathf.Max(0f, _flashTime - Time.unscaledDeltaTime);
        float t = _flashTime / _impactDuration;
        _rock.color = Color.Lerp(_baseColor, new Color(1.5f, 1.6f, 1.75f, _baseColor.a), t);
        if (_visualRoot != null)
        {
            var scale = _visualRoot.transform.parent.lossyScale;
            var offset = new Vector3(Mathf.Sin(Time.unscaledTime * 97f), Mathf.Cos(Time.unscaledTime * 113f), 0) *
                         (_shakeStrength * t);
            _visualRoot.transform.localPosition =
                _restPosition + new Vector3(offset.x / scale.x, offset.y / scale.y, 0);
        }

        if (_flashTime <= 0f) RestoreVisual();
        UpdateCrackMask();
    }

    private void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    private void OnTriggerStay2D(Collider2D other) => TryHit(other);

    public bool TryHit(Collider2D other)
    {
        if (IsBroken || other == null)
        {
            return false;
        }

        var attack = other.GetComponentInParent<PlayerAttackInstance>();
        if (attack == null || attack.Owner == null)
        {
            return false;
        }

        var hitbox = attack.GetComponent<PlayerDamageHitbox>();
        if (hitbox == null || !hitbox.IsActive)
        {
            return false;
        }

        var player = attack.Owner.GetComponentInParent<PlayerJump>();
        if (player == null || player.IsGrounded)
        {
            return false;
        }

        var health = player.GetComponent<Health>();
        if (health != null && health.IsDead)
        {
            return false;
        }

        _countedAttacks.RemoveWhere(a => a == null);
        if (!_countedAttacks.Add(attack))
        {
            return false;
        }

        if (_player != player)
        {
            ResetUnfinishedAttempt();
            BindPlayer(player, health);
        }

        HitCount++;
        _flashTime = _impactDuration;
        _shakeStrength = _hitShake * (1f + HitCount * 0.1f);
        UpdateCracks();
        var bounds = GetComponent<Collider2D>().bounds;
        var impact = new Vector3(Mathf.Clamp(other.bounds.center.x, bounds.min.x, bounds.max.x), bounds.min.y + 0.1f,
            bounds.center.z);
        SpawnDust(impact, 14, 1f);
        SpawnChips(impact, 4);
        CameraManager.ShakeCamera(0.09f, 0.1f);
        HitImpactFeedback.EnsureInstance().RequestHitstop(0.025f);
        if (HitCount >= _requiredHits)
        {
            Break();
        }

        return true;
    }

    private void BindPlayer(PlayerJump player, Health health)
    {
        UnbindPlayer();
        _player = player;
        _playerHealth = health;
        _player.OnLanded += ResetUnfinishedAttempt;
        if (_playerHealth != null)
        {
            _playerHealth.OnDie += ResetUnfinishedAttempt;
        }
    }

    private void UnbindPlayer()
    {
        if (_player != null)
        {
            _player.OnLanded -= ResetUnfinishedAttempt;
        }

        if (_playerHealth != null)
        {
            _playerHealth.OnDie -= ResetUnfinishedAttempt;
        }

        _player = null;
        _playerHealth = null;
    }

    private void ResetUnfinishedAttempt()
    {
        if (IsBroken) return;
        HitCount = 0;
        _flashTime = 0f;
        RestoreVisual();
        UpdateCracks();
    }

    private void UpdateCracks()
    {
        if (_cracks == null) return;
        for (int i = 0; i < _cracks.Length; i++)
        {
            if (_cracks[i] != null)
            {
                int threshold = 1 + i * Mathf.Max(1, _requiredHits - 1) / _cracks.Length;
                _cracks[i].enabled = HitCount >= threshold;
                _cracks[i].startWidth = _cracks[i].endWidth = 0.09f + HitCount * 0.018f;
            }
        }

        UpdateCrackMask();
    }

    private void UpdateCrackMask()
    {
        if (_rock == null || _rock.sprite == null || _cracks == null)
        {
            return;
        }

        if (_crackMask == null)
        {
            _crackMask = new MaterialPropertyBlock();
        }

        var bounds = _rock.bounds;
        var rect = _rock.sprite.rect;
        var texture = _rock.sprite.texture;
        foreach (var line in _cracks)
        {
            if (line == null)
            {
                continue;
            }

            line.GetPropertyBlock(_crackMask);
            _crackMask.SetTexture("_MainTex", texture);
            _crackMask.SetVector("_RockBounds",
                new Vector4(bounds.min.x, bounds.min.y, 1f / bounds.size.x, 1f / bounds.size.y));
            _crackMask.SetVector("_RockUVRect",
                new Vector4(rect.x / texture.width, rect.y / texture.height, rect.width / texture.width,
                    rect.height / texture.height));
            line.SetPropertyBlock(_crackMask);
        }
    }

    private void Break()
    {
        IsBroken = true;
        if (_blockingColliders != null)
        {
            foreach (var collider in _blockingColliders)
            {
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
        }

        _flashTime = _impactDuration;
        _shakeStrength = _breakShake;
        _breakDelay = 0.1f;
        CameraManager.ShakeCamera(0.35f, 0.22f);
        HitImpactFeedback.EnsureInstance().RequestHitstop(0.065f);
        UnbindPlayer();
    }

    private void RestoreVisual()
    {
        if (_rock != null)
        {
            _rock.color = _baseColor;
        }

        if (_visualRoot != null)
        {
            _visualRoot.transform.localPosition = _restPosition;
        }
    }

    private void Shatter()
    {
        if (_rock == null) return;
        RestoreVisual();
        var bounds = _rock.bounds;
        for (int i = 0; i < _fragmentCount; i++)
        {
            SpawnFragment((float)i / _fragmentCount, (float)(i + 1) / _fragmentCount, i,
                bounds.center, 1f, new Vector2((i / (float)(_fragmentCount - 1) - 0.5f) * 10f, Random.Range(1f, 4f)),
                1.7f);
        }

        for (int i = 0; i < 7; i++)
        {
            SpawnDust(new Vector3(Mathf.Lerp(bounds.min.x, bounds.max.x, i / 6f), bounds.center.y, bounds.center.z), 12,
                2.4f);
        }

        if (_visualRoot != null)
        {
            _visualRoot.SetActive(false);
        }
    }

    private void SpawnChips(Vector3 impact, int count)
    {
        if (_rock == null) return;
        for (int i = 0; i < count; i++)
        {
            float left = Random.Range(0.1f, 0.8f);
            SpawnFragment(left, left + 0.12f, i + 31, impact, Random.Range(0.09f, 0.18f),
                new Vector2(Random.Range(-3.5f, 3.5f), Random.Range(-1f, 2f)), 0.65f);
        }
    }

    private void SpawnFragment(float left, float right, int seed, Vector3 origin, float scale, Vector2 velocity,
        float lifetime)
    {
        var sprite = _rock.sprite;
        var rect = sprite.rect;
        var bounds = _rock.bounds;
        float middle = (left + right) * 0.5f;
        var vertices = new Vector3[8];
        var uv = new Vector2[8];
        var colors = new Color[8];
        for (int row = 0; row < 4; row++)
        {
            float y = row / 3f;
            for (int edge = 0; edge < 2; edge++)
            {
                float boundary = edge == 0 ? left : right;
                float x = Mathf.Clamp01(boundary + Mathf.Sin(boundary * 53f + row * 2.7f) * 0.012f);
                int n = row * 2 + edge;
                vertices[n] = new Vector3((x - middle) * bounds.size.x, (y - 0.5f) * bounds.size.y, 0);
                uv[n] = new Vector2((rect.x + x * rect.width) / sprite.texture.width,
                    (rect.y + y * rect.height) / sprite.texture.height);
                colors[n] = _baseColor;
            }
        }

        var mesh = new Mesh { name = "Ceiling fragment" };
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.colors = colors;
        mesh.triangles = new[] { 0, 1, 2, 1, 3, 2, 2, 3, 4, 3, 5, 4, 4, 5, 6, 5, 7, 6 };
        mesh.RecalculateBounds();
        var piece = new GameObject("Ceiling_Fragment");
        piece.transform.position = scale < 1f ? origin : origin + new Vector3((middle - 0.5f) * bounds.size.x, 0, 0);
        piece.transform.localScale = Vector3.one * scale;
        piece.AddComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = piece.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = _rock.sharedMaterial;
        renderer.sortingLayerID = _rock.sortingLayerID;
        renderer.sortingOrder = _rock.sortingOrder + 2;
        piece.AddComponent<CeilingRockFragment>().Initialize(mesh, velocity, Random.Range(-110f, 110f), lifetime);
    }

    private void SpawnDust(Vector3 position, int count, float size)
    {
        if (_dustMaterial == null) return;
        var go = new GameObject("Ceiling_StoneDust");
        go.transform.position = position;
        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.loop = false;
        main.duration = 0.15f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.35f, 0.8f);
        main.startSize = new ParticleSystem.MinMaxCurve(size * 0.25f, size * 0.65f);
        main.startColor = new Color(0.3f, 0.38f, 0.47f, 0.6f);
        main.gravityModifier = 0.4f;
        main.stopAction = ParticleSystemStopAction.Destroy;
        var emission = ps.emission;
        emission.enabled = false;
        var shape = ps.shape;
        shape.enabled = false;
        var fade = ps.colorOverLifetime;
        fade.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) },
            new[] { new GradientAlphaKey(0.6f, 0), new GradientAlphaKey(0, 1) });
        fade.color = gradient;
        var sheet = ps.textureSheetAnimation;
        sheet.enabled = true;
        sheet.numTilesX = sheet.numTilesY = 2;
        sheet.frameOverTime = new ParticleSystem.MinMaxCurve(0, 1);
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = _dustMaterial;
        renderer.sortingLayerID = _rock.sortingLayerID;
        renderer.sortingOrder = _rock.sortingOrder + 3;
        ps.Play();
        for (int i = 0; i < count; i++)
        {
            var particle = new ParticleSystem.EmitParams
            {
                position = new Vector3(Random.Range(-0.4f, 0.4f) * size, Random.Range(-0.15f, 0.15f), 0),
                velocity = new Vector3(Random.Range(-2f, 2f), Random.Range(-0.5f, 1.8f), 0),
                rotation = Random.Range(0f, 360f)
            };
            ps.Emit(particle, 1);
        }
    }
}