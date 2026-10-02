using System;
using UnityEngine;
[Serializable]
public struct PlayerAttackHitbox
{
    [Min(0f)] public Vector2 Size;
    [Tooltip("판정 중심이자 VFX/투사체/근접 히트박스 스폰 위치. 방향에 따라 x가 뒤집힌다.")]
    public Vector2 Offset;
    [Min(0.05f)] public float Duration;
}