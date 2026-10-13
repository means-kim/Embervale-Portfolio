using System.Collections.Generic;
using UnityEngine;

internal sealed class ChargeJumpAim
{
    private readonly Transform _player;
    private readonly Collider2D _collider;
    private readonly List<Collider2D> _nearby = new();
    private ChargeJumpArrow _arrow;
    private GameObject _effect;

    public Vector3 Origin => _collider != null ? _collider.bounds.center : _player.position;
    public Vector2 Direction { get; private set; }

    public ChargeJumpAim(Transform player, Collider2D collider)
    {
        _player = player;
        _collider = collider;
    }

    public IceProjectile FindNearest(float radius, LayerMask layers)
    {
        var filter = new ContactFilter2D { useTriggers = true };
        filter.SetLayerMask(layers);
        _nearby.Clear();
        Physics2D.OverlapCircle(Origin, radius, filter, _nearby);
        IceProjectile nearest = null;
        float bestDistance = float.PositiveInfinity;
        foreach (var collider in _nearby)
        {
            if (collider == null) continue;
            var projectile = collider.GetComponentInParent<IceProjectile>();
            if (projectile == null || !projectile.CanBeCharged) continue;
            float distance = ((Vector2)Origin - collider.ClosestPoint(Origin)).sqrMagnitude;
            if (distance >= bestDistance) continue;
            bestDistance = distance;
            nearest = projectile;
        }
        return nearest;
    }

    public void Begin(ChargeJumpArrow prefab, Camera camera, Vector3 origin, int facing)
    {
        Direction = Vector2.right * facing;
        if (_arrow == null) _arrow = UnityEngine.Object.Instantiate(prefab);
        UpdateDirection(camera, origin);
        _arrow.Show(origin, Direction);
        _effect = EffectManager.Instance?.PlayChargeJumpAim(Origin);
    }

    public void UpdateDirection(Camera camera, Vector3 origin)
    {
        var plane = new Plane(Vector3.forward, origin);
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!plane.Raycast(ray, out float distance)) return;
        Vector2 direction = ray.GetPoint(distance) - origin;
        if (direction.sqrMagnitude > 0.0001f) Direction = direction.normalized;
    }

    public void Tick(Camera camera, Vector3 origin)
    {
        if (_arrow == null) return;
        UpdateDirection(camera, origin);
        _arrow.Show(origin, Direction);
        if (_effect != null) _effect.transform.position = Origin;
    }

    public void End()
    {
        if (_arrow != null) _arrow.Hide();
        if (_effect != null) UnityEngine.Object.Destroy(_effect);
        _effect = null;
    }

    public void Dispose()
    {
        End();
        if (_arrow != null) UnityEngine.Object.Destroy(_arrow.gameObject);
        _arrow = null;
    }
}
