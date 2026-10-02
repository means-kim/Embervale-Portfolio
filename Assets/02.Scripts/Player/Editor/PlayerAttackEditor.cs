using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(PlayerAttack))]
public class PlayerAttackEditor : Editor
{
    private void OnSceneGUI()
    {
        var atk = (PlayerAttack)target;
        if (atk == null || atk.Attacks == null) return;
        Vector3 basePos = atk.transform.position;
        foreach (var so in atk.Attacks)
        {
            if (so == null) continue;
            PlayerAttackType type = so.attackType;
            PlayerAttackHitbox box = so.hitbox;
            Color color = ColorOf(type);
            string label = string.IsNullOrEmpty(so.name) ? type.ToString() : so.name;
            if (so.projectilePrefab != null)
            {
                DrawProjectilePath(basePos, box, so, color, label);
            }
            else
            {
                DrawBox(basePos + new Vector3(box.Offset.x, box.Offset.y, 0f), box.Size, color, label);
                Color ghost = color;
                ghost.a = 0.25f;
                DrawBox(basePos + new Vector3(-box.Offset.x, box.Offset.y, 0f), box.Size, ghost, null);
            }
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
    private static void DrawProjectilePath(Vector3 basePos, PlayerAttackHitbox box, PlayerAttackDataSO so, Color color, string label)
    {
        float range = box.Size.x;
        var stab = so.projectilePrefab.GetComponent<IPlayerProjectile>();
        if (stab != null && stab.MaxDistance > 0f)
            range = stab.MaxDistance;
        Vector3 origin = basePos + new Vector3(box.Offset.x, box.Offset.y, 0f);
        Vector3 end = origin + new Vector3(range, 0f, 0f);
        Handles.color = color;
        Handles.DrawLine(origin, end, 3f);
        Handles.DrawWireCube((origin + end) * 0.5f, new Vector3(range, Mathf.Max(box.Size.y, 0.2f), 0f));
        Handles.Label(end + new Vector3(0.15f, 0f, 0f), label + " (projectile)");
        Color ghost = color;
        ghost.a = 0.25f;
        Handles.color = ghost;
        Vector3 morigin = basePos + new Vector3(-box.Offset.x, box.Offset.y, 0f);
        Handles.DrawLine(morigin, morigin + new Vector3(-range, 0f, 0f), 2f);
    }
    private static void DrawBox(Vector3 center, Vector2 size, Color color, string label)
    {
        Handles.color = color;
        Handles.DrawWireCube(center, new Vector3(size.x, size.y, 0f));
        if (!string.IsNullOrEmpty(label))
            Handles.Label(center + new Vector3(0f, size.y * 0.5f + 0.15f, 0f), label);
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
