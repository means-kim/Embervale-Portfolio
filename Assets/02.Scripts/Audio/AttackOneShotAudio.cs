using System.Collections.Generic;
using UnityEngine;

// Scene-owned voices outlive pooled attack objects. Selected SFX restart on each request.
public sealed class AttackOneShotAudio : MonoBehaviour
{
    private const int MaxVoices = 16;
    private static AttackOneShotAudio _instance;
    private readonly List<AudioSource> _voices = new List<AudioSource>();
    private readonly Dictionary<AudioSource, Object> _voiceOwners = new Dictionary<AudioSource, Object>();

    // Runtime-created service: explicit asset names keep the policy here without
    // changing SFXData or requiring scene references. Update this list on asset rename.
    private static readonly HashSet<string> RestartingSounds = new HashSet<string>
    {
        "SmallLightning_SFX",
        "Telegraph_HV_DoubleMotor_SFX",
        "Telegraph_HV_HeavyStoneSwell_SFX",
        "Telegraph_Circle12_UndergroundPressure_SFX",
        "Telegraph_Circle34_ReferenceBalance_SFX",
        "Telegraph_VerticalSquare_SmoothWind_SFX"
    };

    // Each selected SO has one dedicated channel, separate from ordinary voices.
    private readonly Dictionary<SFXData, AudioSource> _restartingVoices = new Dictionary<SFXData, AudioSource>();

    public static void Play(SFXData sound, float pitchMultiplier = 1f, Object owner = null)
    {
        if (!Application.isPlaying || AudioListener.pause || sound == null || sound.Clip == null) return;
        if (_instance == null)
            _instance = new GameObject("Attack One Shot Audio").AddComponent<AttackOneShotAudio>();
        _instance.PlaySound(sound, pitchMultiplier, owner);
    }

    public static void StopOwned(Object owner)
    {
        if (_instance == null || owner == null) return;
        foreach (AudioSource voice in _instance._voices)
        {
            if (voice != null && _instance._voiceOwners.TryGetValue(voice, out Object currentOwner) && currentOwner == owner)
            {
                voice.Stop();
                _instance._voiceOwners.Remove(voice);
            }
        }
    }

    private void PlaySound(SFXData sound, float pitchMultiplier, Object owner)
    {
        if (owner == null && RestartingSounds.Contains(sound.name))
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

        // Reused channels must never retain ownership from an earlier sound.
        _voiceOwners.Remove(voice);
        if (owner != null) _voiceOwners[voice] = owner;
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
