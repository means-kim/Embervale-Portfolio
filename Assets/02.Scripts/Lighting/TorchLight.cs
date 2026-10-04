using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchLight : MonoBehaviour
{
    [SerializeField]
    private float _duration = 10f;

    [SerializeField] private Light2D _light;
    [SerializeField] private float _targetIntensity = 1.5f;
    [SerializeField] private float _fadeDuration = 0.4f;
    [SerializeField] private ParticleSystem _fireEffect;
    [SerializeField] private GameObject _flame;
    [SerializeField] private string _fireInteract = "FireInteract";
    [SerializeField] private string _unFireInteract = "UnFireInteract";

    private Coroutine _routine;
    private WaitForSeconds _litWait;
    private float _baseIntensity;
    public bool IsLit { get; private set; }
    private void Awake()
    {
        if (_duration > 0f)
        {
            _litWait = new WaitForSeconds(_duration);
        }
        SetFire(false);
        ResetLight();
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }
        _routine = null;
        SetFire(false);
        ResetLight();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(_fireInteract))
        {
            FireActive();
        }
        else if (other.gameObject.CompareTag(_unFireInteract))
        {
            FireUnActive();
        }
    }

    private void FireActive()
    {
        SetFire(true);
        StartRoutine(FireOnRoutine());
    }

    private void FireUnActive()
    {
        if (!IsLit)
        {
            return;
        }
        SetFire(false);
        StartRoutine(FadeTo(0f));
    }

    private IEnumerator FireOnRoutine()
    {
        yield return FadeTo(_targetIntensity);
        if (_litWait == null)
        {
            yield break;
        }

        yield return _litWait;

        SetFire(false);
        yield return FadeTo(0f);
    }

    private IEnumerator FadeTo(float target)
    {
        _light.enabled = true;
        float start = _baseIntensity;

        for (float t = 0f; t < _fadeDuration; t += Time.deltaTime)
        {
            _baseIntensity = Mathf.Lerp(start, target, t / _fadeDuration);
            _light.intensity = _baseIntensity;
            yield return null;
        }

        _baseIntensity = target;
        if (target <= 0f)
        {
            _light.enabled = false;
        }
        _light.intensity = _baseIntensity;
    }

    private void SetFire(bool lit)
    {
        IsLit = lit;

        if (_flame != null)
        {
            _flame.SetActive(lit);
        }

        if (_fireEffect == null)
        {
            return;
        }

        if (lit)
        {
            _fireEffect.Play();
        }
        else
        {
            _fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void ResetLight()
    {
        _baseIntensity = 0f;
        _light.intensity = 0f;
        _light.enabled = false;
    }

    private void StartRoutine(IEnumerator routine)
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }
        _routine = StartCoroutine(routine);
    }
}
