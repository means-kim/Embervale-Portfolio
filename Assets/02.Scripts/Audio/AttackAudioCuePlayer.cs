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

    private void OnEnable()
    {
        _played = new bool[_cues.Length];
    }

    public void PlayAudioCue(int index)
    {
        if (!Application.isPlaying || !isActiveAndEnabled || index < 0 || index >= _cues.Length) return;

        Cue cue = _cues[index];
        if (cue == null || cue.Sounds == null) return;
        if (_played == null || _played.Length != _cues.Length) _played = new bool[_cues.Length];
        if (!cue.Repeat && _played[index]) return;
        _played[index] = true;
        foreach (SFXData sound in cue.Sounds)
        {
            if (sound == null) continue;
            bool staggerSound = sound.name == "Stagger_Crackle_SFX" || sound.name == "Stagger_Recovery_SFX";
            if (staggerSound)
            {
                // Ignore outgoing animation events after cancellation/cross-fade.
                var stagger = GetComponent<AirBossStagger>();
                if (stagger == null || !stagger.IsRunning) continue;
            }
            var attack = GetComponentInParent<AirBossAttack>();
            bool pillarSound = attack != null && attack.Type == AirBossAttackType.LargeIcePillar &&
                (sound.name == "LargeIcePillar_Create_SFX" || sound.name == "LargeIcePillar_Broken_SFX");
            bool owned = staggerSound || pillarSound;
            AttackOneShotAudio.Play(sound, owner: owned ? this : null);
        }
    }
}
