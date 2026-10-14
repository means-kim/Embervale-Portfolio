using UnityEngine;
using UnityEngine.EventSystems;

// 강화 UI가 ESC를 처리하기 전에 현재 열려 있는지 확인한다.
[DefaultExecutionOrder(-100)]
public sealed class InGamePauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _resumeButton;
    [SerializeField] private Health _playerHealth;
    [SerializeField] private Behaviour[] _playerControls = new Behaviour[0];
    [SerializeField] private CinematicManager _cinematics;
    [SerializeField] private BossPhase3Cinematic _phase3;
    [SerializeField] private string _mainMenuSceneName = "OutGameScene";

    public bool IsOpen { get; private set; }
    private bool[] _controlStates;
    private float _previousTimeScale;
    private bool _previousAudioPause;
    private GameObject _previousSelection;

    private void Awake()
    {
        if (_panel != null)
        {
            _panel.SetActive(false);
        }
    }

    private void Update()
    {
        if (GameManager.IsLoading)
        {
            return;
        }

        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }
        if (IsOpen)
        {
            Resume();
        }
        else
        {
            TryOpen();
        }
    }

    public bool TryOpen()
    {
        if (IsOpen || _panel == null || _playerHealth == null || _playerHealth.IsDead ||
            !Mathf.Approximately(Time.timeScale, 1f) || GameManager.IsLoading ||
            GameIntroCinematic.IsTransitioning ||
            CinematicSkipController.IsActive ||
            (UI_Ability.Instance != null && UI_Ability.Instance.IsOpen) ||
            (_cinematics != null && _cinematics.IsPlaying) ||
            (_phase3 != null && _phase3.IsPlaying) ||
            (HitImpactFeedback.Instance != null && HitImpactFeedback.Instance.IsStopping))
        {
            return false;
        }

        _previousTimeScale = Time.timeScale;
        _previousAudioPause = AudioListener.pause;
        _controlStates = new bool[_playerControls.Length];
        for (int i = 0; i < _playerControls.Length; i++)
        {
            if (_playerControls[i] != null)
            {
                _controlStates[i] = _playerControls[i].enabled;
            }
        }
        IsOpen = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        foreach (var control in _playerControls)
        {
            if (control != null)
            {
                control.enabled = false;
            }
        }
        _panel.SetActive(true);
        if (EventSystem.current != null)
        {
            _previousSelection = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(_resumeButton);
        }
        return true;
    }

    public void Resume()
    {
        if (!IsOpen)
        {
            return;
        }
        IsOpen = false;
        _panel.SetActive(false);
        Time.timeScale = _previousTimeScale;
        AudioListener.pause = _previousAudioPause;
        for (int i = 0; i < _playerControls.Length; i++)
        {
            if (_playerControls[i] != null)
            {
                _playerControls[i].enabled = _controlStates[i];
            }
        }
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(_previousSelection);
        }
    }

    public void ReturnToMainMenu()
    {
        if (!IsOpen || GameManager.IsLoading)
        {
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(_mainMenuSceneName))
        {
            Debug.LogError("일시정지 메뉴: 타이틀 씬이 Build Profiles에 없습니다: " + _mainMenuSceneName, this);
            return;
        }

        // 다음 씬에 일시정지 시간과 오디오 상태가 남지 않게 먼저 복구한다.
        Resume();
        if (!GameManager.LoadGameScene(_mainMenuSceneName))
        {
            TryOpen();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("게임 종료: 빌드에서는 게임이 종료됩니다. 에디터에서는 Play를 유지합니다.", this);
#else
        Application.Quit();
#endif
    }

    private void OnDisable()
    {
        Resume();
    }
}
