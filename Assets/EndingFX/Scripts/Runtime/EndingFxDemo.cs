using System.Collections;
using UnityEngine;

namespace EndingFX
{
    /// <summary>
    /// 이펙트 확인용 데모. 분석한 엔딩 타이밍(보스 포효 → 약 1초 사이클 × 7회 → 마지막 일격 → 달아오름 → 산산조각)을
    /// 그대로 재생한다. 실제 엔딩 연출(Timeline/카드 컷)은 다음 단계에서 이 API 들을 호출하는 형태로 붙인다.
    /// 플레이 모드에서 자동 시작, 컴포넌트 우클릭 → Replay 로 재실행.
    /// </summary>
    public sealed class EndingFxDemo : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FireSlashPool slashPool;
        [SerializeField] private ImpactBurst hitBurst;
        [SerializeField] private ImpactBurst finalBurst;
        [SerializeField] private BossDissolveEffect bossDissolve;
        [SerializeField] private BossEndingAnimator bossAnimator;
        [SerializeField] private Transform target;

        [Header("Timing (영상 분석 기준)")]
        [SerializeField, Min(1)] private int cycles = 7;
        [SerializeField, Min(1)] private int slashesPerCycle = 3;
        [SerializeField, Min(0.01f)] private float slashInterval = 0.17f;
        [SerializeField, Min(0.1f)] private float cycleInterval = 0.95f;
        [SerializeField, Min(0f)] private float glowSeconds = 4f;

        [Header("Slash")]
        [SerializeField] private float slashScale = 1f;
        [SerializeField] private float finalSlashScale = 1.6f;
        [SerializeField] private float hitScatterRadius = 0.6f;

        // 매 슬래시 각도. 할로우 나이트 카드 컷처럼 대각/역대각/수평이 번갈아 나오게 고정 패턴 사용.
        private static readonly float[] SlashAngles = { 20f, -35f, 5f, 160f, -20f, 40f, 185f, -50f };

        private float currentIntensity;
        private int hitsLanded;
        private Coroutine sequence;

        private void OnEnable()
        {
            slashPool.SlashPeaked += HandleSlashPeaked;
        }

        private void OnDisable()
        {
            slashPool.SlashPeaked -= HandleSlashPeaked;
            if (sequence != null)
            {
                StopCoroutine(sequence);
                sequence = null;
            }
        }

        private void Start()
        {
            Replay();
        }

        /// <summary>데모 시퀀스를 처음부터 다시 재생.</summary>
        [ContextMenu("Replay")]
        public void Replay()
        {
            if (sequence != null)
            {
                StopCoroutine(sequence);
            }

            hitBurst.Clear();
            finalBurst.Clear();
            bossDissolve.ResetEffect();
            bossAnimator.ResetToIdle();
            hitsLanded = 0;
            sequence = StartCoroutine(PlaySequence());
        }

        private IEnumerator PlaySequence()
        {
            var slashWait = new WaitForSeconds(slashInterval);
            float cycleRest = Mathf.Max(0f, cycleInterval - slashInterval * slashesPerCycle);
            var cycleWait = new WaitForSeconds(cycleRest);
            int slashIndex = 0;

            // 보스 포효 시작 → 팔을 펼치는 순간(슬로우 구간 시작)부터 공격 개시.
            // 포효는 느리게 계속 재생되고, 끝나면 그때까지 쌓인 균열 프레임으로 넘어간다.
            bossAnimator.PlayIntro();
            yield return new WaitForSeconds(bossAnimator.IntroAttackStartTime);

            for (int cycle = 0; cycle < cycles; cycle++)
            {
                // 진행도에 따라 강도가 올라감 → 파티클이 점점 많아짐 (누적 피해 표현)
                currentIntensity = cycles == 1 ? 1f : (float)cycle / (cycles - 1);

                for (int i = 0; i < slashesPerCycle; i++)
                {
                    float angle = SlashAngles[slashIndex % SlashAngles.Length];
                    Vector3 pos = target.position + (Vector3)(Random.insideUnitCircle * hitScatterRadius);
                    slashPool.Spawn(pos, angle, slashIndex % 2 == 1, slashScale);
                    slashIndex++;
                    yield return slashWait;
                }

                yield return cycleWait;
            }

            // 마지막 일격: 큰 슬래시 + 전용 폭발 + 보스 달아오름
            currentIntensity = -1f; // 일반 hitBurst 대신 finalBurst 를 쓰도록 표시
            slashPool.Spawn(target.position, -25f, false, finalSlashScale);
            yield return new WaitForSeconds(0.15f);
            bossDissolve.BeginGlow();

            yield return new WaitForSeconds(glowSeconds);
            bossDissolve.Dissolve();
            sequence = null;
        }

        private void HandleSlashPeaked(FireSlashEffect slash)
        {
            // 균열 진행도: 마지막 일격에서 정확히 1이 되도록 전체 타수 기준으로 계산
            int totalHits = cycles * slashesPerCycle + 1;
            hitsLanded++;
            bossAnimator.ApplyHit((float)hitsLanded / totalHits);

            Transform t = slash.transform;
            if (currentIntensity < 0f)
            {
                finalBurst.Play(t.position, t.eulerAngles.z, 1f);
                return;
            }

            hitBurst.Play(t.position, t.eulerAngles.z, currentIntensity);
        }
    }
}
