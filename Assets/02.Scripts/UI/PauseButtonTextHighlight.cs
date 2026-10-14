using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class PauseButtonTextHighlight : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TMP_Text _label;
    [Tooltip("연결하면 공통 색상을 사용합니다. 비워 두면 기존 개별 글자 색상과 Button 설정을 유지합니다.")]
    [SerializeField] private UIButtonColorTheme _theme;
    [SerializeField] private Color _normalColor = new Color(0.52f, 0.60f, 0.67f, 1f);
    [SerializeField] private Color _highlightColor = new Color(1f, 0.96f, 0.88f, 1f);
    [SerializeField, Min(0f)] private float _blendDuration = 0.12f;

    private Button _button;
    private bool _selected;

    private Color NormalColor => _theme != null ? _theme.NormalTextColor : _normalColor;
    private Color HighlightColor => _theme != null ? _theme.HighlightTextColor : _highlightColor;
    private float BlendDuration => _theme != null ? _theme.TextBlendDuration : _blendDuration;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_label == null)
        {
            _label = GetComponentInChildren<TMP_Text>(true);
        }
    }

    private void OnEnable()
    {
        ApplyButtonTheme();
        _selected = EventSystem.current != null &&
                    EventSystem.current.currentSelectedGameObject == gameObject;
        if (_label != null)
        {
            _label.color = _selected && _button != null && _button.IsInteractable()
                ? HighlightColor : NormalColor;
        }
    }

    private void Update()
    {
        ApplyButtonTheme();
        if (_label == null) return;
        Color target = _selected && _button != null && _button.IsInteractable()
            ? HighlightColor
            : NormalColor;
        // 일시정지 중에도 색상 전환을 진행한다.
        float duration = BlendDuration;
        float step = duration > 0f ? Time.unscaledDeltaTime / duration : 1f;
        Color color = _label.color;
        color.r = Mathf.MoveTowards(color.r, target.r, step);
        color.g = Mathf.MoveTowards(color.g, target.g, step);
        color.b = Mathf.MoveTowards(color.b, target.b, step);
        color.a = target.a;
        _label.color = color;
    }

    private void ApplyButtonTheme()
    {
        if (_theme == null || _button == null) return;
        if (_button.transition != UnityEngine.UI.Selectable.Transition.ColorTint)
        {
            _button.transition = UnityEngine.UI.Selectable.Transition.ColorTint;
        }
        // SO를 Play 중 수정해도 반영하되, 같은 색을 매 프레임 다시 적용하지 않는다.
        if (_button.colors != _theme.ButtonColors)
        {
            _button.colors = _theme.ButtonColors;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_button == null || !_button.IsInteractable()) return;
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
        else
        {
            _selected = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _selected = false;
        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OnSelect(BaseEventData eventData) => _selected = true;
    public void OnDeselect(BaseEventData eventData) => _selected = false;

    private void OnDisable()
    {
        _selected = false;
        if (_label != null)
        {
            _label.color = NormalColor;
        }
    }
}
