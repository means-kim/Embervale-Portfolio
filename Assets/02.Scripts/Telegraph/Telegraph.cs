using UnityEngine;

public class Telegraph : MonoBehaviour
{
    [SerializeField] private TelegraphType _type;

    private bool _isReserved;
    private Vector3 _defaultScale;

    public TelegraphType Type => _type;
    public bool IsReserved => _isReserved;

    private Quaternion _defaultLocalRotation;

    private void Awake()
    {
        _defaultScale = transform.localScale;
        _defaultLocalRotation = transform.localRotation;
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
    }

    public void Release()
    {
        gameObject.SetActive(false);

        transform.localScale = _defaultScale;
        _isReserved = false;
        transform.localRotation = _defaultLocalRotation;
    }
}