using UnityEngine;

public class AreaBlock : MonoBehaviour
{
    [SerializeField] private Collider2D _blockingCollider;

    private SpriteRenderer[] _visuals;

    public bool IsClosed { get; private set; }

    private void Awake()
    {
        if (_blockingCollider == null)
        {
            _blockingCollider = GetComponent<Collider2D>();
        }

        _visuals = GetComponentsInChildren<SpriteRenderer>(true);

        OpenImmediate();
    }

    [ContextMenu("Close")]
    public void Close()
    {
        SetBlocked(true);
    }

    [ContextMenu("Open Immediate")]
    public void OpenImmediate()
    {
        SetBlocked(false);
    }

    private void SetBlocked(bool blocked)
    {
        if (_blockingCollider == null)
        {
            return;
        }

        _blockingCollider.enabled = blocked;

        foreach (SpriteRenderer visual in _visuals)
        {
            visual.enabled = blocked;
        }

        IsClosed = blocked;
    }
}