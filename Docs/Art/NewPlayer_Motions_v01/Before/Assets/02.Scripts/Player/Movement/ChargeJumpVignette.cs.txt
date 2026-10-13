using UnityEngine;
using UnityEngine.UI;

// Canvas의 전체 화면 Image에 연결합니다. 외부 이미지 없이 그라데이션을 한 번 생성합니다.
[DisallowMultipleComponent]
[RequireComponent(typeof(Image), typeof(CanvasGroup))]
public class ChargeJumpVignette : MonoBehaviour
{
    [SerializeField] private PlayerChargeJump _player;
    [SerializeField, Range(0f, 1f)] private float _opacity = 0.75f;
    [SerializeField, Min(0.01f)] private float _fadeInDuration = 0.12f;
    [SerializeField, Min(0.01f)] private float _fadeOutDuration = 0.18f;
    [SerializeField, Range(0f, 0.95f)] private float _clearRadius = 0.45f;
    private Image _image;
    private CanvasGroup _group;
    private Texture2D _texture;
    private Sprite _sprite;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _group = GetComponent<CanvasGroup>();
        _image.raycastTarget = false;
        _group.blocksRaycasts = false;
        _group.interactable = false;
        _group.alpha = 0f;
        _image.color = Color.white;
        _image.type = Image.Type.Simple;
        _image.preserveAspect = false;
        const int width = 256, height = 144;
        _texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            name = "ChargeJumpVignette_Runtime",
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        var pixels = new Color32[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float nx = 2f * x / (width - 1f) - 1f;
                float ny = 2f * y / (height - 1f) - 1f;
                float radius = Mathf.Sqrt(nx * nx + ny * ny);
                float alpha = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(_clearRadius, 1f, radius));
                pixels[y * width + x] = new Color32(0, 0, 0, (byte)Mathf.RoundToInt(alpha * 255f));
            }
        }
        _texture.SetPixels32(pixels);
        _texture.Apply(false, true);
        _sprite = Sprite.Create(_texture, new Rect(0, 0, width, height), Vector2.one * 0.5f);
        _image.sprite = _sprite;
    }

    private void Update()
    {
        float target = _player != null && _player.IsAiming ? _opacity : 0f;
        float duration = target > _group.alpha ? _fadeInDuration : _fadeOutDuration;
        _group.alpha = Mathf.MoveTowards(_group.alpha, target, _opacity * Time.unscaledDeltaTime / duration);
    }

    private void OnDisable()
    {
        if (_group != null) _group.alpha = 0f;
    }

    private void OnDestroy()
    {
        if (_sprite != null) Destroy(_sprite);
        if (_texture != null) Destroy(_texture);
    }
}
