using UnityEngine;
using System.Collections;

public class Telegraph : MonoBehaviour
{
    [SerializeField] private TelegraphType _type;
    [SerializeField] private SFXData _warningSound;

    private bool _isReserved;
    private Vector3 _defaultScale;

    public TelegraphType Type => _type;
    public bool IsReserved => _isReserved;

    private Quaternion _defaultLocalRotation;

    private TelegraphVisual _visual;
    private Coroutine _releaseSequence;

    private void Awake()
    {
        _defaultScale = transform.localScale;
        _defaultLocalRotation = transform.localRotation;
        _visual = GetComponentInChildren<TelegraphVisual>(true);
    }

    public bool TryReserve()
    {
        if (_isReserved || gameObject.activeSelf)
        {
            return false;
        }

        _isReserved = true;
        return true;
    }

    public void Activate(Vector3 position)
    {
        Activate(position, Vector3.one);
    }

    public void Activate(Vector3 position, Vector3 scaleMultiplier)
    {
        if (!_isReserved || gameObject.activeSelf) return;

        transform.position = position;
        transform.localScale = Vector3.Scale(
            _defaultScale,
            scaleMultiplier
        );

        gameObject.SetActive(true);
        AttackOneShotAudio.Play(_warningSound);
    }

    public void Release()
    {
        if (_releaseSequence != null)
        {
            StopCoroutine(_releaseSequence);
            _releaseSequence = null;
        }

        gameObject.SetActive(false);

        transform.localScale = _defaultScale;
        transform.localRotation = _defaultLocalRotation;
        _isReserved = false;
    }

    public void FadeOutAndRelease()
    {
        if (_releaseSequence != null) return;

        if (!gameObject.activeInHierarchy ||
            _visual == null ||
            !_visual.isActiveAndEnabled)
        {
            Release();
            return;
        }

        _releaseSequence = StartCoroutine(ReleaseSequence());
    }

    private IEnumerator ReleaseSequence()
    {
        yield return _visual.FadeOut();

        _releaseSequence = null;
        Release();
    }
}