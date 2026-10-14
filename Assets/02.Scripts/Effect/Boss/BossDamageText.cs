using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class BossDamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;

    [Header("표시 시간")]
    [SerializeField, Min(0f)] private float _fadeInDuration = 0.08f;
    [SerializeField, Min(0f)] private float _holdDuration = 0.25f;
    [SerializeField, Min(0f)] private float _fadeOutDuration = 0.35f;

    [Header("표시 위치")]
    [SerializeField] private Vector2 _positionJitter = new Vector2(0.4f, 0.15f);

    [SerializeField, Min(0f)] private float _riseDistance = 0.6f;

    private Coroutine _routine;

    public bool IsPlaying => _routine != null;

    private void Awake()
    {
        if (_text == null)
        {
            _text = GetComponent<TextMeshPro>();
        }

        _text.alpha = 0f;
    }

    public void Play(int damage, Vector3 position, bool isThirdComboHit)
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        position += new Vector3(
            Random.Range(-_positionJitter.x, _positionJitter.x),
            Random.Range(-_positionJitter.y, _positionJitter.y),
            0f);

        transform.position = position;

        float randomMultiplier;
        if (isThirdComboHit)
        {
            randomMultiplier = Random.Range(3000, 6001) / 1000f;
        }
        else
        {
            randomMultiplier = Random.Range(1000, 3001) / 1000f;
        }

        int displayDamage = Mathf.RoundToInt(damage * 1000f * randomMultiplier);

        _text.SetText("{0}", displayDamage);
        _text.alpha = 0f;

        _routine = StartCoroutine(PlayRoutine(position));
    }

    private IEnumerator PlayRoutine(Vector3 startPosition)
    {
        // IsPlaying을 확인하는 서비스와 시작 순서를 맞춘다.
        yield return null;

        float totalDuration = _fadeInDuration + _holdDuration + _fadeOutDuration;

        for (float elapsed = 0f; elapsed < totalDuration; elapsed += Time.unscaledDeltaTime)
        {
            float alpha;

            if (_fadeInDuration > 0f && elapsed < _fadeInDuration)
            {
                alpha = elapsed / _fadeInDuration;
            }
            else if (elapsed < _fadeInDuration + _holdDuration)
            {
                alpha = 1f;
            }
            else
            {
                float fadeElapsed = elapsed - _fadeInDuration - _holdDuration;

                alpha = _fadeOutDuration > 0f ? 1f - Mathf.Clamp01(fadeElapsed / _fadeOutDuration) : 0f;
            }

            _text.alpha = alpha;

            float progress = totalDuration > 0f ? Mathf.Clamp01(elapsed / totalDuration) : 1f;

            transform.position = startPosition + Vector3.up * (_riseDistance * progress);

            yield return null;
        }

        _text.alpha = 0f;
        _routine = null;
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        if (_text != null)
        {
            _text.alpha = 0f;
        }
    }
}
