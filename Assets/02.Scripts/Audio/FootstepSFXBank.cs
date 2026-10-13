using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "FootstepBank", menuName = "Audio/Footstep Bank")]
public class FootstepSFXBank : ScriptableObject
{
    [Header("Clips (직접 참조)")]
    public List<AudioClip> Clips = new();
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
}
