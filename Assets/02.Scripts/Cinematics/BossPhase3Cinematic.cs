using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Game.Boss.Roar;

public sealed class BossPhase3Cinematic : CinematicSequence
{
    [Header("3페이즈 포효 타임라인")]
    [SerializeField] private CinematicManager _cinematicManager;
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private AirBossController _boss;
    [SerializeField] private BossRoarScreenEffect _roarEffect;
    [SerializeField, Min(0.1f)] private float _roarDuration = 0.8333333f;
    [SerializeField] private GameObject[] _existingPlatforms = new GameObject[0];
    [SerializeField] private GameObject[] _phase3Platforms = new GameObject[0];
    [SerializeField] private Material _flashMaterial;
    [SerializeField, Min(0.1f)] private float _flashDuration = 0.7f;
    [SerializeField, Min(0.05f)]
    private float _spawnFlashDuration = 0.18f;
    [SerializeField, Min(0f)]
    private float _platformDelay = 0.08f;
    [SerializeField] private Transform _bossDestination;
    public bool IsPlaying => _timelineRequested || _routine != null;
    public bool IsPhase3Active { get; private set; }
    public Transform BossDestination => _bossDestination;

    private readonly List<PlatformState> _old = new();
    private readonly List<PlatformState> _next = new();
    private Coroutine _routine;
    private bool _timelineRequested;
    private bool _stateCaptured;
    private Vector3 _bossOriginalPosition;
    private SpriteRenderer _bossRenderer;
    private bool _bossWasVisible;
    private static readonly int FlashOpacity = Shader.PropertyToID("_FlashOpacity");

    private sealed class PlatformState
    {
        public GameObject root;
        public bool active;
        public Renderer[] renderers;
        public bool[] visible;
        public Collider2D[] colliders;
        public bool[] solid;
        public readonly List<Renderer> flashes = new();
    }

    private void Awake()
    {
        Capture(_existingPlatforms, _old);
        Capture(_phase3Platforms, _next);
        ResetPlatforms();
    }

    private void Capture(GameObject[] roots, List<PlatformState> states)
    {
        foreach (var root in roots)
        {
            if (root == null)
            {
                continue;
            }
            var state = new PlatformState
            {
                root = root,
                active = root.activeSelf,
                renderers = root.GetComponentsInChildren<Renderer>(true),
                colliders = root.GetComponentsInChildren<Collider2D>(true)
            };
            state.visible = new bool[state.renderers.Length];
            state.solid = new bool[state.colliders.Length];
            for (int i = 0; i < state.colliders.Length; i++) state.solid[i] = state.colliders[i].enabled;
            for (int i = 0; i < state.renderers.Length; i++)
            {
                var source = state.renderers[i];
                state.visible[i] = source.enabled;
                if (_flashMaterial == null || !source.enabled)
                {
                    continue;
                }
                var mesh = source.GetComponent<MeshFilter>();
                var sprite = source as SpriteRenderer;
                if ((mesh == null || mesh.sharedMesh == null) && sprite == null)
                {
                    continue;
                }
                var go = new GameObject("Phase3_Flash");
                go.transform.SetParent(source.transform, false);
                Renderer overlay;
                if (sprite != null)
                {
                    var sr = go.AddComponent<SpriteRenderer>();
                    sr.sprite = sprite.sprite;
                    sr.flipX = sprite.flipX;
                    sr.flipY = sprite.flipY;
                    sr.drawMode = sprite.drawMode;
                    sr.size = sprite.size;
                    overlay = sr;
                }
                else
                {
                    go.AddComponent<MeshFilter>().sharedMesh = mesh.sharedMesh;
                    overlay = go.AddComponent<MeshRenderer>();
                }

                overlay.sharedMaterial = _flashMaterial;
                overlay.sortingLayerID = source.sortingLayerID;
                overlay.sortingOrder = source.sortingOrder + 1;
                var block = new MaterialPropertyBlock();
                Texture texture = sprite != null && sprite.sprite != null ? sprite.sprite.texture : null;
                if (texture == null && source.sharedMaterial != null && source.sharedMaterial.HasProperty("_MainTex"))
                {
                    texture = source.sharedMaterial.GetTexture("_MainTex");
                }
                block.SetTexture("_MainTex", texture != null ? texture : Texture2D.whiteTexture);
                block.SetFloat(FlashOpacity, 0f);
                overlay.SetPropertyBlock(block);
                overlay.enabled = false;
                state.flashes.Add(overlay);
            }

            states.Add(state);
        }
    }

    public void StartPhase3() => TryPlay();

    public override bool TryPlay()
    {
        if (!Application.isPlaying || !isActiveAndEnabled || IsPlaying || IsPhase3Active || _next.Count == 0)
        {
            return false;
        }

        if (_boss == null || _bossDestination == null || _cinematicManager == null || _director == null ||
            _director.playableAsset == null || _director.extrapolationMode != DirectorWrapMode.None)
        {
            return false;
        }
        _timelineRequested = true;
        if (!_cinematicManager.Play(_director, HandleTimelineCompleted, PrepareRoutine, HandleTimelineCancelled))
        {
            _timelineRequested = false;
            return false;
        }
        return true;
    }

    private IEnumerator PrepareRoutine()
    {
        _bossOriginalPosition = _boss.transform.position;
        _bossRenderer = _boss.GetComponent<SpriteRenderer>();
        _bossWasVisible = _bossRenderer != null && _bossRenderer.enabled;
        _stateCaptured = true;
        yield return null;
    }

    public void PlayRoar()
    {
        if (_timelineRequested && _boss != null)
        {
            _roarEffect?.Play(_boss.transform, _roarDuration, BossRoarScreenEffect.BossPhase.Phase2);
        }
    }

    private void MoveBossToDestination()
    {
        if (!_stateCaptured || _boss == null || _bossDestination == null)
        {
            return;
        }
        if (_bossRenderer != null)
        {
            _bossRenderer.enabled = false;
        }
        _boss.transform.position = _bossDestination.position;
    }

    private void HandleTimelineCompleted()
    {
        _timelineRequested = false;
        _roarEffect?.Stop();
        MoveBossToDestination();
        _routine = StartCoroutine(Transition());
    }

    private void HandleTimelineCancelled()
    {
        _timelineRequested = false;
        ResetPlatforms();
        NotifyCancelled();
    }

    public override bool TryPlayWithoutCinematic()
    {
        return false;
    }

    private IEnumerator Transition()
    {
        yield return null;
        float duration = Mathf.Max(0.1f, _flashDuration);
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            float t = elapsed / duration;
            float flash = Mathf.Pow(Mathf.Abs(Mathf.Sin(t * Mathf.PI * 2f)), 4f);
            foreach (var state in _old) SetFlash(state, flash);
            yield return null;
        }

        foreach (var state in _old)
        {
            SetFlash(state, 0f);
            if (state.root != null)
            {
                state.root.SetActive(false);
            }
        }

        var ordered = new List<PlatformState>(_next);
        ordered.RemoveAll(state => state.root == null);
        ordered.Sort((a, b) =>
        {
            int height = LandingHeight(a).CompareTo(LandingHeight(b));
            return height != 0 ? height : a.root.transform.position.x.CompareTo(b.root.transform.position.x);
        });
        for (int i = 0; i < ordered.Count; i++)
        {
            var state = ordered[i];
            Restore(state);
            SetCollision(state, false);
            float spawnDuration = Mathf.Max(0.05f, _spawnFlashDuration);
            for (float elapsed = 0f; elapsed < spawnDuration; elapsed += Time.deltaTime)
            {
                SetFlash(state, Mathf.Sin(Mathf.PI * elapsed / spawnDuration));
                yield return null;
            }

            SetFlash(state, 0f);
            SetCollision(state, true);
            if (i < ordered.Count - 1 && _platformDelay > 0f)
            {
                yield return new WaitForSeconds(_platformDelay);
            }
        }

        if (_bossRenderer != null && _stateCaptured)
        {
            _bossRenderer.enabled = _bossWasVisible;
        }
        _stateCaptured = false;
        IsPhase3Active = true;
        _routine = null;
        NotifyCompleted();
    }

    private static float LandingHeight(PlatformState state)
    {
        float height = float.NegativeInfinity;
        foreach (var collider in state.colliders)
        {
            if (!(collider is EdgeCollider2D edge))
            {
                continue;
            }

            foreach (var point in edge.points)
            {
                height = Mathf.Max(height, edge.transform.TransformPoint(point + edge.offset).y);
            }
        }

        return float.IsNegativeInfinity(height) ? state.root.transform.position.y : height;
    }

    private static void SetCollision(PlatformState state, bool enabled)
    {
        for (int i = 0; i < state.colliders.Length; i++)
        {
            if (state.colliders[i] != null)
            {
                state.colliders[i].enabled = enabled && state.solid[i];
            }
        }
    }

    private static void SetFlash(PlatformState state, float opacity)
    {
        var block = new MaterialPropertyBlock();
        foreach (var renderer in state.flashes)
        {
            if (renderer == null)
            {
                continue;
            }

            renderer.GetPropertyBlock(block);
            block.SetFloat(FlashOpacity, opacity);
            renderer.SetPropertyBlock(block);
            renderer.enabled = opacity > 0.001f;
        }
    }

    private static void Restore(PlatformState state)
    {
        if (state.root == null)
        {
            return;
        }
        state.root.SetActive(state.active);
        for (int i = 0; i < state.renderers.Length; i++)
        {
            if (state.renderers[i] != null)
            {
                state.renderers[i].enabled = state.visible[i];
            }
        }

        SetCollision(state, true);
        SetFlash(state, 0f);
    }

    public void ResetPlatforms()
    {
        if (_timelineRequested)
        {
            _cinematicManager.Cancel();
            return;
        }
        _roarEffect?.Stop();
        if (_stateCaptured)
        {
            if (_boss != null)
            {
                _boss.transform.position = _bossOriginalPosition;
            }
            if (_bossRenderer != null)
            {
                _bossRenderer.enabled = _bossWasVisible;
            }
            _stateCaptured = false;
        }
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        foreach (var state in _old)
        {
            Restore(state);
        }

        foreach (var state in _next)
        {
            SetFlash(state, 0f);
            SetCollision(state, false);
            if (state.root != null)
            {
                state.root.SetActive(false);
            }
        }

        IsPhase3Active = false;
    }

    public override void Cancel()
    {
        if (!IsPlaying)
        {
            return;
        }
        if (_timelineRequested)
        {
            _cinematicManager.Cancel();
            return;
        }
        ResetPlatforms();
        NotifyCancelled();
    }

    private void OnDisable() => Cancel();

    [ContextMenu("Test/Start Phase 3")]
    private void TestStart() => StartPhase3();

    [ContextMenu("Test/Reset Platforms")]
    private void TestReset()
    {
        if (Application.isPlaying)
        {
            ResetPlatforms();
        }
    }
}
