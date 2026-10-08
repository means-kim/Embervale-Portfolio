using System;
using System.Collections;
using Game.Upgrades;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool IsLoading { get; private set; }
    private static string _requestedScene;

    [SerializeField] private PlayerUpgrades _upgrades;
    [SerializeField] private UI_Ability _abilityUI;
    [SerializeField] private UI_UpgradeBinding _upgradeBinding;
    [SerializeField] private BgmType _startBgm = BgmType.Snow;
    [SerializeField] private UnityEvent _onGameReady = new UnityEvent();
    public bool WaitingForAbility { get; private set; }
    private float _resumeTimeScale = 1f;
    private bool _restoreCancel;
    private bool _previousAllowCancel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        Instance = null;
        IsLoading = false;
        _requestedScene = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneCallback()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Single || _requestedScene == null)
        {
            return;
        }

        if (scene.path != _requestedScene && scene.name != _requestedScene)
        {
            return;
        }

        _requestedScene = null;
        IsLoading = false;
        foreach (GameManager manager in FindObjectsByType<GameManager>(FindObjectsSortMode.None))
        {
            if (manager.gameObject.scene == scene)
            {
                return;
            }
        }

        // A requested game scene can bootstrap even before an Inspector manager is placed.
        var root = new GameObject("GameManager");
        SceneManager.MoveGameObjectToScene(root, scene);
        root.AddComponent<GameManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this && Instance.gameObject.scene == gameObject.scene)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        _resumeTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
        Time.timeScale = 0f;
    }

    private IEnumerator Start()
    {
        // Let every scene component finish Awake/OnEnable/Start before opening the UI.
        yield return null;
        while (GameIntroCinematic.IsTransitioning)
        {
            yield return null;
        }
        if (_upgrades == null)
        {
            foreach (PlayerUpgrades candidate in FindObjectsByType<PlayerUpgrades>(FindObjectsSortMode.None))
            {
                if (candidate.gameObject.scene == gameObject.scene)
                {
                    _upgrades = candidate;
                    break;
                }
            }
        }

        if (_abilityUI == null)
        {
            foreach (UI_Ability candidate in FindObjectsByType<UI_Ability>(FindObjectsSortMode.None))
            {
                if (candidate.gameObject.scene == gameObject.scene)
                {
                    _abilityUI = candidate;
                    break;
                }
            }
        }

        AudioManager.Instance?.StopAllBgm();
        AudioManager.Instance?.PlayBgm(_startBgm);

        if (_upgrades == null || _abilityUI == null)
        {
            Debug.LogWarning("GameManager: 실제 플레이어의 PlayerUpgrades와 UI_Ability를 연결하세요.", this);
            ResumeGame();
            yield break;
        }

        if (_upgradeBinding == null)
        {
            _upgradeBinding = _abilityUI.GetComponent<UI_UpgradeBinding>();
        }

        if (_upgradeBinding == null)
        {
            _upgradeBinding = _abilityUI.gameObject.AddComponent<UI_UpgradeBinding>();
        }

        _upgradeBinding.Bind(_abilityUI, _upgrades);
        _abilityUI.ConfigurePlayer(_upgrades.gameObject);
        _previousAllowCancel = _abilityUI.AllowCancel;
        _restoreCancel = true;
        _abilityUI.SetAllowCancel(false);

        if (!_abilityUI.HasSelectableOptions)
        {
            ResumeGame();
            yield break;
        }

        WaitingForAbility = true;
        _abilityUI.AbilityConfirmed += HandleAbilityConfirmed;
        _abilityUI.Open();
        if (!_abilityUI.IsOpen)
        {
            Debug.LogWarning("GameManager: 능력치 UI의 Panel 연결을 확인하세요.", this);
            ResumeGame();
        }
    }

    private void HandleAbilityConfirmed(UI_Ability.AbilityType type) => ResumeGame();

    private void ResumeGame()
    {
        WaitingForAbility = false;
        if (_abilityUI != null)
        {
            _abilityUI.AbilityConfirmed -= HandleAbilityConfirmed;
        }
        RestoreCancel();
        Time.timeScale = _resumeTimeScale;
        _onGameReady.Invoke();
    }

    // Preserve upgrades, intro history, and defeated enemies on retry/continue.
    public static bool LoadGameScene(string sceneName, bool newGame = false, VideoClip introClip = null)
    {
        if (IsLoading || GameIntroCinematic.IsTransitioning || string.IsNullOrWhiteSpace(sceneName))
        {
            return false;
        }
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("GameManager: Build Profiles에 등록되지 않은 씬: " + sceneName);
            return false;
        }

        if (newGame)
        {
            UpgradeHistory.ResetProgress();
            EncounterHistory.ResetSavedProgress();
            GameIntroCinematic.ResetProgress();
        }
        else if (Instance != null && Instance._upgrades != null)
        {
            Instance._upgrades.SaveProgress();
        }
        IsLoading = true;
        _requestedScene = sceneName;
        try
        {
            bool showIntro = introClip != null && !GameIntroCinematic.HasStartedGame;
            AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (showIntro)
            {
                loading.allowSceneActivation = false;
                GameIntroCinematic.Begin(introClip, loading);
            }
            return true;
        }
        catch (Exception exception)
        {
            IsLoading = false;
            _requestedScene = null;
            Debug.LogException(exception);
            return false;
        }
    }

    public static bool RetryScene(Scene scene) => LoadGameScene(scene.path);

    private void RestoreCancel()
    {
        if (_restoreCancel && _abilityUI != null)
        {
            _abilityUI.SetAllowCancel(_previousAllowCancel);
        }
        _restoreCancel = false;
    }

    private void OnDisable()
    {
        if (_abilityUI != null)
        {
            _abilityUI.AbilityConfirmed -= HandleAbilityConfirmed;
        }
        RestoreCancel();
        if (Instance == this && !IsLoading)
        {
            Time.timeScale = _resumeTimeScale;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
