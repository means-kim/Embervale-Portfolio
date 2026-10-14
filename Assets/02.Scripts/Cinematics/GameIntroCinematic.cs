using System;
using System.Collections;
using System.Collections.Generic;
using Game.Upgrades;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

// A persistent black overlay separates the recorded intro from live gameplay.
public sealed class GameIntroCinematic : MonoBehaviour
{
    private const string StartedKey = "Game.Intro.LivingTree.Completed";
    private const double TrimEndSeconds = 1.0;
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
    private Image _loadingIndicator;
    private bool _firstFrameReady;
    private bool _skipRequested;
    private float _skipHoldDuration = 1.5f;
    private readonly List<PlayerInput> _lockedInputs = new List<PlayerInput>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => IsTransitioning = false;

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(StartedKey);
        PlayerPrefs.Save();
    }

    public static void Begin(VideoClip clip, string sceneName, float skipHoldDuration = 1.5f)
    {
        if (IsTransitioning) return;
        var root = new GameObject("LivingTree Intro Transition");
        DontDestroyOnLoad(root);
        var transition = root.AddComponent<GameIntroCinematic>();
        transition._skipHoldDuration = skipHoldDuration;
        transition.StartCoroutine(transition.Play(clip, sceneName));
    }

    private IEnumerator Play(VideoClip clip, string sceneName)
    {
        IsTransitioning = true;
        _previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        AudioManager.Instance?.StopAllBgm();

        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue - 1;
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

        var indicator = new GameObject("Loading Indicator", typeof(RectTransform), typeof(Image));
        indicator.transform.SetParent(transform, false);
        var indicatorRect = indicator.GetComponent<RectTransform>();
        indicatorRect.anchorMin = indicatorRect.anchorMax = new Vector2(0.95f, 0.06f);
        indicatorRect.sizeDelta = new Vector2(18f, 18f);
        _loadingIndicator = indicator.GetComponent<Image>();
        _loadingIndicator.raycastTarget = false;

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
        _video.sendFrameReadyEvents = true;
        _video.frameReady += OnFrameReady;
        CinematicSkipController.Begin(this, RequestSkip, _skipHoldDuration);
        // Load and activate gameplay under the opaque overlay before decoding video.
        // GameManager waits for IsTransitioning, so gameplay resumes after the intro.
        yield return null;
        SceneManager.sceneLoaded += LockSceneInput;
        _loading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!_loading.isDone) yield return null;
        SceneManager.sceneLoaded -= LockSceneInput;
        // Allow the destination scene's Start methods and first render to settle.
        yield return null;
        yield return null;
        if (!_skipRequested) _video.Prepare();

        float deadline = Time.realtimeSinceStartup + 20f;
        while (!_video.isPrepared && !_failed && !_skipRequested && Time.realtimeSinceStartup < deadline)
            yield return null;

        if (!_video.isPrepared && !_failed && !_skipRequested) Fail("Video preparation timed out.");
        if (!_failed && !_skipRequested)
        {
            double playbackEndTime = Math.Max(0.0, clip.length - TrimEndSeconds);
            _video.Play();
            deadline = Time.realtimeSinceStartup + (float)clip.length + 20f;
            while (!_finished && !_failed && !_skipRequested && Time.realtimeSinceStartup < deadline)
            {
                if (_firstFrameReady && _video.time >= playbackEndTime)
                {
                    // Reaching the requested cut is a successful intro completion.
                    _finished = true;
                    break;
                }
                yield return null;
            }
            if (!_finished && !_failed && !_skipRequested) Fail("Video playback timed out.");
            _video.Pause();
        }
        if (_skipRequested) _finished = true;

        // The destination is already loaded: reveal it without another loading wait.
        if (_finished && (!_failed || _skipRequested))
        {
            PlayerPrefs.SetInt(StartedKey, 1);
            PlayerPrefs.Save();
        }

        // Cut directly to the already-loaded game without a fade or black interval.
        canvas.enabled = false;
        CinematicSkipController.End(this);
        IsTransitioning = false;
        RestoreSceneInput();
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

    private void LockSceneInput(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Single) return;
        foreach (PlayerInput input in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
        {
            if (input.gameObject.scene != scene || !input.enabled) continue;
            _lockedInputs.Add(input);
            input.enabled = false;
        }
    }

    private void RestoreSceneInput()
    {
        foreach (PlayerInput input in _lockedInputs)
            if (input != null) input.enabled = true;
        _lockedInputs.Clear();
    }

    public void RequestSkip()
    {
        if (!IsTransitioning || _skipRequested) return;
        _skipRequested = true;
        _video?.Stop();
        // The loading coroutine still waits for a ready scene before revealing gameplay.
    }

    private void OnFinished(VideoPlayer source) => _finished = true;
    private void OnFrameReady(VideoPlayer source, long frame)
    {
        _firstFrameReady = true;
        _loadingIndicator.enabled = false;
        source.sendFrameReadyEvents = false;
    }

    private void Update()
    {
        if (_loadingIndicator != null && _loadingIndicator.enabled)
            _loadingIndicator.rectTransform.Rotate(0f, 0f, -180f * Time.unscaledDeltaTime);
    }
    private void OnError(VideoPlayer source, string message) => Fail(message);
    private void Fail(string message)
    {
        _failed = true;
        Debug.LogWarning($"LivingTree intro: {message} Prepared={_video != null && _video.isPrepared}, " +
            $"FirstFrame={_firstFrameReady}, Graphics={SystemInfo.graphicsDeviceType}. Continuing into the game.", this);
    }

    private void OnDestroy()
    {
        CinematicSkipController.End(this);
        SceneManager.sceneLoaded -= LockSceneInput;
        RestoreSceneInput();
        if (_loading != null && !_loading.isDone) _loading.allowSceneActivation = true;
        if (_video != null)
        {
            _video.loopPointReached -= OnFinished;
            _video.errorReceived -= OnError;
            _video.frameReady -= OnFrameReady;
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
