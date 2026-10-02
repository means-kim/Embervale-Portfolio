using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerAttack))]
public class PlayerAttackEditor : Editor
{
    private void OnSceneGUI()
    {
        var attack = (PlayerAttack)target;
        if (attack == null || !attack.DebugDrawEnabled) return;
        if (Application.isPlaying)
        {
            if (attack.TryGetDebugBox(out var type, out var center, out var size))
            {
                Handles.color = Color.white;
                Handles.DrawWireCube(center, size);
                Handles.Label(center, type + " 실제 판정 bounds");
            }
            return;
        }
        var input = attack.GetComponent<PlayerInput>();
        if (input == null) return;
        foreach (var binding in input.AttackBindings)
        {
            if (binding?.attack?.attackPrefab == null) continue;
            var definition = binding.attack;
            for (int direction = -1; direction <= 1; direction += 2)
            {
                var root = definition.attackPrefab.transform;
                var scale = root.localScale;
                scale.x = Mathf.Abs(scale.x) * direction;
                Vector3 origin = attack.transform.position + new Vector3(direction * definition.spawnOffset.x, definition.spawnOffset.y);
                Matrix4x4 spawn = Matrix4x4.TRS(origin, Quaternion.identity, scale);
                Handles.color = direction == 1 ? Color.cyan : new Color(1, 0.7f, 0.3f, 0.45f);
                foreach (var collider in definition.attackPrefab.GetComponentsInChildren<Collider2D>())
                {
                    Matrix4x4 matrix = spawn * root.worldToLocalMatrix * collider.transform.localToWorldMatrix;
                    DrawCollider(collider, matrix);
                }
                Handles.Label(origin, definition.name + " / " + binding.key + " 스폰 판정");
            }
        }
    }
    private static void DrawCollider(Collider2D collider, Matrix4x4 matrix)
    {
        if (collider is PolygonCollider2D polygon)
        {
            for (int path = 0; path < polygon.pathCount; path++)
            {
                var points = polygon.GetPath(path);
                for (int i = 0; i < points.Length; i++) Handles.DrawLine(matrix.MultiplyPoint3x4(points[i] + polygon.offset), matrix.MultiplyPoint3x4(points[(i + 1) % points.Length] + polygon.offset));
            }
        }
        else if (collider is CircleCollider2D circle)
        {
            float scale = Mathf.Max(matrix.MultiplyVector(Vector3.right).magnitude, matrix.MultiplyVector(Vector3.up).magnitude);
            Handles.DrawWireDisc(matrix.MultiplyPoint3x4(circle.offset), Vector3.forward, circle.radius * scale);
        }
        else
        {
            Vector2 size = collider is CapsuleCollider2D capsule ? capsule.size : collider is BoxCollider2D box ? box.size : Vector2.zero;
            using (new Handles.DrawingScope(matrix)) Handles.DrawWireCube(collider.offset, size);
        }
    }
}
