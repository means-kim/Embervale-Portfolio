using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ButtonColorTheme", menuName = "UI/Button Color Theme")]
public sealed class UIButtonColorTheme : ScriptableObject
{
    [Header("글자 색상")]
    [SerializeField] private Color _normalTextColor = new Color(133f / 255f, 153f / 255f, 171f / 255f, 1f);

    [SerializeField] private Color _highlightTextColor = new Color(1f, 230f / 255f, 204f / 255f, 1f);

    [SerializeField, Min(0f)] private float _textBlendDuration = 0.12f;

    [Header("버튼 이미지 색상 / 밝기")]
    [SerializeField]
    private ColorBlock _buttonColors = new ColorBlock
    {
        normalColor = new Color(0.55f, 0.62f, 0.7f, 1f),
        highlightedColor = Color.white,
        selectedColor = Color.white,
        pressedColor = new Color(1f, 0.7f, 0.4f, 1f),
        disabledColor = new Color(0.78431374f, 0.78431374f, 0.78431374f, 0.5019608f),
        colorMultiplier = 1f,
        fadeDuration = 0.15f
    };

    public Color NormalTextColor => _normalTextColor;
    public Color HighlightTextColor => _highlightTextColor;
    public float TextBlendDuration => _textBlendDuration;
    public ColorBlock ButtonColors => _buttonColors;
}