using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

// Persist coordinates and identities, never references to scene objects.
public static class CheckpointHistory
{
    private const string SaveKey = "Progress.Checkpoint";

    [System.Serializable]
    private sealed class SaveData
    {
        public string scenePath;
        public string checkpointPath;
        public Vector3 checkpointPosition;
        public Vector3 continuePosition;
        public bool hasContinuePosition;
        public VisibilityData[] continueVisibility;
        public string continueCameraPath;
    }

    [System.Serializable]
    private sealed class VisibilityData
    {
        public string path;
        public bool inside;
        public bool outside;
    }

    private static string _scenePath;
    private static string _checkpointPath;
    private static Vector3 _checkpointPosition;
    private static bool _hasCheckpoint;
    private static Vector3 _continuePosition;
    private static bool _hasContinuePosition;
    private static VisibilityData[] _continueVisibility;
    private static string _continueCameraPath;
    public static bool HasSavedProgress => _hasCheckpoint || PlayerPrefs.HasKey(SaveKey);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSession()
    {
        _scenePath = null;
        _checkpointPath = null;
        _checkpointPosition = Vector3.zero;
        _hasCheckpoint = false;
        _hasContinuePosition = false;
        _continueVisibility = null;
        _continueCameraPath = null;
    }

    public static void Clear()
    {
        ResetSession();
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }

    private static void Save()
    {
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(new SaveData
        {
            scenePath = _scenePath,
            checkpointPath = _checkpointPath,
            checkpointPosition = _checkpointPosition,
            continuePosition = _continuePosition,
            hasContinuePosition = _hasContinuePosition,
            continueVisibility = _continueVisibility,
            continueCameraPath = _continueCameraPath
        }));
        PlayerPrefs.Save();
    }

    private static void Load()
    {
        if (_hasCheckpoint || !PlayerPrefs.HasKey(SaveKey)) return;
        try
        {
            SaveData data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(SaveKey));
            if (data == null || string.IsNullOrEmpty(data.scenePath)) return;
            _scenePath = data.scenePath;
            _checkpointPath = data.checkpointPath;
            _checkpointPosition = data.checkpointPosition;
            _continuePosition = data.continuePosition;
            _hasContinuePosition = data.hasContinuePosition;
            _continueVisibility = data.continueVisibility;
            _continueCameraPath = data.continueCameraPath;
            _hasCheckpoint = true;
        }
        catch (System.ArgumentException)
        {
            Debug.LogWarning("CheckpointHistory: 저장된 위치 정보를 읽을 수 없습니다.");
        }
    }

    public static void Record(Player player, Transform checkpoint)
    {
        if (player == null || checkpoint == null || player.gameObject.scene != checkpoint.gameObject.scene)
            return;

        _scenePath = checkpoint.gameObject.scene.path;
        _checkpointPath = GetPath(checkpoint);
        _checkpointPosition = checkpoint.position;
        _hasCheckpoint = true;
        _hasContinuePosition = false;
        Save();
    }

    // 씬을 지우기 전에 실제 플레이어가 가진 복귀 위치를 보존한다.
    public static void RecordCurrent(Player player)
    {
        if (player == null) return;
        _scenePath = player.gameObject.scene.path;
        _checkpointPosition = player.RespawnPosition;
        _checkpointPath = player.RespawnAnchor != null ? GetPath(player.RespawnAnchor.transform) : null;
        _hasCheckpoint = true;
        _hasContinuePosition = false;
        Save();
    }

    public static void RecordContinue(Player player)
    {
        if (player == null) return;
        RecordCurrent(player);
        _continuePosition = player.transform.position;
        _hasContinuePosition = true;
        var visibility = new System.Collections.Generic.List<VisibilityData>();
        foreach (MapVisibility map in Object.FindObjectsByType<MapVisibility>(FindObjectsInactive.Include,
                     FindObjectsSortMode.None))
        {
            if (map.gameObject.scene != player.gameObject.scene)
            {
                continue;
            }

            visibility.Add(new VisibilityData
            { path = GetPath(map.transform), inside = map.IsInside, outside = map.IsOutside });
        }

        _continueVisibility = visibility.ToArray();
        CinemachineCamera camera = CameraManager.Instance != null ? CameraManager.Instance.CurrentCamera : null;
        _continueCameraPath = camera != null && camera.gameObject.scene == player.gameObject.scene
            ? GetPath(camera.transform)
            : null;
        Save();
    }

    public static bool Restore(Scene scene, bool restoreContinuePosition = false)
    {
        Load();
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
            player.MoveToCheckpoint(restoreContinuePosition && _hasContinuePosition
                ? _continuePosition
                : _checkpointPosition);
            if (restoreContinuePosition && _hasContinuePosition) RestoreContinueView(player);
            return true;
        }

        RespawnPoint point = checkpoint.GetComponent<RespawnPoint>();
        if (point != null) player.SetRespawnPoint(point);
        else player.SetRespawnPoint(checkpoint.position);
        // The reloaded scene already reset combat. Only restore position and view.
        player.MoveToCheckpoint(restoreContinuePosition && _hasContinuePosition
            ? _continuePosition
            : checkpoint.position);
        if (point != null) point.RestoreView(player.transform);
        if (restoreContinuePosition && _hasContinuePosition) RestoreContinueView(player);
        return true;
    }

    private static void RestoreContinueView(Player player)
    {
        foreach (MapVisibility map in Object.FindObjectsByType<MapVisibility>(FindObjectsInactive.Include,
                     FindObjectsSortMode.None))
        {
            if (map.gameObject.scene != player.gameObject.scene) continue;
            VisibilityData saved = null;
            if (_continueVisibility != null)
            {
                string path = GetPath(map.transform);
                foreach (VisibilityData entry in _continueVisibility)
                {
                    if (entry != null && entry.path == path)
                    {
                        saved = entry;
                        break;
                    }
                }
            }

            if (saved != null)
            {
                map.SetState(new MapVisibility.PortalState { Inside = saved.inside, Outside = saved.outside });
            }
            else
            {
                GateSwitch.RestoreVisibilityForPosition(map, player.transform.position);
            }
        }

        if (string.IsNullOrEmpty(_continueCameraPath)) return;
        foreach (CinemachineCamera camera in Object.FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include,
                     FindObjectsSortMode.None))
        {
            if (camera.gameObject.scene != player.gameObject.scene ||
                GetPath(camera.transform) != _continueCameraPath)
            {
                continue;
            }
            camera.Follow = player.transform;
            camera.PreviousStateIsValid = false;
            CameraManager.SwithCamera(camera);
            break;
        }
    }

    internal static string GetPath(Transform target)
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