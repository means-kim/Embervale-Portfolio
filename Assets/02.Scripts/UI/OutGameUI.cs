using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
public class OutGameUI : MonoBehaviour
{
    [Header("Scene / Intro")]
    [SerializeField] private string _gameSceneName = "MainTestScene";
    [SerializeField] private VideoClip _introClip;
    [SerializeField, Min(0.1f)] private float _introSkipHoldDuration = 1.5f;

    [Header("Main Buttons")]
    [Tooltip("메인 버튼 그룹 루트. CanvasGroup이 있으면 페이드 전환에 사용한다. 비워두면 개별 버튼만 켜고 끈다.")]
    [SerializeField] private GameObject _mainButtonsRoot;
    [Tooltip("이어하기 버튼. 세이브가 없을 때는 자동으로 숨긴다. 기존 GameStartButton을 연결하면 된다.")]
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _newGameButton;
    [Tooltip("설정 버튼. 새로 만들어 연결한다. 비워두어도 동작한다.")]
    [SerializeField] private GameObject _settingsButton;
    [Tooltip("나가기 버튼. 메인 그룹으로 함께 숨김 처리할 때 사용한다. 비워두어도 된다.")]
    [SerializeField] private GameObject _leaveButton;

    [Header("Difficulty Buttons")]
    [SerializeField] private CanvasGroup _difficultyButtons;
    [Tooltip("뒤로가기 버튼. 새로 만들어 연결한다.")]
    [SerializeField] private UnityEngine.UI.Button _backButton;
    [SerializeField] private UnityEngine.UI.Button _easyButton;
    [SerializeField] private UnityEngine.UI.Button _normalButton;
    [SerializeField] private UnityEngine.UI.Button _hardButton;

    [Header("Settings (Optional)")]
    [Tooltip("설정 패널. 있으면 설정 버튼으로 열고 닫는다. 없으면 OnSettingsRequested 이벤트만 발생한다.")]
    [SerializeField] private GameObject _settingsPanel;
    [Tooltip("켜져 있으면 설정 화면이 열릴 때 메인 버튼 그룹을 숨긴다.")]
    [SerializeField] private bool _hideMainWhileSettings = true;
    [SerializeField] private UnityEvent _onSettingsRequested = new UnityEvent();

    [Header("Continue Rule")]
    [Tooltip("켜져 있으면 세이브가 없을 때 이어하기 버튼을 숨긴다. 끄면 회색 비활성화로 표시한다.")]
    [SerializeField] private bool _hideContinueWhenNoSave = true;

    [SerializeField, Min(0f)] private float _difficultyFadeDuration = 0.3f;
    private bool _choosingDifficulty;
    private bool _isSettingsOpen;
    private bool _fading;
    private bool _hasEnabled;

    private CanvasGroup MainGroup => _mainButtonsRoot != null ? _mainButtonsRoot.GetComponent<CanvasGroup>() : null;

    private void OnDisable()
    {
        StopAllCoroutines();
        ResetMainInteractable();
        _choosingDifficulty = _isSettingsOpen = _fading = false;
    }

    private void ResetMainInteractable()
    {
        CanvasGroup mainGroup = MainGroup;
        if (mainGroup != null)
        {
            mainGroup.alpha = 1f;
            mainGroup.interactable = mainGroup.blocksRaycasts = true;
        }
        else if (_newGameButton != null)
        {
            var group = _newGameButton.GetComponent<CanvasGroup>();
            if (group != null && _choosingDifficulty)
            {
                group.alpha = 1f;
                group.interactable = group.blocksRaycasts = true;
            }
        }
    }

    private void OnEnable()
    {
        _choosingDifficulty = _isSettingsOpen = _fading = false;
        if (_difficultyButtons != null)
        {
            _difficultyButtons.alpha = 0f;
            _difficultyButtons.interactable = _difficultyButtons.blocksRaycasts = false;
            _difficultyButtons.gameObject.SetActive(false);
        }
        if (_settingsPanel != null)
        {
            _settingsPanel.SetActive(false);
        }
        ShowMainInstant();
        _hasEnabled = true;
    }

    private void ShowMainInstant()
    {
        RefreshContinueVisibility();
        CanvasGroup mainGroup = MainGroup;
        if (_mainButtonsRoot != null)
        {
            _mainButtonsRoot.SetActive(true);
            if (mainGroup != null)
            {
                mainGroup.alpha = 1f;
                mainGroup.interactable = mainGroup.blocksRaycasts = true;
            }
        }
        else if (_newGameButton != null)
        {
            _newGameButton.SetActive(true);
            if (_hasEnabled && _newGameButton.TryGetComponent<CanvasGroup>(out var group))
            {
                group.alpha = 1f;
                group.interactable = group.blocksRaycasts = true;
            }
        }
        SelectMainDefault();
    }

    private void RefreshContinueVisibility()
    {
        if (_continueButton == null)
        {
            return;
        }
        bool hasSave = GameIntroCinematic.HasStartedGame;
        if (_hideContinueWhenNoSave)
        {
            _continueButton.SetActive(hasSave);
        }
        else
        {
            _continueButton.SetActive(true);
        }
        if (_continueButton.TryGetComponent<UnityEngine.UI.Button>(out var button))
        {
            button.interactable = hasSave;
        }
    }

    private static string SaveBreakdown()
    {
        var hits = new System.Collections.Generic.List<string>();
        if (PlayerPrefs.GetInt("Game.Intro.LivingTree.Completed", 0) == 1)
        {
            hits.Add("인트로완료");
        }
        foreach (UpgradeType type in System.Enum.GetValues(typeof(UpgradeType)))
        {
            if (PlayerPrefs.HasKey("Upgrade.Level." + type))
            {
                hits.Add("업그레이드기록");
                break;
            }
        }
        foreach (EncounterId id in System.Enum.GetValues(typeof(EncounterId)))
        {
            if (PlayerPrefs.GetInt("Encounter.IntroSeen." + id, 0) == 1)
            {
                hits.Add("조우연출기록");
                break;
            }
        }
        foreach (EliminateId id in System.Enum.GetValues(typeof(EliminateId)))
        {
            if (EncounterHistory.HasEliminated(id))
            {
                hits.Add("처치기록");
                break;
            }
        }
        return hits.Count == 0 ? "기록없음" : string.Join(",", hits);
    }

    [ContextMenu("테스트: 세이브 기록 삭제 (첫 실행 상태)")]
    private void ClearSaveRecordsForTest()
    {
        GameIntroCinematic.ResetProgress();
        UpgradeHistory.ResetProgress();
        EncounterHistory.ResetSavedProgress();
        RefreshContinueVisibility();
        Debug.Log("[OutGameUI] 세이브 기록 삭제. 씬을 리로드하면 이어하기가 숨겨진다.", this);
    }

    private void SelectMainDefault()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        if (_continueButton != null && _continueButton.activeInHierarchy &&
            _continueButton.TryGetComponent<UnityEngine.UI.Button>(out var continueBtn))
        {
            continueBtn.Select();
            return;
        }
        if (_newGameButton != null && _newGameButton.activeInHierarchy &&
            _newGameButton.TryGetComponent<UnityEngine.UI.Button>(out var newGameBtn))
        {
            newGameBtn.Select();
        }
    }

    public void OnClickContinue()
    {
        if (_choosingDifficulty || _isSettingsOpen || GameManager.IsLoading || GameIntroCinematic.IsTransitioning)
        {
            return;
        }
        if (!GameIntroCinematic.HasStartedGame)
        {
            return;
        }
        GameManager.LoadGameScene(_gameSceneName, introClip: _introClip, introSkipHoldDuration: _introSkipHoldDuration);
    }

    public void OnClickGameStart()
    {
        if (_continueButton != null)
        {
            OnClickContinue();
            return;
        }
        if (!GameIntroCinematic.HasStartedGame)
        {
            OnClickNewGame();
        }
        else
        {
            OnClickContinue();
        }
    }

    public void OnClickNewGame()
    {
        if (_choosingDifficulty || _isSettingsOpen || GameManager.IsLoading || GameIntroCinematic.IsTransitioning)
        {
            return;
        }
        if (_difficultyButtons == null || _easyButton == null || _normalButton == null || _hardButton == null)
        {
            Debug.LogError("OutGameUI: 난이도 그룹과 Easy/Normal/hard 버튼을 연결해주세요.", this);
            return;
        }
        _choosingDifficulty = _fading = true;
        StartCoroutine(ShowDifficulty());
    }

    private IEnumerator ShowDifficulty()
    {
        yield return FadeMain(0f, hideAfter: true);
        _difficultyButtons.gameObject.SetActive(true);
        yield return Fade(_difficultyButtons, 1f);
        _difficultyButtons.interactable = _difficultyButtons.blocksRaycasts = true;
        _fading = false;
        if (_normalButton != null)
        {
            _normalButton.Select();
        }
        else if (_backButton != null)
        {
            _backButton.Select();
        }
    }

    public void OnClickBack()
    {
        if (!_choosingDifficulty || _fading || _isSettingsOpen)
        {
            return;
        }
        _choosingDifficulty = _fading = true;
        StartCoroutine(ShowMain());
    }

    private IEnumerator ShowMain()
    {
        if (_difficultyButtons != null && _difficultyButtons.gameObject.activeInHierarchy)
        {
            _difficultyButtons.interactable = _difficultyButtons.blocksRaycasts = false;
            yield return Fade(_difficultyButtons, 0f);
            _difficultyButtons.gameObject.SetActive(false);
        }
        RefreshContinueVisibility();
        if (_mainButtonsRoot != null)
        {
            _mainButtonsRoot.SetActive(true);
        }
        else if (_newGameButton != null)
        {
            _newGameButton.SetActive(true);
        }
        yield return FadeMain(1f, hideAfter: false);
        _choosingDifficulty = _fading = false;
        SelectMainDefault();
    }

    private IEnumerator FadeMain(float target, bool hideAfter)
    {
        if (target >= 1f)
        {
            SetMainVisible(true);
        }
        var groups = new System.Collections.Generic.List<CanvasGroup>();
        CanvasGroup mainGroup = MainGroup;
        if (mainGroup != null)
        {
            groups.Add(mainGroup);
        }
        else
        {
            AddButtonGroup(groups, _continueButton);
            AddButtonGroup(groups, _newGameButton);
            AddButtonGroup(groups, _settingsButton);
            AddButtonGroup(groups, _leaveButton);
        }
        if (groups.Count > 0)
        {
            foreach (CanvasGroup group in groups)
            {
                group.interactable = group.blocksRaycasts = false;
            }
            yield return FadeAll(groups, target);
            if (hideAfter)
            {
                SetMainVisible(false);
            }
            else
            {
                foreach (CanvasGroup group in groups)
                {
                    group.interactable = group.blocksRaycasts = true;
                }
            }
            yield break;
        }
        SetMainVisible(!hideAfter);
    }

    private void SetMainVisible(bool visible)
    {
        if (_mainButtonsRoot != null)
        {
            _mainButtonsRoot.SetActive(visible);
        }
        else if (_newGameButton != null)
        {
            _newGameButton.SetActive(visible);
        }
    }

    private static void AddButtonGroup(System.Collections.Generic.List<CanvasGroup> groups, GameObject button)
    {
        if (button != null && button.activeInHierarchy && button.TryGetComponent<CanvasGroup>(out var group))
        {
            if (!groups.Contains(group))
            {
                groups.Add(group);
            }
        }
    }

    private IEnumerator FadeAll(System.Collections.Generic.List<CanvasGroup> groups, float target)
    {
        float[] starts = new float[groups.Count];
        for (int i = 0; i < groups.Count; i++)
        {
            starts[i] = groups[i].alpha;
        }
        float elapsed = 0f;
        while (elapsed < _difficultyFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / _difficultyFadeDuration);
            for (int i = 0; i < groups.Count; i++)
            {
                groups[i].alpha = Mathf.Lerp(starts[i], target, t);
            }
            yield return null;
        }
        foreach (CanvasGroup group in groups)
        {
            group.alpha = target;
        }
    }

    private IEnumerator Fade(CanvasGroup group, float target)
    {
        float start = group.alpha;
        float elapsed = 0f;
        while (elapsed < _difficultyFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, target, Mathf.Clamp01(elapsed / _difficultyFadeDuration));
            yield return null;
        }
        group.alpha = target;
    }

    public void OnClickEasy() => StartNewGame(Difficulty.Easy);
    public void OnClickNormal() => StartNewGame(Difficulty.Normal);
    public void OnClickHard() => StartNewGame(Difficulty.Hard);

    public void OnClickSettings()
    {
        if (_choosingDifficulty || _fading || GameManager.IsLoading || GameIntroCinematic.IsTransitioning)
        {
            return;
        }
        if (_settingsPanel == null)
        {
            _onSettingsRequested.Invoke();
            return;
        }
        if (_isSettingsOpen)
        {
            OnClickCloseSettings();
            return;
        }
        _isSettingsOpen = _fading = true;
        StartCoroutine(ShowSettings());
    }

    private IEnumerator ShowSettings()
    {
        if (_hideMainWhileSettings)
        {
            yield return FadeMain(0f, hideAfter: true);
        }
        _settingsPanel.SetActive(true);
        if (_settingsPanel.TryGetComponent<CanvasGroup>(out var group))
        {
            group.interactable = group.blocksRaycasts = false;
            yield return Fade(group, 1f);
            group.interactable = group.blocksRaycasts = true;
        }
        _fading = false;
        _onSettingsRequested.Invoke();
    }

    public void OnClickCloseSettings()
    {
        if (!_isSettingsOpen || _fading)
        {
            return;
        }
        _fading = true;
        StartCoroutine(HideSettingsAndShowMain());
    }

    private IEnumerator HideSettingsAndShowMain()
    {
        if (_settingsPanel != null && _settingsPanel.TryGetComponent<CanvasGroup>(out var group)
            && _settingsPanel.activeInHierarchy)
        {
            group.interactable = group.blocksRaycasts = false;
            yield return Fade(group, 0f);
            _settingsPanel.SetActive(false);
        }
        else if (_settingsPanel != null)
        {
            _settingsPanel.SetActive(false);
        }
        _isSettingsOpen = false;
        RefreshContinueVisibility();
        if (_mainButtonsRoot != null)
        {
            _mainButtonsRoot.SetActive(true);
        }
        else if (_newGameButton != null)
        {
            _newGameButton.SetActive(true);
        }
        yield return FadeMain(1f, hideAfter: false);
        _fading = false;
        SelectMainDefault();
    }

    private void StartNewGame(Difficulty difficulty)
    {
        if (!_choosingDifficulty || _fading)
        {
            return;
        }
        if (GameManager.LoadGameScene(_gameSceneName, true, _introClip, difficulty: difficulty, introSkipHoldDuration: _introSkipHoldDuration))
        {
            _difficultyButtons.interactable = _difficultyButtons.blocksRaycasts = false;
        }
    }
    public void OnClickLeave()
    {
#if UNITY_EDITOR
        Debug.Log("[OutGameUI] LeaveButton 클릭: 에디터에서는 종료하지 않습니다.");
#else
        Application.Quit();
#endif
    }
}
