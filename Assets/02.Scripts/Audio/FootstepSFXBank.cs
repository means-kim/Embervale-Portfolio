using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "FootstepBank", menuName = "Audio/Footstep Bank")]
public class FootstepSFXBank : ScriptableObject
{
    [Header("Clips (테스트 30개 권장)")]
    public List<AudioClip> Clips = new();
    [Header("폴더 자동 채우기 설정")]
    [Tooltip("Project 기준 폴더 경로. 예: Assets/Footsteps Pack Expanded/SingleSteps/IceAndSnowSteps")]
    public string SourceFolder = "Assets/Footsteps Pack Expanded/SingleSteps/IceAndSnowSteps";
    [Tooltip("자동 채우기 시 담을 개수. 242개 중 균등 간격으로 뽑는다.")]
    [Min(1)] public int AutoFillCount = 30;
    [Header("재생 설정")]
    [Range(0f, 1f)] public float Volume = 0.9f;
    [Range(0.5f, 1.5f)] public float MinPitch = 0.95f;
    [Range(0.5f, 1.5f)] public float MaxPitch = 1.05f;
    public bool AvoidRepeat = true;
    [System.NonSerialized] private int _lastIndex = -1;
    public bool HasClips
    {
        get
        {
            if (Clips == null) return false;
            for (int i = 0; i < Clips.Count; i++)
            {
                if (Clips[i] != null) return true;
            }
            return false;
        }
    }
    public AudioClip PickRandom()
    {
        if (Clips == null || Clips.Count == 0) return null;
        int validCount = 0;
        for (int i = 0; i < Clips.Count; i++)
        {
            if (Clips[i] != null) validCount++;
        }
        if (validCount == 0) return null;
        if (validCount == 1)
        {
            for (int i = 0; i < Clips.Count; i++)
            {
                if (Clips[i] != null) return Clips[i];
            }
            return null;
        }
        int pick = Random.Range(0, validCount);
        int index = -1;
        for (int i = 0; i < Clips.Count; i++)
        {
            if (Clips[i] == null) continue;
            if (pick == 0) { index = i; break; }
            pick--;
        }
        if (index < 0) return null;
        if (AvoidRepeat && index == _lastIndex)
        {
            for (int offset = 1; offset <= Clips.Count; offset++)
            {
                int next = (index + offset) % Clips.Count;
                if (Clips[next] != null) { index = next; break; }
            }
        }
        _lastIndex = index;
        return Clips[index];
    }
    public float RollPitch()
    {
        float lo = Mathf.Min(MinPitch, MaxPitch);
        float hi = Mathf.Max(MinPitch, MaxPitch);
        return Random.Range(lo, hi);
    }
    public void Play()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayFootstep(this);
    }
    public void PlayOn(AudioSource source)
    {
        if (source == null || !HasClips) return;
        AudioClip clip = PickRandom();
        if (clip == null) return;
        source.pitch = RollPitch();
        source.PlayOneShot(clip, Volume);
    }
#if UNITY_EDITOR
    [ContextMenu("Fill Clips From Folder (균등 간격)")]
    private void FillFromFolder()
    {
        var guids = UnityEditor.AssetDatabase.FindAssets("t:AudioClip", new[] { SourceFolder });
        if (guids == null || guids.Length == 0)
        {
            Debug.LogWarning($"[FootstepSFXBank] 오디오를 못 찾음: {SourceFolder}");
            return;
        }
        var paths = new List<string>(guids.Length);
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(path)) paths.Add(path);
        }
        paths.Sort();
        int count = Mathf.Clamp(AutoFillCount, 1, paths.Count);
        var picked = new List<AudioClip>(count);
        for (int i = 0; i < count; i++)
        {
            int src = Mathf.FloorToInt((float)i * paths.Count / count);
            src = Mathf.Clamp(src, 0, paths.Count - 1);
            var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(paths[src]);
            if (clip != null) picked.Add(clip);
        }
        Clips = picked;
        _lastIndex = -1;
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[FootstepSFXBank] {paths.Count}개 중 {picked.Count}개 등록: {SourceFolder}");
    }
    [ContextMenu("Fill All Clips From Folder (전체)")]
    private void FillAllFromFolder()
    {
        var guids = UnityEditor.AssetDatabase.FindAssets("t:AudioClip", new[] { SourceFolder });
        var picked = new List<AudioClip>(guids.Length);
        var paths = new List<string>(guids.Length);
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(path)) paths.Add(path);
        }
        paths.Sort();
        foreach (string path in paths)
        {
            var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip != null) picked.Add(clip);
        }
        Clips = picked;
        _lastIndex = -1;
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[FootstepSFXBank] 전체 {picked.Count}개 등록: {SourceFolder}");
    }
#endif
}
