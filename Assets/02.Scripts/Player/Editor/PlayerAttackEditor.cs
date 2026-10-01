using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 씬뷰에 5개 공격 판정을 타입별 색+라벨로 표시한다. 오른쪽 실선, 왼쪽 반투명.
/// 런타임 무영향(에디터 전용). 인스펙터 숫자 변경이 즉시 반영된다.
/// </summary>
[CustomEditor(typeof(PlayerAttack))]
public class PlayerAttackEditor : Editor
{
    private void OnSceneGUI()
    {
        var atk = (PlayerAttack)target;
        if (atk == null) return;
        Vector3 basePos = atk.transform.position;

        foreach (PlayerAttackType type in (PlayerAttackType[])Enum.GetValues(typeof(PlayerAttackType)))
        {
            PlayerAttackHitbox box = atk.GetHitbox(type);
            Color color = ColorOf(type);
            DrawBox(basePos + new Vector3(box.Offset.x, box.Offset.y, 0f), box.Size, color, type.ToString());
            Color ghost = color;
            ghost.a = 0.25f;
            DrawBox(basePos + new Vector3(-box.Offset.x, box.Offset.y, 0f), box.Size, ghost, null);
            // VFX 소환점(판정과 독립). × 표식으로 표시.
            DrawMarker(basePos + new Vector3(box.VfxOffset.x, box.VfxOffset.y, 0f), color, type + " VFX");
            DrawMarker(basePos + new Vector3(-box.VfxOffset.x, box.VfxOffset.y, 0f), ghost, null);
        }

        if (atk.TryGetDebugBox(out PlayerAttackType debugType, out Vector2 debugCenter, out Vector2 debugSize))
        {
            Color color = ColorOf(debugType);
            color.a = 1f;
            Handles.color = Color.white;
            Handles.DrawWireCube(debugCenter, new Vector3(debugSize.x, debugSize.y, 0f));
            Handles.Label(debugCenter + Vector2.up * (debugSize.y * 0.5f + 0.3f), debugType + " HIT");
        }
    }

    private static void DrawBox(Vector3 center, Vector2 size, Color color, string label)
    {
        Handles.color = color;
        Handles.DrawWireCube(center, new Vector3(size.x, size.y, 0f));
        if (!string.IsNullOrEmpty(label))
            Handles.Label(center + new Vector3(0f, size.y * 0.5f + 0.15f, 0f), label);
    }

    private static void DrawMarker(Vector3 pos, Color color, string label)
    {
        const float r = 0.18f;
        Handles.color = color;
        Handles.DrawLine(pos + new Vector3(-r, -r, 0f), pos + new Vector3(r, r, 0f));
        Handles.DrawLine(pos + new Vector3(-r, r, 0f), pos + new Vector3(r, -r, 0f));
        if (!string.IsNullOrEmpty(label))
            Handles.Label(pos + new Vector3(r + 0.05f, 0f, 0f), label);
    }

    private static Color ColorOf(PlayerAttackType type)
    {
        switch (type)
        {
            case PlayerAttackType.Stab: return Color.yellow;
            case PlayerAttackType.Horizontal: return Color.cyan;
            case PlayerAttackType.Down: return Color.magenta;
            case PlayerAttackType.Up: return new Color(1f, 0.5f, 0f);
            default: return Color.green;
        }
    }
}
