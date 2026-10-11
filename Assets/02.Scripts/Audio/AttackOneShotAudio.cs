using System.Collections.Generic;
using UnityEngine;

// Scene-owned voices outlive pooled attack objects, so their tails are not cut off.
public sealed class AttackOneShotAudio : MonoBehaviour
{
    private const int MaxVoices = 16;
    private static AttackOneShotAudio _instance;
    private readonly List<AudioSource> _voices = new List<AudioSource>();

    public static void Play(SFXData sound, float pitchMultiplier = 1f)
    {
        if (!Application.isPlaying || AudioListener.pause || sound == null || sound.Clip == null) return;
        if (_instance == null)
            _instance = new GameObject("Attack One Shot Audio").AddComponent<AttackOneShotAudio>();
        _instance.PlaySound(sound, pitchMultiplier);
    }

    private void PlaySound(SFXData sound, float pitchMultiplier)
    {
        AudioSource voice = null;
        foreach (AudioSource candidate in _voices)
        {
            if (candidate != null && !candidate.isPlaying)
            {
                voice = candidate;
                break;
            }
        }

        if (voice == null)
        {
            // Avoid unbounded stacking; never cut off an existing sound to reuse it.
            if (_voices.Count >= MaxVoices) return;
            var child = new GameObject("Voice " + _voices.Count);
            child.transform.SetParent(transform);
            voice = child.AddComponent<AudioSource>();
            voice.playOnAwake = false;
            voice.spatialBlend = 0f;
            voice.loop = false;
            _voices.Add(voice);
        }

        voice.clip = sound.Clip;
        voice.volume = Mathf.Clamp01(sound.Volume);
        voice.pitch = Mathf.Clamp(sound.Pitch * pitchMultiplier, 0.5f, 1.5f);
        voice.Play();
    }

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}
