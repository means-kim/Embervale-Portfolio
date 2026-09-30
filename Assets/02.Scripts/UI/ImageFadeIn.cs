using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ImageFadeIn : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private float _fadeDuration = 2.0f;
    [SerializeField] private float _awaitTime = 2.0f;
    [Tooltip("페이드 중 클릭만 막습니다. Button Disabled 틴트 현상을 피하기 위해 interactable은 건드리지 않습니다.")]
    [SerializeField] private bool _controlInteraction = true;

    private CanvasGroup _canvasGroup;
    private bool _initialBlocksRaycasts = true;

    private void Reset()
    {
        EnsureCanvasGroup();
    }

    private void Awake()
    {
        EnsureCanvasGroup();
        _initialBlocksRaycasts = _canvasGroup.blocksRaycasts;
        // 첫 프레임부터 안 보이게 미리 숨김 + 클릭 방지.
        // interactable은 건드리지 않음 (Button이 Disabled 색상[a≈0.5]으로 변해
        // 50%까지만 페이드되다 마지막에 뚝 끊기는 현상 방지).
        _canvasGroup.alpha = 0f;
        if (_controlInteraction)
            _canvasGroup.blocksRaycasts = false;
    }

    private void Start()
    {
        StartCoroutine(FadeInRoutine());
    }

    private void EnsureCanvasGroup()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private IEnumerator FadeInRoutine()
    {
        if (_canvasGroup == null)
            EnsureCanvasGroup();

        if (_awaitTime > 0f)
            yield return new WaitForSeconds(_awaitTime);

        float elapsedTime = 0f;
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Clamp01(elapsedTime / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
        if (_controlInteraction)
            _canvasGroup.blocksRaycasts = _initialBlocksRaycasts;
    }
}
