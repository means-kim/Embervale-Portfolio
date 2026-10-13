using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerWallSensor
{
    private readonly Rigidbody2D _body;
    private readonly List<Collider2D> _colliders = new();
    private readonly List<ContactPoint2D> _contacts = new();
    private readonly List<RaycastHit2D> _hits = new();

    public PlayerWallSensor(Rigidbody2D body) => _body = body;

    public bool HasWall(int direction, float probeDistance, float minimumNormal, Func<Collider2D, bool> ignored)
    {
        if (direction == 0 || _body == null || !_body.simulated)
        {
            return false;
        }
        _body.GetAttachedColliders(_colliders);
        foreach (var own in _colliders)
        {
            if (own == null || !own.enabled || own.isTrigger)
            {
                continue;
            }
            var filter = new ContactFilter2D { useTriggers = false };
            filter.SetLayerMask(Physics2D.GetLayerCollisionMask(own.gameObject.layer));
            own.GetContacts(filter, _contacts);
            foreach (var contact in _contacts)
            {
                Collider2D other = contact.collider;
                if (contact.collider == own)
                {
                    other = contact.otherCollider;
                }
                if (Eligible(own, other, ignored) && IsFacingWall(contact.normal.x, direction, minimumNormal))
                {
                    return true;
                }
            }

            if (probeDistance <= 0f)
            {
                continue;
            }
            own.Cast(Vector2.right * direction, filter, _hits, probeDistance);
            foreach (var hit in _hits)
            {
                if (!Eligible(own, hit.collider, ignored) || !IsFacingWall(hit.normal.x, direction, minimumNormal))
                {
                    continue;
                }

                if (hit.distance <= 0f)
                {
                    Bounds a = own.bounds;
                    Bounds b = hit.collider.bounds;
                    float gap;
                    if (direction > 0)
                    {
                        gap = b.min.x - a.max.x;
                    }
                    else
                    {
                        gap = a.min.x - b.max.x;
                    }
                    if (gap < -0.01f || gap > probeDistance)
                    {
                        continue;
                    }
                    if (Mathf.Min(a.max.y, b.max.y) - Mathf.Max(a.min.y, b.min.y) <= 0.01f)
                    {
                        continue;
                    }
                }
                return true;
            }
        }
        return false;
    }

    private static bool IsFacingWall(float normalX, int direction, float minimumNormal) => -normalX * direction >= minimumNormal;

    private bool Eligible(Collider2D own, Collider2D other, Func<Collider2D, bool> ignored)
    {
        if (other == null || !other.enabled || !other.gameObject.activeInHierarchy || other.isTrigger)
        {
            return false;
        }
        if (other.attachedRigidbody == _body)
        {
            return false;
        }
        if (!other.CompareTag("Ground") && !other.CompareTag("Platform"))
        {
            return false;
        }
        if (Physics2D.GetIgnoreCollision(own, other) || ignored(other))
        {
            return false;
        }
        return true;
    }
}
