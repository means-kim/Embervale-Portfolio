using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class ImageFadeIn : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private Image _targetImage;
    [SerializeField] private float _fadeDuration = 2.0f;
    [SerializeField] private float _awaitTime = 2.0f;
    private void Start()
    {
        if (_targetImage == null) _targetImage = GetComponent<Image>();
        StartCoroutine(FadeInRoutine());
    }
    private IEnumerator FadeInRoutine()
    {
        float elapsedTime = 0f;
        Color color = _targetImage.color;
        color.a = 0f;
        _targetImage.color = color;
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / _fadeDuration);
            _targetImage.color = color;
            yield return null;
        }
        color.a = 1f;
        _targetImage.color = color;
    }
}