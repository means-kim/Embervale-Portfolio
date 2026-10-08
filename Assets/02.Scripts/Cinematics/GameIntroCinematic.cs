using System;
using System.Collections;
using Game.Upgrades;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// A persistent overlay hides scene activation and retains the video's final frame.
public sealed class GameIntroCinematic : MonoBehaviour
{
    private const string StartedKey = "Game.Intro.LivingTree.Completed";
    public static bool IsTransitioning { get; private set; }

    public static bool HasStartedGame
    {
        get
        {
            if (PlayerPrefs.GetInt(StartedKey, 0) == 1) return true;
            // Existing saves predate this intro. Preserve their continue behavior.
            foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
                if (PlayerPrefs.HasKey("Upgrade.Level." + type)) return true;
            foreach (EncounterId id in Enum.GetValues(typeof(EncounterId)))
                if (PlayerPrefs.GetInt("Encounter.IntroSeen." + id, 0) == 1) return true;
            foreach (EliminateId id in Enum.GetValues(typeof(EliminateId)))
                if (EncounterHistory.HasEliminated(id)) return true;
            return false;
        }
    }

    private VideoPlayer _video;
    private RenderTexture _texture;
    private CanvasGroup _group;
    private AsyncOperation _loading;
    private bool _finished;
    private bool _failed;
    private float _previousTimeScale;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => IsTransitioning = false;

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(StartedKey);
        PlayerPrefs.Save();
    }

    public static void Begin(VideoClip clip, AsyncOperation loading)
    {
        var root = new GameObject("LivingTree Intro Transition");
        DontDestroyOnLoad(root);
        var transition = root.AddComponent<GameIntroCinematic>();
        transition._loading = loading;
        transition.StartCoroutine(transition.Play(clip));
    }

    private IEnumerator Play(VideoClip clip)
    {
        IsTransitioning = true;
        _previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        AudioManager.Instance?.StopAllBgm();

        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;
        gameObject.AddComponent<GraphicRaycaster>();
        _group = gameObject.AddComponent<CanvasGroup>();

        var background = new GameObject("Black Background", typeof(RectTransform), typeof(Image));
        background.transform.SetParent(transform, false);
        Stretch(background.GetComponent<RectTransform>());
        background.GetComponent<Image>().color = Color.black;

        var picture = new GameObject("Video", typeof(RectTransform), typeof(RawImage), typeof(AspectRatioFitter));
        picture.transform.SetParent(transform, false);
        Stretch(picture.GetComponent<RectTransform>());
        var aspect = picture.GetComponent<AspectRatioFitter>();
        aspect.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        aspect.aspectRatio = clip.height > 0 ? (float)clip.width / clip.height : 16f / 9f;
        _texture = new RenderTexture(Mathf.Max(1, (int)clip.width), Mathf.Max(1, (int)clip.height), 0);
        _texture.Create();
        var previous = RenderTexture.active;
        RenderTexture.active = _texture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = previous;
        picture.GetComponent<RawImage>().texture = _texture;

        _video = gameObject.AddComponent<VideoPlayer>();
        _video.playOnAwake = false;
        _video.source = VideoSource.VideoClip;
        _video.clip = clip;
        _video.renderMode = VideoRenderMode.RenderTexture;
        _video.targetTexture = _texture;
        _video.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;
        _video.audioOutputMode = VideoAudioOutputMode.Direct;
        _video.waitForFirstFrame = true;
        _video.isLooping = false;
        _video.loopPointReached += OnFinished;
        _video.errorReceived += OnError;
        _video.Prepare();

        float deadline = Time.realtimeSinceStartup + 20f;
        while (!_video.isPrepared && !_failed && Time.realtimeSinceStartup < deadline)
            yield return null;

        if (!_video.isPrepared && !_failed) Fail("Video preparation timed out.");
        if (!_failed)
        {
            _video.Play();
            deadline = Time.realtimeSinceStartup + (float)clip.length + 20f;
            while (!_finished && !_failed && Time.realtimeSinceStartup < deadline)
                yield return null;
            if (!_finished && !_failed) Fail("Video playback timed out.");
            _video.Pause();
        }

        // Leave the last frame visible if the scene is not ready yet.
        _loading.allowSceneActivation = true;
        while (!_loading.isDone) yield return null;
        // Let Awake/Start and the first scene render finish under the overlay.
        yield return null;
        yield return null;
        if (_finished && !_failed)
        {
            PlayerPrefs.SetInt(StartedKey, 1);
            PlayerPrefs.Save();
        }

        float elapsed = 0f;
        while (elapsed < 0.4f)
        {
            elapsed += Time.unscaledDeltaTime;
            _group.alpha = 1f - Mathf.Clamp01(elapsed / 0.4f);
            yield return null;
        }
        IsTransitioning = false;
        // The scene's GameManager owns the ability-selection pause when present.
        if (GameManager.Instance == null) Time.timeScale = _previousTimeScale;
        Destroy(gameObject);
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    private void OnFinished(VideoPlayer source) => _finished = true;
    private void OnError(VideoPlayer source, string message) => Fail(message);
    private void Fail(string message)
    {
        _failed = true;
        Debug.LogWarning("LivingTree intro: " + message + " Continuing into the game.", this);
    }

    private void OnDestroy()
    {
        if (_loading != null && !_loading.isDone) _loading.allowSceneActivation = true;
        if (_video != null)
        {
            _video.loopPointReached -= OnFinished;
            _video.errorReceived -= OnError;
            _video.Stop();
            _video.targetTexture = null;
        }
        if (_texture != null)
        {
            _texture.Release();
            Destroy(_texture);
        }
        if (IsTransitioning && GameManager.Instance == null) Time.timeScale = _previousTimeScale;
        IsTransitioning = false;
    }
}
