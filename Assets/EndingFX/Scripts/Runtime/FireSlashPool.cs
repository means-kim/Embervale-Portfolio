using System;
using UnityEngine;
using UnityEngine.Pool;

namespace EndingFX
{
    /// <summary>
    /// 불 슬래시 오브젝트 풀. 엔딩 연타 구간은 1초에 3~4번 슬래시가 생성되므로
    /// Instantiate/Destroy 대신 재사용한다. (Unity 2021.1+ 의 UnityEngine.Pool 사용)
    /// </summary>
    public sealed class FireSlashPool : MonoBehaviour
    {
        [SerializeField] private FireSlashEffect prefab;
        [SerializeField, Min(1)] private int prewarmCount = 4;

        private ObjectPool<FireSlashEffect> pool;

        /// <summary>
        /// 풀에서 나간 어떤 슬래시든 피크(타격 순간)에 도달하면 호출.
        /// 재사용 오브젝트마다 구독/해제할 필요 없이 풀 한 곳만 구독하면 된다.
        /// </summary>
        public event Action<FireSlashEffect> SlashPeaked;

        private void Awake()
        {
            pool = new ObjectPool<FireSlashEffect>(
                createFunc: Create,
                actionOnGet: null,                 // 활성화는 FireSlashEffect.Play 가 담당
                actionOnRelease: null,             // 비활성화는 재생 종료 시 스스로 처리
                actionOnDestroy: effect => Destroy(effect.gameObject),
                collectionCheck: false,
                defaultCapacity: prewarmCount);

            Prewarm();
        }

        /// <summary>
        /// 슬래시 하나를 꺼내 재생한다.
        /// </summary>
        /// <returns>재생 중인 이펙트.</returns>
        public FireSlashEffect Spawn(Vector3 position, float angleDegrees, bool flipX, float scale = 1f)
        {
            FireSlashEffect effect = pool.Get();
            effect.Play(position, angleDegrees, flipX, scale);
            return effect;
        }

        private FireSlashEffect Create()
        {
            FireSlashEffect effect = Instantiate(prefab, transform);
            effect.gameObject.SetActive(false);
            effect.Finished += pool.Release;
            effect.Peaked += RaiseSlashPeaked;
            return effect;
        }

        private void RaiseSlashPeaked(FireSlashEffect effect) => SlashPeaked?.Invoke(effect);

        private void Prewarm()
        {
            var temp = new FireSlashEffect[prewarmCount];
            for (int i = 0; i < prewarmCount; i++)
            {
                temp[i] = pool.Get();
            }

            for (int i = 0; i < prewarmCount; i++)
            {
                pool.Release(temp[i]);
            }
        }
    }
}
