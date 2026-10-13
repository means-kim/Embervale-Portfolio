using System;
using UnityEngine;
using UnityEngine.Serialization;
namespace EndingFX
{
    /// <summary>
    /// 타격 시 "터져 나오는" 파티클 묶음.
    /// 여러 레이어(플래시/스파크/불티/링/재)를 강도(0~1)에 따라 개수를 보간해서 한 번에 Emit 한다.
    ///
    /// 설계 의도:
    /// - 할로우 나이트 엔딩처럼 타격이 거듭될수록 파티클이 늘어나야 하므로 Burst 를 모듈에 고정하지 않고
    ///   코드에서 ParticleSystem.Emit(count) 로 직접 뿌린다. (모양/속도/색은 각 파티클 시스템 설정 그대로 사용)
    /// - 링 레이어는 수명이 길어서 연타 시 화면에 자연스럽게 누적된다.
    /// - 파티클은 모두 World 시뮬레이션이라 이 오브젝트를 옮겨도 이미 나온 입자는 제자리에 남는다.
    ///   그래서 프리팹 하나를 계속 위치만 옮겨서 재사용하면 된다. (풀 불필요)
    /// </summary>
    public sealed class ImpactBurst : MonoBehaviour
    {
        [Serializable]
        public struct Layer
        {
            public ParticleSystem system;

            [Header("강도 0 일 때 개수")]
            [FormerlySerializedAs("minCount")]
            [Min(0)] public int MinCount;

            [Header("강도 1 일 때 개수")]
            [FormerlySerializedAs("maxCount")]
            [Min(0)] public int MaxCount;

            public Layer(ParticleSystem system, int MinCount, int MaxCount)
            {
                this.system = system;
                this.MinCount = MinCount;
                this.MaxCount = MaxCount;
            }
        }

        [FormerlySerializedAs("layers")]
        [SerializeField] private Layer[] _layers = new Layer[0];

        /// <summary>
        /// 현재 위치에서 터뜨린다.
        /// </summary>
        /// <param name="intensity">0~1. 연타 진행도(누적 피해)를 넣으면 점점 화려해진다.</param>
        public void Play(float intensity)
        {
            intensity = Mathf.Clamp01(intensity);
            for (int i = 0; i < _layers.Length; i++)
            {
                Layer layer = _layers[i];
                if (layer.system == null)
                {
                    continue;
                }

                int count = Mathf.RoundToInt(Mathf.Lerp(layer.MinCount, layer.MaxCount, intensity));
                if (count > 0)
                {
                    layer.system.Emit(count);
                }
            }
        }

        /// <summary>
        /// 위치/방향을 지정해서 터뜨린다.
        /// </summary>
        /// <param name="position">타격 지점.</param>
        /// <param name="angleDegrees">슬래시 각도. 방향성 있는 레이어(Shape 회전)에 반영된다.</param>
        /// <param name="intensity">0~1 강도.</param>
        public void Play(Vector3 position, float angleDegrees, float intensity)
        {
            transform.SetPositionAndRotation(position, Quaternion.Euler(0f, 0f, angleDegrees));
            Play(intensity);
        }

        /// <summary>모든 레이어의 입자를 즉시 제거. (컷 전환/리셋용)</summary>
        public void Clear()
        {
            for (int i = 0; i < _layers.Length; i++)
            {
                if (_layers[i].system != null)
                {
                    _layers[i].system.Clear(false);
                }
            }
        }
    }
}
