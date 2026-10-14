using UnityEngine;
using UnityEngine.SceneManagement;

// Session-only checkpoint identity; scene objects themselves do not survive reloads.
public static class CheckpointHistory
{
    private static string _scenePath;
    private static string _checkpointPath;
    private static Vector3 _checkpointPosition;
    private static bool _hasCheckpoint;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Clear()
    {
        _scenePath = null;
        _checkpointPath = null;
        _checkpointPosition = Vector3.zero;
        _hasCheckpoint = false;
    }

    public static void Record(Player player, Transform checkpoint)
    {
        if (player == null || checkpoint == null || player.gameObject.scene != checkpoint.gameObject.scene)
            return;

        _scenePath = checkpoint.gameObject.scene.path;
        _checkpointPath = GetPath(checkpoint);
        _checkpointPosition = checkpoint.position;
        _hasCheckpoint = true;
    }

    // 씬을 지우기 전에 실제 플레이어가 가진 복귀 위치를 보존한다.
    public static void RecordCurrent(Player player)
    {
        if (player == null) return;
        _scenePath = player.gameObject.scene.path;
        _checkpointPosition = player.RespawnPosition;
        _checkpointPath = player.RespawnAnchor != null ? GetPath(player.RespawnAnchor.transform) : null;
        _hasCheckpoint = true;
    }

    public static bool Restore(Scene scene)
    {
        if (!_hasCheckpoint || scene.path != _scenePath) return false;

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

        if (player == null)
        {
            Debug.LogWarning("CheckpointHistory: 재시작할 플레이어 또는 체크포인트를 찾을 수 없습니다.");
            return false;
        }

        if (checkpoint == null)
        {
            if (_checkpointPath != null)
                Debug.LogWarning("CheckpointHistory: 체크포인트 오브젝트를 찾지 못해 저장된 월드 위치로 복원합니다.");
            player.SetRespawnPoint(_checkpointPosition);
            player.MoveToCheckpoint(_checkpointPosition);
            return true;
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
