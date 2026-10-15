using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Ability : MonoBehaviour
{
    public static UI_Ability Instance { get; private set; }
    public enum AbilityType { Health, Attack, AttackSpeed }

    [Serializable]
    public class Option
    {
        public AbilityType type;
        public Button button;
        public Image frame;
        public Image[] icons = Array.Empty<Image>();
        public GameObject glow;
        public GameObject rim;
        public Image[] pips = Array.Empty<Image>();
        [Min(0)] public int level;
        [Min(1)] public int maxLevel = 5;
        public bool available = true;
    }

    [Serializable] public class AbilityEvent : UnityEvent<int> { }

    [Header("화면")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private Option[] _options = Array.Empty<Option>();
    [SerializeField] private Sprite _pipFull;
    [SerializeField] private Sprite _pipEmpty;
    [SerializeField] private Sprite _pipNext;
    [SerializeField] private bool _allowCancel = true;

    [Header("선택 중 입력 잠금")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private PlayerAttack _playerAttack;
    [Header("테스트 플레이어 입력 잠금")]
    [SerializeField] private HyunseoTest.PlayerInput _testPlayerInput;
    [SerializeField] private HyunseoTest.PlayerMove _testPlayerMove;
    [SerializeField] private HyunseoTest.PlayerAttack _testPlayerAttack;

    [Header("능력치 적용 연결: 0 체력 / 1 공격력 / 2 공격속도")]
    [SerializeField] private AbilityEvent _onAbilityConfirmed = new AbilityEvent();
    [SerializeField] private UnityEvent _onClosed = new UnityEvent();
    public event Action<AbilityType> AbilityConfirmed;
    // 능력치 적용과 UI·입력 정리가 모두 끝난 뒤 발송한다.
    public event Action<AbilityType> AbilitySelectionCompleted;
    public event Action Closed;
    public bool IsOpen => _isOpen;
    public bool AllowCancel => _allowCancel;
    public int SelectedIndex => _selectedIndex;
    public bool HasSelectableOptions
    {
        get
        {
            for (int i = 0; i < _options.Length; i++) if (CanSelect(i)) return true;
            return false;
        }
    }

    public void SetAllowCancel(bool allow) => _allowCancel = allow;

    public void ConfigurePlayer(GameObject player)
    {
        if (_isOpen) Close();
        _playerInput = player != null ? player.GetComponent<PlayerInput>() : null;
        _playerMove = player != null ? player.GetComponent<PlayerMove>() : null;
        _playerAttack = player != null ? player.GetComponent<PlayerAttack>() : null;
        _testPlayerInput = player != null ? player.GetComponent<HyunseoTest.PlayerInput>() : null;
        _testPlayerMove = player != null ? player.GetComponent<HyunseoTest.PlayerMove>() : null;
        _testPlayerAttack = player != null ? player.GetComponent<HyunseoTest.PlayerAttack>() : null;
    }

    private bool _isOpen;
    private bool _confirming;
    private bool _inputWasEnabled;
    private bool _testInputWasEnabled;
    private bool _locksHeld;
    private int _selectedIndex = -1;
    private EventSystem _lockedEvents;
    private bool _navigationWasEnabled;
    private Image _background;
    private Color _backgroundColor;
    private float _flashRemaining;
    private const float FlashDuration = 0.25f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        if (_panel != null)
        {
            Transform background = _panel.transform.Find("BackgroundDim");
            if (background != null)
            {
                _background = background.GetComponent<Image>();
                if (_background != null) _backgroundColor = _background.color;
            }
            _panel.SetActive(false);
        }

        foreach (Option option in _options)
        {
            if (option == null || option.button == null) continue;
            switch (option.type)
            {
                case AbilityType.Health:
                    option.button.onClick.AddListener(SelectHealth);
                    break;
                case AbilityType.Attack:
                    option.button.onClick.AddListener(SelectAttack);
                    break;
                case AbilityType.AttackSpeed:
                    option.button.onClick.AddListener(SelectAttackSpeed);
                    break;
            }
            option.button.navigation = new Navigation { mode = Navigation.Mode.None };
        }
    }

    private void SelectHealth()
    {
        SelectAbility(AbilityType.Health);
    }
    private void SelectAttack()
    {
        SelectAbility(AbilityType.Attack);
    }
    private void SelectAttackSpeed()
    {
        SelectAbility(AbilityType.AttackSpeed);
    }

    private void SelectAbility(AbilityType type)
    {
        if (!_isOpen) return;
        for (int i = 0; i < _options.Length; i++)
        {
            if (!CanSelect(i) || _options[i].type != type)
            {
                continue;
            }
            Select(i);
            Confirm();
            return;
        }
    }

    private void Update()
    {
        if (!_isOpen)
        {
            return;
        }
        UpdateFlash();
        UpdateMouseSelection();
        if (_allowCancel && !CinematicSkipController.IsActive && Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    private void UpdateFlash()
    {
        if (_background == null || _flashRemaining <= 0f)
        {
            return;
        }
        _flashRemaining = Mathf.Max(0f, _flashRemaining - Time.unscaledDeltaTime);
        _background.color = Color.Lerp(_backgroundColor, new Color(1f, 1f, 1f, 0.85f),
            _flashRemaining / FlashDuration);
    }

    private void ResetFlash()
    {
        _flashRemaining = 0f;
        if (_background != null) _background.color = _backgroundColor;
    }

    private void UpdateMouseSelection()
    {
        int hoveredIndex = -1;
        for (int i = 0; i < _options.Length; i++)
        {
            if (!CanSelect(i) || _options[i].button == null) continue;
            Button button = _options[i].button;
            Canvas canvas = button.GetComponentInParent<Canvas>();
            Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            if (RectTransformUtility.RectangleContainsScreenPoint(
                button.transform as RectTransform, Input.mousePosition, camera))
            {
                hoveredIndex = i;
                break;
            }
        }
        if (_selectedIndex == hoveredIndex)
        {
            return;
        }
        _selectedIndex = hoveredIndex;
        Refresh();
    }

    public void Open()
    {
        if (_isOpen || _panel == null)
        {
            return;
        }
        _isOpen = true;
        _panel.SetActive(true);
        _flashRemaining = FlashDuration;
        if (_background != null)
        {
            _background.color = new Color(1f, 1f, 1f, 0.85f);
        }
        if (Application.isPlaying)
        {
            _lockedEvents = EventSystem.current;
            if (_lockedEvents != null)
            {
                _navigationWasEnabled = _lockedEvents.sendNavigationEvents;
                _lockedEvents.sendNavigationEvents = false;
            }
            if (_playerAttack != null && _playerAttack.gameObject.activeInHierarchy)
                _playerAttack.CancelAttack();
            if (_playerMove != null && _playerMove.gameObject.activeInHierarchy)
                _playerMove.SetMovementLocked(this, true);
            if (_testPlayerAttack != null && _testPlayerAttack.gameObject.activeInHierarchy)
                _testPlayerAttack.CancelAttack();
            if (_testPlayerMove != null && _testPlayerMove.gameObject.activeInHierarchy)
                _testPlayerMove.SetMovementLocked(this, true);
            if (_playerInput != null)
            {
                _inputWasEnabled = _playerInput.enabled;
                _playerInput.enabled = false;
            }
            if (_testPlayerInput != null)
            {
                _testInputWasEnabled = _testPlayerInput.enabled;
                _testPlayerInput.enabled = false;
            }
            _locksHeld = true;
        }
        _selectedIndex = -1;
        Refresh();
    }

    public void Select(int index)
    {
        if (!_isOpen || !CanSelect(index))
        {
            return;
        }
        _selectedIndex = index;
        Refresh();
    }

    public void Confirm()
    {
        if (_confirming || !_isOpen || !CanSelect(_selectedIndex))
        {
            return;
        }
        AbilityType type = _options[_selectedIndex].type;
        _confirming = true;
        try
        {
            AbilityConfirmed?.Invoke(type);
            _onAbilityConfirmed.Invoke((int)type);
            Close();
            AbilitySelectionCompleted?.Invoke(type);
        }
        finally
        {
            _confirming = false;
        }
    }

    public void Close()
    {
        bool wasOpen = _isOpen;
        _isOpen = false;
        ResetFlash();
        ReleaseInput();
        if (_panel != null)
        {
            _panel.SetActive(false);
        }
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null
            && _panel != null && EventSystem.current.currentSelectedGameObject.transform.IsChildOf(_panel.transform))
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        if (wasOpen)
        {
            _onClosed.Invoke();
            Closed?.Invoke();
        }
    }

    public void SetLevel(AbilityType type, int level)
    {
        foreach (Option option in _options)
        {
            if (option != null && option.type == type)
            {
                option.level = Mathf.Clamp(level, 0, Mathf.Max(1, option.maxLevel));
            }
        }

        if (!CanSelect(_selectedIndex))
        {
            _selectedIndex = -1;
        }
        Refresh();
    }

    public void SetProgress(AbilityType type, int level, int maxLevel)
    {
        foreach (Option option in _options)
        {
            if (option != null && option.type == type)
            {
                option.maxLevel = Mathf.Max(1, maxLevel);
            }
        }
        SetLevel(type, level);
    }

    public int GetLevel(AbilityType type)
    {
        foreach (Option option in _options)
        {
            if (option != null && option.type == type)
            {
                return option.level;
            }
        }
        return 0;
    }

    private bool CanSelect(int index)
    {
        return index >= 0 && index < _options.Length && _options[index] != null
            && _options[index].available && _options[index].level < Mathf.Max(1, _options[index].maxLevel);
    }

    public void Refresh()
    {
        for (int i = 0; i < _options.Length; i++)
        {
            Option option = _options[i];
            if (option == null)
            {
                continue;
            }
            bool available = CanSelect(i);
            bool selected = _isOpen && i == _selectedIndex && available;
            if (option.button != null)
            {
                option.button.interactable = available;
            }
            if (option.frame != null)
            {
                option.frame.color = available ? Color.white : new Color(.4f, .4f, .43f, 1f);
            }
            foreach (Image icon in option.icons)
            {
                if (icon != null)
                {
                    icon.color = available ? Color.white : new Color(.32f, .32f, .36f, .45f);
                }
            }

            option.glow?.SetActive(selected);
            option.rim?.SetActive(selected);
            for (int n = 0; n < option.pips.Length; n++)
            {
                Image pip = option.pips[n];
                if (pip == null)
                {
                    continue;
                }
                pip.sprite = n < option.level ? _pipFull : n == option.level && available ? _pipNext : _pipEmpty;
                pip.preserveAspect = true;
                pip.color = selected ? new Color(1f, .3f, .3f, 1f) : available ? Color.white : new Color(.6f, .6f, .65f, .65f);
            }
        }
    }

    private void ReleaseInput()
    {
        if (!_locksHeld)
        {
            return;
        }
        if (_playerMove != null && _playerMove.gameObject.activeInHierarchy)
            _playerMove.SetMovementLocked(this, false);
        if (_testPlayerMove != null && _testPlayerMove.gameObject.activeInHierarchy)
            _testPlayerMove.SetMovementLocked(this, false);
        if (_playerInput != null)
        {
            _playerInput.enabled = _inputWasEnabled;
        }
        if (_testPlayerInput != null)
            _testPlayerInput.enabled = _testInputWasEnabled;
        if (_lockedEvents != null)
        {
            _lockedEvents.sendNavigationEvents = _navigationWasEnabled;
        }
        _lockedEvents = null;
        _locksHeld = false;
    }

    private void OnDisable()
    {
        _isOpen = false;
        ResetFlash();
        ReleaseInput();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }


        foreach (Option option in _options)
        {
            if (option == null || option.button == null) continue;
            option.button.onClick.RemoveListener(SelectHealth);
            option.button.onClick.RemoveListener(SelectAttack);
            option.button.onClick.RemoveListener(SelectAttackSpeed);
        }

    }

}
