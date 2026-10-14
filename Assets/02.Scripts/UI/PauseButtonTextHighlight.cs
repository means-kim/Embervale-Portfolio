using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class PauseButtonTextHighlight : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TMP_Text _label;
    [SerializeField] private Color _normalColor = new Color(0.52f, 0.60f, 0.67f, 1f);
    [SerializeField] private Color _highlightColor = new Color(1f, 0.96f, 0.88f, 1f);
    [SerializeField, Min(0f)] private float _blendDuration = 0.12f;

    private Button _button;
    private bool _selected;

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
        _selected = EventSystem.current != null &&
                    EventSystem.current.currentSelectedGameObject == gameObject;
        if (_label != null)
        {
            _label.color = _selected ? _highlightColor : _normalColor;
        }
    }

    private void Update()
    {
        if (_label == null) return;
        Color target = _selected && _button != null && _button.IsInteractable()
            ? _highlightColor
            : _normalColor;
        // 일시정지 중에도 색상 전환을 진행한다.
        float step = _blendDuration > 0f ? Time.unscaledDeltaTime / _blendDuration : 1f;
        Color color = _label.color;
        color.r = Mathf.MoveTowards(color.r, target.r, step);
        color.g = Mathf.MoveTowards(color.g, target.g, step);
        color.b = Mathf.MoveTowards(color.b, target.b, step);
        color.a = target.a;
        _label.color = color;
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
            _label.color = _normalColor;
        }
    }
}