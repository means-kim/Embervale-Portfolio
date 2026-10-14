using UnityEngine;

public sealed class PlayerChargeJumpVisual : MonoBehaviour
{
    private SpriteRenderer _sourceRenderer;
    private SpriteRenderer _chargeRenderer;
    private Vector2 _chargeDirection;
    private bool _active;
    private bool _previousForceRenderingOff;
    private MaterialPropertyBlock _visualProperties;

    public bool IsActive => _active;

    private void Awake()
    {
        _sourceRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
    }

    public void Begin(Vector2 direction)
    {
        if (!_active && _sourceRenderer != null)
        {
            _previousForceRenderingOff = _sourceRenderer.forceRenderingOff;
        }
        _active = true;
        _chargeDirection = direction;
        Sync();
    }

    public void End()
    {
        if (_active && _sourceRenderer != null)
        {
            _sourceRenderer.forceRenderingOff = _previousForceRenderingOff;
        }
        _active = false;
        if (_chargeRenderer != null)
        {
            _chargeRenderer.enabled = false;
        }
    }

    public SpriteRenderer Resolve(SpriteRenderer original)
    {
        if (!_active || original != _sourceRenderer)
        {
            return original;
        }
        Sync();
        if (_chargeRenderer != null)
        {
            return _chargeRenderer;
        }
        return original;
    }

    private void LateUpdate()
    {
        if (_active)
        {
            Sync();
        }
    }

    private void OnDisable()
    {
        End();
    }

    private void OnDestroy()
    {
        if (_chargeRenderer != null)
        {
            Destroy(_chargeRenderer.gameObject);
        }
    }

    private void Sync()
    {
        if (_sourceRenderer == null)
        {
            return;
        }
        if (_chargeRenderer == null)
        {
            var visual = new GameObject("ChargeJumpLaunchVisual");
            visual.transform.SetParent(_sourceRenderer.transform, false);
            _chargeRenderer = visual.AddComponent<SpriteRenderer>();
        }
        _sourceRenderer.forceRenderingOff = true;
        _chargeRenderer.gameObject.layer = _sourceRenderer.gameObject.layer;
        _chargeRenderer.enabled = _sourceRenderer.enabled && !_previousForceRenderingOff;
        _chargeRenderer.sprite = _sourceRenderer.sprite;
        _chargeRenderer.sharedMaterial = _sourceRenderer.sharedMaterial;
        _chargeRenderer.color = _sourceRenderer.color;
        _chargeRenderer.flipX = _sourceRenderer.flipX;
        _chargeRenderer.flipY = _sourceRenderer.flipY;
        _chargeRenderer.sortingLayerID = _sourceRenderer.sortingLayerID;
        _chargeRenderer.sortingOrder = _sourceRenderer.sortingOrder;
        _chargeRenderer.maskInteraction = _sourceRenderer.maskInteraction;
        _chargeRenderer.drawMode = _sourceRenderer.drawMode;
        _chargeRenderer.size = _sourceRenderer.size;
        _chargeRenderer.spriteSortPoint = _sourceRenderer.spriteSortPoint;
        _visualProperties ??= new MaterialPropertyBlock();
        _sourceRenderer.GetPropertyBlock(_visualProperties);
        _chargeRenderer.SetPropertyBlock(_visualProperties);
        Vector3 localDirection = _sourceRenderer.transform.InverseTransformVector(_chargeDirection);
        if (_sourceRenderer.flipX)
        {
            localDirection = -localDirection;
        }
        float angle = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;
        _chargeRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
