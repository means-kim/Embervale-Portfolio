using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CinematicBars : MonoBehaviour
{
    [SerializeField] private CanvasGroup _cinematicCanvasGroup;
    [SerializeField] private float _fadeDuration = 0.5f;
    private Coroutine _fadeRoutine;

    private void Awake()
    {
        if (_cinematicCanvasGroup == null)
            _cinematicCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeIn()
    {
        StartFade(1f);
    }

    public void FadeOut()
    {
        StartFade(0f);
    }

    private void StartFade(float targetAlpha)
    {
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }

        _fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        _cinematicCanvasGroup.blocksRaycasts = true;

        float speed = 1f / Mathf.Max(_fadeDuration, 0.0001f);
        while (!Mathf.Approximately(_cinematicCanvasGroup.alpha, targetAlpha))
        {
            _cinematicCanvasGroup.alpha = Mathf.MoveTowards(
                _cinematicCanvasGroup.alpha, targetAlpha, speed * Time.unscaledDeltaTime);
            yield return null;
        }

        _cinematicCanvasGroup.alpha = targetAlpha;
        _cinematicCanvasGroup.blocksRaycasts = targetAlpha > 0f;
        _fadeRoutine = null;
    }
}
