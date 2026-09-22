using UnityEngine;

namespace GA1.PlayerArt
{
    /// <summary>Preview-only component. Does not move the player or control gameplay.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class PlayerMotionPreviewV4 : MonoBehaviour
    {
        public enum Motion
        {
            Run, JumpStart, JumpRise, JumpApex, Fall, Land, DoubleJump,
            DashStart, DashLoop, DashEnd, Attack, WallCling, WallClimb, WallSlide, WallJump, Hurt
        }

        public Motion motion = Motion.Run;
        public bool autoAdvance = true;
        [Min(0.5f)] public float secondsPerMotion = 1.2f;
        private Animator cachedAnimator;
        private Motion current;
        private float elapsed;

        private void OnEnable()
        {
            cachedAnimator = GetComponent<Animator>();
            if (Application.isPlaying) PlaySelected();
        }

        private void Update()
        {
            if (cachedAnimator == null || cachedAnimator.runtimeAnimatorController == null) return;
            if (motion != current) PlaySelected();
            elapsed += Time.deltaTime;
            if (autoAdvance && elapsed >= Mathf.Max(0.5f, secondsPerMotion))
            {
                motion = (Motion)(((int)motion + 1) % 16);
                PlaySelected();
            }
        }

        [ContextMenu("Replay Selected Motion")]
        public void PlaySelected()
        {
            if (!Application.isPlaying) return;
            if (cachedAnimator == null) cachedAnimator = GetComponent<Animator>();
            current = motion;
            elapsed = 0f;
            if (cachedAnimator != null && cachedAnimator.runtimeAnimatorController != null)
                cachedAnimator.Play("Base Layer." + motion, 0, 0f);
        }
    }
}
