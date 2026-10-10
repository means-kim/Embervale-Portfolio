using System;
using UnityEngine;

// Attach beside the attack Animator. Each event can layer several SFXData assets.
public sealed class AttackAudioCuePlayer : MonoBehaviour
{
    [Serializable]
    public class Cue
    {
        public string Name;
        public SFXData[] Sounds;
    }

    [SerializeField] private Cue[] _cues = Array.Empty<Cue>();
    private bool[] _played;

    private void OnEnable()
    {
        _played = new bool[_cues.Length];
    }

    public void PlayAudioCue(int index)
    {
        if (!Application.isPlaying || !isActiveAndEnabled || index < 0 || index >= _cues.Length || _played[index]) return;

        _played[index] = true;
        Cue cue = _cues[index];
        if (cue == null || cue.Sounds == null) return;
        foreach (SFXData sound in cue.Sounds)
        {
            AttackOneShotAudio.Play(sound);
        }
    }
}