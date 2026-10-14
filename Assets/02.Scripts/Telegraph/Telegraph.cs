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
    private AirBossAttackBase _audioScope;
    private uint _audioGeneration;
    private bool _hasAudioScope;

    public void SetAudioScope(AirBossAttackBase scope)
    {
        _audioScope = scope;
        _hasAudioScope = scope != null;
        _audioGeneration = scope != null ? scope.AudioGeneration : 0;
    }

    private void OnDisable() => AttackOneShotAudio.StopOwned(this);
    private void OnDestroy() => AttackOneShotAudio.StopOwned(this);

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
        if (!_hasAudioScope || (_audioScope != null && _audioScope.AudioActive && _audioScope.AudioGeneration == _audioGeneration))
            AttackOneShotAudio.Play(_warningSound, owner: this, scope: _audioScope);
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
        AttackOneShotAudio.StopOwned(this);
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
