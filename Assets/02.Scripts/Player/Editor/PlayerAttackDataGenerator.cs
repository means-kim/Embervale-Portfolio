using UnityEditor;
using UnityEngine;

/// <summary>
/// 기존 하드코딩 5종과 같은 값의 공격 SO 세트를 만든다.
/// Tools/Player/Generate Default Attack Set 1회 실행 → 씬 PlayerAttack이 비어 있으면 자동 연결.
/// 같은 이름 에셋이 있으면 덮어쓰지 않고 재사용한다.
/// </summary>
public static class PlayerAttackDataGenerator
{
    private const string FolderPath = "Assets/07.Datas/Player/Attacks";

    private struct Spec
    {
        public string fileName;
        public PlayerAttackType type;
        public KeyCode key;
        public KeyCode altKey;
        public Vector2 size;
        public Vector2 offset;
        public Vector2 vfxOffset;
        public float duration;
        public string clip;
        public bool withProjectile;
    }

    [MenuItem("Tools/Player/Generate Default Attack Set")]
    public static void Generate()
    {
        EnsureFolder("Assets/07.Datas", "Player");
        EnsureFolder("Assets/07.Datas/Player", "Attacks");

        var specs = new[]
        {
            new Spec { fileName = "PlayerAttack_Side", type = PlayerAttackType.SideAttack,
                key = KeyCode.Alpha5, altKey = KeyCode.Keypad5,
                size = new Vector2(1.4f, 1.2f), offset = new Vector2(0.8f, 0.8f), vfxOffset = new Vector2(0.8f, 0.8f),
                duration = 0.38f, clip = "SideAttack" },
            new Spec { fileName = "PlayerAttack_Stab", type = PlayerAttackType.Stab,
                key = KeyCode.Alpha1, altKey = KeyCode.Keypad1,
                size = new Vector2(2.2f, 0.8f), offset = new Vector2(1.1f, 0.8f), vfxOffset = new Vector2(1.1f, 0.8f),
                duration = 0.67f, clip = "StabAttack", withProjectile = true },
            new Spec { fileName = "PlayerAttack_Horizontal", type = PlayerAttackType.Horizontal,
                key = KeyCode.Alpha2, altKey = KeyCode.Keypad2,
                size = new Vector2(1.4f, 1.2f), offset = new Vector2(0.8f, 0.8f), vfxOffset = new Vector2(0.8f, 0.8f),
                duration = 1.17f, clip = "HorizontalAttack" },
            new Spec { fileName = "PlayerAttack_Down", type = PlayerAttackType.Down,
                key = KeyCode.Alpha3, altKey = KeyCode.Keypad3,
                size = new Vector2(1.2f, 1.6f), offset = new Vector2(0.2f, -1.0f), vfxOffset = new Vector2(0.2f, -1.0f),
                duration = 1.17f, clip = "DownAttack" },
            new Spec { fileName = "PlayerAttack_Up", type = PlayerAttackType.Up,
                key = KeyCode.Alpha4, altKey = KeyCode.Keypad4,
                size = new Vector2(1.2f, 1.8f), offset = new Vector2(0.2f, 1.2f), vfxOffset = new Vector2(0.2f, 1.2f),
                duration = 1.17f, clip = "UpAttack" },
        };

        GameObject stabProjectile = FindStabProjectile();
        var created = new PlayerAttackDataSO[specs.Length];
        for (int i = 0; i < specs.Length; i++)
        {
            var s = specs[i];
            string path = FolderPath + "/" + s.fileName + ".asset";
            var so = AssetDatabase.LoadAssetAtPath<PlayerAttackDataSO>(path);
            if (so == null)
            {
                so = ScriptableObject.CreateInstance<PlayerAttackDataSO>();
                so.attackType = s.type;
                so.triggerKey = s.key;
                so.altTriggerKey = s.altKey;
                so.hitbox = new PlayerAttackHitbox
                {
                    Size = s.size,
                    Offset = s.offset,
                    VfxOffset = s.vfxOffset,
                    Duration = s.duration
                };
                so.animClipName = s.clip;
                so.damage = 10;
                if (s.withProjectile) so.projectilePrefab = stabProjectile;
                AssetDatabase.CreateAsset(so, path);
                Debug.Log($"공격 SO 생성: {path}", so);
            }
            else
            {
                Debug.Log($"공격 SO 재사용 (덮어쓰지 않음): {path}", so);
            }
            created[i] = so;
        }

        if (stabProjectile == null)
            Debug.LogWarning("StabProjectile 프리팹을 못 찾아 Stab SO의 투사체가 비었다. 프리팹 만든 뒤 SO에 직접 연결해라.");
        TryWireScenePlayer(created);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void EnsureFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + name))
            AssetDatabase.CreateFolder(parent, name);
    }

    private static GameObject FindStabProjectile()
    {
        foreach (var guid in AssetDatabase.FindAssets("StabProjectile t:Prefab"))
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            if (prefab != null && prefab.GetComponent<StabProjectile>() != null)
                return prefab;
        }
        return null;
    }

    /// <summary>열린 씬의 PlayerAttack 목록이 비어 있을 때만 생성한 5종을 넣어준다.</summary>
    private static void TryWireScenePlayer(PlayerAttackDataSO[] sos)
    {
        var attacker = Object.FindFirstObjectByType<PlayerAttack>();
        if (attacker == null)
        {
            Debug.Log("씬에 PlayerAttack이 없어 자동 연결을 건너뛴다. SO를 직접 연결해라.");
            return;
        }
        var serialized = new SerializedObject(attacker);
        var list = serialized.FindProperty("_attacks");
        if (list == null)
        {
            Debug.LogWarning("PlayerAttack._attacks를 못 찾아 자동 연결을 건너뛴다.", attacker);
            return;
        }
        if (list.arraySize > 0)
        {
            Debug.Log("PlayerAttack 목록이 이미 있어 자동 연결을 건너뛴다.", attacker);
            return;
        }
        list.arraySize = sos.Length;
        for (int i = 0; i < sos.Length; i++)
            list.GetArrayElementAtIndex(i).objectReferenceValue = sos[i];
        serialized.ApplyModifiedProperties();
        Debug.Log("씬 PlayerAttack에 기본 5종을 연결했다.", attacker);
    }
}
