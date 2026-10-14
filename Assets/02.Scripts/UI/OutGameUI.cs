using System.Collections;
using UnityEngine;
using UnityEngine.Video;
public class OutGameUI : MonoBehaviour
{
    [SerializeField] private string _gameSceneName = "MainTestScene";
    [SerializeField] private VideoClip _introClip;
    [SerializeField] private GameObject _newGameButton;
    [SerializeField] private CanvasGroup _difficultyButtons;
    [SerializeField] private UnityEngine.UI.Button _easyButton;
    [SerializeField] private UnityEngine.UI.Button _normalButton;
    [SerializeField] private UnityEngine.UI.Button _hardButton;
    [SerializeField, Min(0f)] private float _difficultyFadeDuration = 0.3f;
    private bool _choosingDifficulty;
    private bool _fading;
    private bool _hasEnabled;

    private void OnDisable()
    {
        StopAllCoroutines();
        if (_choosingDifficulty && _newGameButton != null)
        {
            var group = _newGameButton.GetComponent<CanvasGroup>();
            if (group != null)
            {
                group.alpha = 1f;
                group.interactable = group.blocksRaycasts = true;
            }
        }
        _choosingDifficulty = _fading = false;
    }

    private void OnEnable()
    {
        _choosingDifficulty = _fading = false;
        if (_difficultyButtons != null)
        {
            _difficultyButtons.alpha = 0f;
            _difficultyButtons.interactable = _difficultyButtons.blocksRaycasts = false;
            _difficultyButtons.gameObject.SetActive(false);
        }
        if (_newGameButton != null)
        {
            _newGameButton.SetActive(true);
            if (_hasEnabled && _newGameButton.TryGetComponent<CanvasGroup>(out var group))
            {
                group.alpha = 1f;
                group.interactable = group.blocksRaycasts = true;
            }
        }
        _hasEnabled = true;
    }

    public void OnClickGameStart()
    {
        if (_choosingDifficulty)
        {
            return;
        }
        if (!GameIntroCinematic.HasStartedGame)
        {
            OnClickNewGame();
        }
        else
        {
            GameManager.LoadGameScene(_gameSceneName, introClip: _introClip);
        }
    }

    public void OnClickNewGame()
    {
        if (_choosingDifficulty || GameManager.IsLoading || GameIntroCinematic.IsTransitioning)
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
        CanvasGroup newGameGroup = _newGameButton != null ? _newGameButton.GetComponent<CanvasGroup>() : null;
        if (_newGameButton != null && _newGameButton.activeInHierarchy && newGameGroup != null)
        {
            newGameGroup.interactable = newGameGroup.blocksRaycasts = false;
            yield return Fade(newGameGroup, 0f);
            _newGameButton.SetActive(false);
        }
        _difficultyButtons.gameObject.SetActive(true);
        yield return Fade(_difficultyButtons, 1f);
        _difficultyButtons.interactable = _difficultyButtons.blocksRaycasts = true;
        _fading = false;
        _normalButton.Select();
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

    private void StartNewGame(Difficulty difficulty)
    {
        if (!_choosingDifficulty || _fading)
        {
            return;
        }
        if (GameManager.LoadGameScene(_gameSceneName, true, _introClip, difficulty: difficulty))
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
