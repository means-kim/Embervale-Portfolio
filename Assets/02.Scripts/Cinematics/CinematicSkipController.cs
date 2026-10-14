using System;
using TMPro;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// One overlay also covers videos and transitions that outlive the shared manager.
public sealed class CinematicSkipController : MonoBehaviour
{
    private static CinematicSkipController _instance;
    private MonoBehaviour _owner;
    private Action _skip;
    private float _duration;
    private float _held;
    private bool _waitForRelease;
    private CanvasGroup _group;
    private RectTransform _fill;
    public static bool IsActive => _instance != null && _instance._owner != null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => _instance = null;

    public static void Begin(MonoBehaviour owner, Action skip, float duration)
    {
        if (!Application.isPlaying || owner == null || skip == null) return;
        if (_instance == null)
        {
            var root = new GameObject("CinematicSkipUI", typeof(RectTransform));
            DontDestroyOnLoad(root);
            _instance = root.AddComponent<CinematicSkipController>();
        }
        _instance._owner = owner;
        _instance._skip = skip;
        _instance._duration = Mathf.Max(0.1f, duration);
        _instance._waitForRelease = EscapeHeld();
        _instance.ResetHold();
    }

    public static void End(MonoBehaviour owner)
    {
        if (_instance != null && _instance._owner == owner) _instance.Clear();
    }

    private static bool EscapeHeld()
    {
        bool held = false;
#if ENABLE_LEGACY_INPUT_MANAGER
        held |= Input.GetKey(KeyCode.Escape);
#endif
#if ENABLE_INPUT_SYSTEM
        held |= Keyboard.current != null && Keyboard.current.escapeKey.isPressed;
#endif
        return held;
    }

    private void Awake()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;
        var scaler = gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        _group = gameObject.AddComponent<CanvasGroup>();
        _group.interactable = _group.blocksRaycasts = false;

        var panel = Rect("SkipProgress", transform, new Vector2(310, 74));
        panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(1, 0);
        panel.anchoredPosition = new Vector2(-48, 44);
        var backdrop = panel.gameObject.AddComponent<UnityEngine.UI.Image>();
        backdrop.color = new Color(0.02f, 0.03f, 0.05f, 0.85f);
        backdrop.raycastTarget = false;

        var label = Rect("Label", panel, new Vector2(274, 38));
        label.anchoredPosition = new Vector2(0, 8);
        var text = label.gameObject.AddComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = "ESC   HOLD TO SKIP";
        text.fontSize = 22;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;

        var track = Rect("Track", panel, new Vector2(274, 5));
        track.anchoredPosition = new Vector2(0, -22);
        var trackImage = track.gameObject.AddComponent<UnityEngine.UI.Image>();
        trackImage.color = new Color(1, 1, 1, 0.2f);
        trackImage.raycastTarget = false;
        _fill = Rect("Fill", track, Vector2.zero);
        _fill.anchorMin = Vector2.zero;
        _fill.anchorMax = Vector2.one;
        _fill.offsetMin = _fill.offsetMax = Vector2.zero;
        var fillImage = _fill.gameObject.AddComponent<UnityEngine.UI.Image>();
        fillImage.color = new Color(1f, 0.78f, 0.42f);
        fillImage.raycastTarget = false;
        ResetHold();
    }

    private static RectTransform Rect(string name, Transform parent, Vector2 size)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.sizeDelta = size;
        return rect;
    }

    private void Update()
    {
        if (_owner == null || !_owner.isActiveAndEnabled)
        {
            Clear();
            return;
        }
        if (!Application.isFocused)
        {
            ResetHold();
            _waitForRelease = true;
            return;
        }
        Tick(EscapeHeld(), Time.unscaledDeltaTime);
    }

    private void Tick(bool pressed, float deltaTime)
    {
        if (_waitForRelease)
        {
            if (!pressed) _waitForRelease = false;
            ResetHold();
            return;
        }
        if (!pressed || _skip == null)
        {
            ResetHold();
            return;
        }
        _held += deltaTime;
        _group.alpha = 1;
        _fill.anchorMax = new Vector2(Mathf.Clamp01(_held / _duration), 1);
        if (_held < _duration) return;
        Action skip = _skip;
        // Clear before invoking: completion may immediately start the next sequence.
        Clear();
        skip();
    }

    private void ResetHold()
    {
        _held = 0;
        if (_group != null) _group.alpha = 0;
        if (_fill != null) _fill.anchorMax = new Vector2(0, 1);
    }

    private void Clear()
    {
        _owner = null;
        _skip = null;
        ResetHold();
    }

    private void OnApplicationFocus(bool focused)
    {
        _waitForRelease = true;
        ResetHold();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) OnApplicationFocus(false);
    }

    private void OnDisable() => Clear();
    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}
