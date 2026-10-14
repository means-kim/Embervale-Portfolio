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
        public bool Repeat;
    }

    [SerializeField] private Cue[] _cues = Array.Empty<Cue>();
    private bool[] _played;

    public void StopOwnedSounds() => AttackOneShotAudio.StopOwned(this);

    private void OnDisable() => StopOwnedSounds();
    private void OnDestroy() => StopOwnedSounds();

    private void OnEnable()
    {
        _played = new bool[_cues.Length];
    }

    public void PlayAudioCue(int index)
    {
        if (!Application.isPlaying || !isActiveAndEnabled || index < 0 || index >= _cues.Length) return;

        var attack = GetComponentInParent<AirBossAttack>();
        var action = GetComponent<AirBossAction>();
        var stagger = GetComponent<AirBossStagger>();
        AirBossAttackBase scope = null;
        if (attack != null)
        {
            if (!attack.CanPlayAudio) return;
            scope = attack.AudioScope;
        }
        else if (action != null || stagger != null)
        {
            scope = action != null ? action.CurrentAttack : null;
            if (scope == null || !scope.AudioActive)
            {
                scope = null;
                if (stagger == null || !stagger.IsRunning) return;
            }
        }

        Cue cue = _cues[index];
        if (cue == null || cue.Sounds == null) return;
        if (_played == null || _played.Length != _cues.Length) _played = new bool[_cues.Length];
        if (!cue.Repeat && _played[index]) return;
        _played[index] = true;
        foreach (SFXData sound in cue.Sounds)
        {
            if (sound == null) continue;
            AttackOneShotAudio.Play(sound, owner: this, scope: scope);
        }
    }
}