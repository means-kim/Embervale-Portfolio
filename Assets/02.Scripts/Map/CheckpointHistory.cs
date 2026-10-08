using UnityEngine;
using UnityEngine.SceneManagement;

// Session-only checkpoint identity; scene objects themselves do not survive reloads.
public static class CheckpointHistory
{
    private static string _scenePath;
    private static string _checkpointPath;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Clear()
    {
        _scenePath = null;
        _checkpointPath = null;
    }

    public static void Record(Player player, Transform checkpoint)
    {
        if (player == null || checkpoint == null || player.gameObject.scene != checkpoint.gameObject.scene)
            return;

        _scenePath = checkpoint.gameObject.scene.path;
        _checkpointPath = GetPath(checkpoint);
    }

    public static bool Restore(Scene scene)
    {
        if (_checkpointPath == null || scene.path != _scenePath) return false;

        Player player = null;
        Transform checkpoint = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Player candidate in root.GetComponentsInChildren<Player>())
            {
                if (candidate.isActiveAndEnabled) player = candidate;
            }
            foreach (Transform candidate in root.GetComponentsInChildren<Transform>(true))
            {
                if (GetPath(candidate) == _checkpointPath) checkpoint = candidate;
            }
        }

        if (player == null || checkpoint == null)
        {
            Debug.LogWarning("CheckpointHistory: 재시작할 플레이어 또는 체크포인트를 찾을 수 없습니다.");
            return false;
        }

        RespawnPoint point = checkpoint.GetComponent<RespawnPoint>();
        if (point != null) player.SetRespawnPoint(point);
        else player.SetRespawnPoint(checkpoint.position);
        // The reloaded scene already reset combat. Only restore position and view.
        player.MoveToCheckpoint(checkpoint.position);
        return true;
    }

    private static string GetPath(Transform target)
    {
        int ordinal = 0;
        if (target.parent != null)
        {
            foreach (Transform sibling in target.parent)
            {
                if (sibling == target) break;
                if (sibling.name == target.name) ordinal++;
            }
        }
        else
        {
            foreach (GameObject root in target.gameObject.scene.GetRootGameObjects())
            {
                if (root.transform == target) break;
                if (root.name == target.name) ordinal++;
            }
        }

        string segment = target.name + "[" + ordinal + "]";
        return target.parent == null ? segment : GetPath(target.parent) + "/" + segment;
    }
}
