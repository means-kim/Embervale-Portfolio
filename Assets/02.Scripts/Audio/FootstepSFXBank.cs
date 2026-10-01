using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 눈 발자국 같은 대량 랜덤 SFX용 뱅크. 기존 SFXData(단일 클립)는 그대로 두고,
/// 여러 클립 중 랜덤 1개를 골라 재생할 때만 사용한다.
/// Inspector에서 직접 만들 수 있고, 우클릭 ContextMenu로 폴더 자동 채우기가 된다.
/// </summary>
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

    // 직전 재생 인덱스. SO 에셋에 저장되지 않는 런타임 상태다.
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

    /// <summary>유효 클립 중 랜덤 1개를 고른다. AvoidRepeat이면 직전과 같은 인덱스를 피한다.</summary>
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
            // 다음 유효 클립으로 한 칸 이동해서 연속 중복만 회피한다.
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

    /// <summary>AudioManager를 통해 랜덤 1개를 재생한다. (Ground 등 공용 경로용, 레거시)</summary>
    public void Play()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayFootstep(this);
    }

    /// <summary>
    /// AudioManager를 거치지 않고 지정한 전용 AudioSource로 직접 재생한다.
    /// Platform 발소리처럼 별도 관리가 필요할 때 사용한다.
    /// </summary>
    public void PlayOn(AudioSource source)
    {
        if (source == null || !HasClips) return;
        AudioClip clip = PickRandom();
        if (clip == null) return;
        source.pitch = RollPitch();
        source.PlayOneShot(clip, Volume);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Inspector에서 이 에셋 우클릭 → Fill Clips From Folder.
    /// SourceFolder에서 오디오를 이름순 정렬 후 AutoFillCount만큼 균등 간격으로 담는다.
    /// 242개 드래그 대신 이 메뉴를 쓰면 된다.
    /// </summary>
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
            // 균등 간격: 242개 중 30개면 약 8칸마다 1개씩 뽑아 다양성 확보.
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

    /// <summary>전부 담고 싶을 때용. 242개 테스트는 메모리 확인 후 사용.</summary>
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
