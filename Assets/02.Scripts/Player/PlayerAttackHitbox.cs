using System;
using UnityEngine;
[Serializable]
public struct PlayerAttackHitbox
{
    [Min(0f)] public Vector2 Size;
    public Vector2 Offset;
    [Tooltip("VFX 소환 위치. 판정 Offset과 독립이라 판정을 옮겨도 이펙트는 안 따라간다.")]
    public Vector2 VfxOffset;
    [Min(0.05f)] public float Duration;
}
