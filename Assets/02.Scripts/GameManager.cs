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
    private static bool _restoreCheckpointOnLoad;
    private static float _normalFixedDeltaTime;

    [SerializeField] private PlayerUpgrades _upgrades;
    [SerializeField] private UI_Ability _abilityUI;
    [SerializeField] private UI_UpgradeBinding _upgradeBinding;
    [SerializeField] private BgmType _startBgm = BgmType.Snow;
    [SerializeField] private UnityEvent _onGameReady = new UnityEvent();
    public bool WaitingForAbility { get; private set; }
    private float _resumeTimeScale = 1f;
    private bool _restoreCancel;
    private bool _previousAllowCancel;
    private Coroutine _resumeAfterSelection;
    private Rigidbody2D _retryBody;
    private Vector3 _retryPosition;
    private bool _retryWasSimulated;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        Instance = null;
        IsLoading = false;
        _requestedScene = null;
        _restoreCheckpointOnLoad = false;
        _normalFixedDeltaTime = Time.fixedDeltaTime;
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
        // 사망/씬 전환 직전의 히트스톱·차지 점프 속도를 이어받지 않는다.
        _resumeTimeScale = 1f;
        Time.fixedDeltaTime = _normalFixedDeltaTime;
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

        bool offerUpgrade = _restoreCheckpointOnLoad;
        if (_restoreCheckpointOnLoad)
        {
            _restoreCheckpointOnLoad = false;
            if (!CheckpointHistory.Restore(gameObject.scene))
            {
                Debug.LogWarning("GameManager: 체크포인트 복원 실패 또는 미등록. 씬 시작 위치에서 재시작합니다.", this);
                foreach (Player player in FindObjectsByType<Player>(FindObjectsSortMode.None))
                {
                    if (player.gameObject.scene != gameObject.scene) continue;
                    player.MoveToCheckpoint(player.transform.position);
                    break;
                }
            }
            Physics2D.SyncTransforms();
            PrepareRetryPhysics();
        }

        // Only the death screen's RetryScene request grants an upgrade choice.
        // Starting from the title keeps saved stats without granting another level.
        if (!offerUpgrade)
        {
            ResumeGame();
            yield break;
        }

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
        _abilityUI.AbilitySelectionCompleted += HandleAbilitySelectionCompleted;
        _abilityUI.Open();
        if (!_abilityUI.IsOpen)
        {
            Debug.LogWarning("GameManager: 능력치 UI의 Panel 연결을 확인하세요.", this);
            ResumeGame();
        }
    }

    private void HandleAbilitySelectionCompleted(UI_Ability.AbilityType type)
    {
        if (!WaitingForAbility || _resumeAfterSelection != null) return;
        _resumeAfterSelection = StartCoroutine(ResumeAfterSelection());
    }

    private IEnumerator ResumeAfterSelection()
    {
        // 선택 클릭이 같은 프레임의 플레이어 공격으로 이어지지 않게 한다.
        yield return null;
        while (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            yield return null;
        }
        _resumeAfterSelection = null;
        if (IsLoading || _abilityUI == null || _abilityUI.IsOpen) yield break;
        Physics2D.SyncTransforms();
        ResumeGame();
    }

    private void ResumeGame()
    {
        WaitingForAbility = false;
        if (_abilityUI != null)
        {
            _abilityUI.AbilitySelectionCompleted -= HandleAbilitySelectionCompleted;
        }
        RestoreCancel();
        RestoreRetryPhysics();
        Time.fixedDeltaTime = _normalFixedDeltaTime;
        Time.timeScale = _resumeTimeScale;
        _onGameReady.Invoke();
    }

    // Preserve upgrades, intro history, and defeated enemies on retry/continue.
    public static bool LoadGameScene(string sceneName, bool newGame = false, VideoClip introClip = null, bool replayIntro = false, Difficulty difficulty = Difficulty.Normal, float introSkipHoldDuration = 1.5f, bool restoreCheckpoint = false)
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
            GameDifficulty.Set(difficulty);
            CheckpointHistory.Clear();
            UpgradeHistory.ResetProgress();
            EncounterHistory.ResetSavedProgress();
            GameIntroCinematic.ResetProgress();
        }
        else if (Instance != null && Instance._upgrades != null)
        {
            Instance._upgrades.SaveProgress();
        }
        IsLoading = true;
        _restoreCheckpointOnLoad = restoreCheckpoint;
        _requestedScene = sceneName;
        try
        {
            bool showIntro = introClip != null && (replayIntro || !GameIntroCinematic.HasStartedGame);
            Debug.Log($"GameManager: Loading {sceneName}, intro={(showIntro ? "play" : "skip")}, replayIntro={replayIntro}, newGame={newGame}.");
            if (showIntro)
            {
                GameIntroCinematic.Begin(introClip, sceneName, introSkipHoldDuration);
            }
            else
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            }
            return true;
        }
        catch (Exception exception)
        {
            IsLoading = false;
            _requestedScene = null;
            _restoreCheckpointOnLoad = false;
            Debug.LogException(exception);
            return false;
        }
    }

    public static bool RetryScene(Scene scene)
    {
        if (IsLoading || GameIntroCinematic.IsTransitioning) return false;
        foreach (Player player in FindObjectsByType<Player>(FindObjectsSortMode.None))
        {
            if (player.gameObject.scene != scene) continue;
            CheckpointHistory.RecordCurrent(player);
            break;
        }
        // 재시도 플래그는 씬 로드를 시작하기 전에 확정한다.
        return LoadGameScene(scene.path, restoreCheckpoint: true);
    }

    private void PrepareRetryPhysics()
    {
        foreach (Player player in FindObjectsByType<Player>(FindObjectsSortMode.None))
        {
            if (player.gameObject.scene != gameObject.scene)
            {
                continue;
            }
            _retryBody = player.GetComponent<Rigidbody2D>();
            if (_retryBody == null)
            {
                return;
            }
            Collider2D bodyCollider = player.GetComponent<Collider2D>();
            if (bodyCollider != null)
            {
                // 발이 얇은 바닥 안에서 시작하지 않도록 실제 겹침만 정리한다.
                float clearance = 0f;
                foreach (Collider2D ground in Physics2D.OverlapBoxAll(bodyCollider.bounds.center, bodyCollider.bounds.size, 0f))
                {
                    if (ground == bodyCollider || ground.isTrigger ||
                        (!ground.CompareTag("Ground") && !ground.CompareTag("Platform")) ||
                        ground.bounds.max.y > bodyCollider.bounds.center.y)
                    {
                        continue;
                    }
                    ColliderDistance2D distance = bodyCollider.Distance(ground);
                    if (distance.isValid && distance.isOverlapped)
                    {
                        clearance = Mathf.Max(clearance, ground.bounds.max.y - bodyCollider.bounds.min.y + 0.02f);
                    }
                }
                if (clearance > 0f)
                {
                    player.MoveToCheckpoint(player.transform.position + Vector3.up * clearance);
                }
            }
            _retryPosition = player.transform.position;
            _retryWasSimulated = _retryBody.simulated;
            _retryBody.linearVelocity = Vector2.zero;
            _retryBody.angularVelocity = 0f;
            _retryBody.simulated = false;
            Debug.Log("GameManager: Retry prepared at " + _retryPosition + ". Physics held until selection completes.", this);
            return;
        }
    }

    private void RestoreRetryPhysics()
    {
        if (_retryBody == null) return;
        _retryBody.transform.position = _retryPosition;
        _retryBody.position = _retryPosition;
        _retryBody.linearVelocity = Vector2.zero;
        _retryBody.angularVelocity = 0f;
        _retryBody.simulated = _retryWasSimulated;
        Physics2D.SyncTransforms();
        Debug.Log("GameManager: Retry resumed at " + _retryPosition + ".", this);
        _retryBody = null;
    }

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
            _abilityUI.AbilitySelectionCompleted -= HandleAbilitySelectionCompleted;
        }
        if (_resumeAfterSelection != null)
        {
            StopCoroutine(_resumeAfterSelection);
            _resumeAfterSelection = null;
        }
        RestoreCancel();
        if (Instance == this && !IsLoading)
        {
            RestoreRetryPhysics();
            Time.timeScale = _resumeTimeScale;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
