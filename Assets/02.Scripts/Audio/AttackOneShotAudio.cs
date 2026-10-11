using System.Collections.Generic;
using UnityEngine;

// Scene-owned voices outlive pooled attack objects. Selected SFX restart on each request.
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
        if (RestartingSounds.Contains(sound.name))
        {
            PlayRestarting(sound);
            return;
        }

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
            if (_voices.Count >= MaxVoices) return;
            voice = CreateVoice("Voice " + _voices.Count);
            _voices.Add(voice);
        }

        voice.clip = sound.Clip;
        voice.volume = Mathf.Clamp01(sound.Volume);
        voice.pitch = Mathf.Clamp(sound.Pitch * pitchMultiplier, 0.5f, 1.5f);
        voice.Play();
    }

    private AudioSource CreateVoice(string voiceName)
    {
        var child = new GameObject(voiceName);
        child.transform.SetParent(transform);
        var source = child.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.loop = false;
        return source;
    }

    private void PlayRestarting(SFXData sound)
    {
        if (!_restartingVoices.TryGetValue(sound, out AudioSource voice) || voice == null)
        {
            voice = CreateVoice("Restarting Voice " + sound.name);
            _restartingVoices[sound] = voice;
        }

        // Discard the previous tail and restart at the asset's original volume.
        voice.Stop();
        voice.clip = sound.Clip;
        voice.volume = Mathf.Clamp01(sound.Volume);
        voice.pitch = Mathf.Clamp(sound.Pitch, 0.5f, 1.5f);
        voice.Play();
    }

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}
