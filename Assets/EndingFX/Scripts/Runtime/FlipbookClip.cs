using UnityEngine;

namespace EndingFX
{
    /// <summary>
    /// 스프라이트 시퀀스(플립북) 데이터. 재생 로직 없이 데이터만 가진다.
    /// 같은 클립을 여러 프리팹/연출에서 공유하기 위해 ScriptableObject 로 분리했다. (SSoT)
    /// </summary>
    [CreateAssetMenu(fileName = "FlipbookClip", menuName = "EndingFX/Flipbook Clip")]
    public sealed class FlipbookClip : ScriptableObject
    {
        [SerializeField] private Sprite[] frames = new Sprite[0];

        [Tooltip("초당 프레임 수. 7프레임 기준 24fps ≈ 0.29초")]
        [SerializeField, Min(1f)] private float framesPerSecond = 24f;

        [Tooltip("슬래시가 가장 크게 펼쳐진 프레임(0부터). 이 프레임에서 타격 판정/파티클이 터진다")]
        [SerializeField, Min(0)] private int peakFrameIndex = 2;

        [Header("Slow Section (선택)")]
        [Tooltip("느리게 재생할 구간의 시작 프레임. -1 이면 사용 안 함")]
        [SerializeField] private int slowStartFrame = -1;

        [Tooltip("느리게 재생할 구간의 끝 프레임(포함)")]
        [SerializeField] private int slowEndFrame = -1;

        [Tooltip("슬로우 구간 재생 속도. 0.25 = 4배 느리게")]
        [SerializeField, Range(0.05f, 1f)] private float slowSpeed = 1f;

        /// <summary>전체 프레임 수.</summary>
        public int FrameCount => frames.Length;

        /// <summary>기본 프레임 1장의 길이(초). 슬로우 구간은 <see cref="GetFrameDuration"/> 사용.</summary>
        public float FrameDuration => 1f / framesPerSecond;

        /// <summary>임팩트가 발생하는 피크 프레임 인덱스(범위 보정됨).</summary>
        public int PeakFrameIndex => Mathf.Clamp(peakFrameIndex, 0, Mathf.Max(0, frames.Length - 1));

        /// <summary>슬로우 구간이 설정되어 있는지.</summary>
        public bool HasSlowSection => slowStartFrame >= 0 && slowEndFrame >= slowStartFrame && slowStartFrame < frames.Length;

        /// <summary>슬로우 구간 시작 시각(초). 구간이 없으면 0.</summary>
        public float SlowSectionStartTime => HasSlowSection ? GetStartTime(slowStartFrame) : 0f;

        /// <summary>슬로우 구간을 반영한 전체 재생 길이(초).</summary>
        public float TotalDuration => GetStartTime(frames.Length);

        /// <summary>인덱스에 해당하는 스프라이트.</summary>
        public Sprite GetFrame(int index) => frames[index];

        /// <summary>해당 프레임의 표시 시간(초). 슬로우 구간이면 늘어난 값.</summary>
        public float GetFrameDuration(int index)
        {
            bool slow = HasSlowSection && index >= slowStartFrame && index <= slowEndFrame;
            return slow ? FrameDuration / slowSpeed : FrameDuration;
        }

        /// <summary>해당 프레임이 시작되는 시각(초).</summary>
        public float GetStartTime(int index)
        {
            float time = 0f;
            for (int i = 0; i < index; i++)
            {
                time += GetFrameDuration(i);
            }

            return time;
        }

#if UNITY_EDITOR
        /// <summary>에디터 빌더에서 프레임을 채울 때 사용.</summary>
        public void EditorSetFrames(Sprite[] newFrames, float fps, int peakIndex)
        {
            frames = newFrames;
            framesPerSecond = fps;
            peakFrameIndex = peakIndex;
        }

        /// <summary>에디터 빌더에서 슬로우 구간을 설정할 때 사용. start = -1 이면 해제.</summary>
        public void EditorSetSlowSection(int start, int end, float speed)
        {
            slowStartFrame = start;
            slowEndFrame = end;
            slowSpeed = speed;
        }
#endif
    }
}
