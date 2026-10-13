using UnityEngine;
using UnityEngine.Serialization;
namespace EndingFX
{
    /// <summary>
    /// 스프라이트 시퀀스(플립북) 데이터. 재생 로직 없이 데이터만 가진다.
    /// 같은 클립을 여러 프리팹/연출에서 공유하기 위해 ScriptableObject 로 분리했다. (SSoT)
    /// </summary>
    [CreateAssetMenu(fileName = "FlipbookClip", menuName = "EndingFX/Flipbook Clip")]
    public sealed class FlipbookClip : ScriptableObject
    {
        [FormerlySerializedAs("frames")]
        [SerializeField] private Sprite[] _frames = new Sprite[0];

        [Tooltip("초당 프레임 수. 7프레임 기준 24fps ≈ 0.29초")]
        [FormerlySerializedAs("framesPerSecond")]
        [SerializeField, Min(1f)] private float _framesPerSecond = 24f;

        [Tooltip("슬래시가 가장 크게 펼쳐진 프레임(0부터). 이 프레임에서 타격 판정/파티클이 터진다")]
        [FormerlySerializedAs("peakFrameIndex")]
        [SerializeField, Min(0)] private int _peakFrameIndex = 2;

        /// <summary>전체 프레임 수.</summary>
        public int FrameCount => _frames.Length;

        /// <summary>프레임 1장의 길이(초).</summary>
        public float FrameDuration => 1f / _framesPerSecond;

        /// <summary>임팩트가 발생하는 피크 프레임 인덱스(범위 보정됨).</summary>
        public int PeakFrameIndex => Mathf.Clamp(_peakFrameIndex, 0, Mathf.Max(0, _frames.Length - 1));

        /// <summary>인덱스에 해당하는 스프라이트.</summary>
        public Sprite GetFrame(int index) => _frames[index];
    }
}