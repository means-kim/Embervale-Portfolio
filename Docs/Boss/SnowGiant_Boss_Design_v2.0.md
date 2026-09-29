# Emberveil — 설산 거인 보스 기획서 v2.0

> **과거 버전:** 현재 제작 기준은 [v3.0 — Idle·Walk 정렬 기준 반영](SnowGiant_Boss_Design_v3.0.md)이다. 아래 v2.0 및 후속 8프레임 제작 이력은 보존 기록이다.

- 개정일: 2026-09-29
- 출처: 사용자가 해당 날짜 대화에 제공한 「Emberveil — 설산 거인 보스 기획서 v2.0」 전체 23절.
- 상태: 최신 사용자 기획 기준. 아래는 원문 요구사항을 프로젝트 참조용으로 정리한 기록이며 구현 완료 보고가 아니다.
- 핵심 변경: 전투 시작 Roar, Phase 2 전환 Roar, BossVisual 크기 증가, 붉은 안광 멀티스프라이트 오버레이.
- 기획 확인·기억 요청에 따라 문서만 저장했다. 씬·코드·프리팹·애니메이션·스프라이트 변경을 승인한 것으로 해석하지 않는다.

> 후속 명시 변경 (2026-09-29): 사용자가 얼음가시를 Slam과 구별되는 전용 Body 동작으로 제작하도록 요청했다. 얼음가시의 Slam 재사용 원칙에 한해 이 요청을 우선한다. Idle 시작·복귀, 2048×1024 / 512×512 × 8셀, Idle·Walk 피봇, 팔·기존 가시 프리팹의 콜라이더를 적용했다. [제작 결과와 검증 범위](IceThorn_v01/Implementation.md). Phase 2 전체 Body 세트 재제작이나 다른 패턴 변경을 뜻하지 않는다.

## 1. 보스 개요와 전투 목표

설산 / 얼음 지대의 설산 거인(가칭). 플레이어와 1대1 전투, 근거리 중심에 얼음 범위 공격을 결합하는 2 Phase 보스다. 핵심 키워드는 거대함, 묵직함, 얼음, 회피, 반격, 광폭화.

느리지만 강력한 공격을 대시로 회피하고 공격 후 빈틈에 반격한다. 기본 리듬은 **관찰 → 회피 → 반격 → 거리 조절 → 다음 패턴 대비**. 높은 공격력, 명확한 전조, 긴 후딜을 유지한다. 완전 무작위보다 패턴 학습을 중시하고, 강한 공격일수록 반격 기회를 제공한다.

Phase 1은 묵직한 육체 공격과 단발 공격 중심이다. Phase 2는 기존 공격에 얼음 능력과 연속 공격을 결합해 템포·위압감을 높인다. 전조를 지나치게 줄이거나 회피 불가능한 공격을 만들지 않는다. 새 애니메이션 대량 제작보다 기존 애니메이션과 VFX 조합을 우선한다.

## 2. 전체 상태와 Encounter

`Dormant → Encounter → Phase1 → PhaseTransition → Phase2 → Death`

| 상태 | 역할 |
| --- | --- |
| Dormant | 보스 방 진입 전 대기 |
| Encounter | 최초 진입 감지 및 전투 시작 Roar |
| Phase1 | 기본 전투 |
| PhaseTransition | HP 50% 이하에서 전환 연출 |
| Phase2 | 강화 전투 |
| Death | 공격 중지 및 사망 연출 |

플레이어 최초 보스 방 진입 → Boss Encounter Trigger → 일반 공격 AI 비활성화 → Roar 애니메이션 + Roar SFX + Camera Shake → Roar 종료 → 플레이어 위치·거리 탐색 → Phase 1 AI 활성화.

- 진입 Roar는 1회 실행한다. 즉시 공격하지 않는다.
- 진입 Roar 중 일반 공격 금지, 기본 크기와 기존 푸른 눈 유지, 붉은 안광 OFF.
- Encounter, PhaseTransition, Death에서는 일반 공격 선택 로직을 실행하지 않는다(`CanAttack = false`). 전환이 끝난 뒤에만 공격을 재개한다.
- Encounter Roar와 Phase Transition Roar는 같은 애니메이션을 재사용하되 상태와 연출을 구분한다.

## 3. Phase 1과 Phase 2

Phase 1의 표기 범위는 HP 100%~50%, Phase 2는 50%~0%다. 경계의 실제 전환 조건은 **HP가 최초로 50% 이하가 됐을 때 한 번**이다. HP 0에서는 Death가 정의되어 있으며 전환 도중 사망 등 세부 우선순위는 별도 결정이 필요하다.

Phase 1: 기본 크기, 기본 푸른 눈, EyeGlow Renderer OFF, 비교적 긴 전조·후딜, 단발 공격 중심. 한손 내려찍기 / 팔 휘두르기 / 돌진 / 얼음 가시 / 양손 지면 강타를 사용한다.

Phase 2 전환 순서:

1. HP 50% 이하 및 전환 이력 확인.
2. 현재 공격 종료 또는 안전한 행동 중단.
3. 일반 공격 AI 일시 정지.
4. Roar 실행.
5. BossVisual Scale 증가.
6. 붉은 EyeGlow Overlay 활성화.
7. Ice Aura / Phase VFX / Camera Shake.
8. Phase 2 공격 데이터·선택 로직 적용 후 AI 재개.

전환은 한 번만 실행하며 전환 중 추가 피격으로 중복 실행하지 않는다. Phase 2 이후 Phase 1으로 돌아가지 않는다. 기본 공격을 제거하지 않고 효과·연속 공격으로 강화한다.

크기 증가량은 **미정, 플레이 테스트로 결정**한다. 초기 구현 시 조절 가능한 변수로 둔다. 전체 Root보다 BossVisual만 확대하는 것을 우선하며 HitBox/HurtBox가 의도치 않게 확대되지 않도록 분리한다. 실제 공격 판정 확대가 필요하면 HitBox는 별도 조절한다.

```text
Boss
├─ BossVisual  ← 시각적 Scale 증가
│  ├─ Body (SpriteRenderer)
│  └─ EyeGlow (SpriteRenderer)
├─ HitBox
├─ HurtBox
└─ BossController
```

보스 로직과 시각 연출은 가능한 한 분리한다. Phase 변경 이벤트를 Boss Combat(공격 강화), Boss Visual(Scale), EyeGlow, Phase VFX가 각각 받는 구조를 지향한다.

## 4. 공격 패턴과 수치

시간은 기획의 대략적인 초기값이며 최종 밸런스나 현재 구현값으로 단정하지 않는다.

| 패턴 | 기본 흐름 | 전조 | 공격 / 범위 | 후딜 | 회피 / 위험 / 반격 | Phase 2 |
| --- | --- | --- | --- | --- | --- | --- |
| 한손 내려찍기 (Slam) | 팔 들기 → 정지 → 지면 강타 → 충격·파편 → 후딜 | 약 0.6~0.8초 | 약 0.1~0.2초, 근거리 | 약 0.8~1.0초 | 좌우 대시 / 낮음 / 높음 | Slam 이후 Ice Spike |
| 팔 휘두르기 (Swing) | 상체 비틀기 → 팔 뒤로 당기기 → 전방 휘두르기 → 후딜 | 약 0.5~0.7초 | 전방 넓은 근거리 광역 | 약 0.7초 | 거리 확보·보스 뒤로 대시 / 중간 / 중간 | 1회 → 2연속 Swing, 기존 애니메이션 재사용 |
| 돌진 (Charge) | 자세 낮추기 → 발밑 눈먼지 → 정지 → 플레이어 방향 돌진 → 정지·벽 충돌 → 후딜 | 약 0.7초 | 장거리 이동 공격 | 약 1.0~1.5초 | 진행 방향을 피해 대시 / 중간 / 높음 | 이동 경로 Ice Trail |
| 얼음 가시 (Ice Spike) | 지면 공격 → 바닥 얼음 균열 → 대기 → 가시 생성 → 얼음 파괴 | 약 0.7~1.0초 | 원거리·범위, 플레이어 주변 또는 방향의 바닥 지정 | 수치 미정 | 안전 지역 이동 / 중간~높음 / 중간 | 개수·범위 증가, 전조 유지 |
| 양손 지면 강타 (Ground Smash) | 양팔 들기 → 정지 → 강타 → 충격파 → 긴 후딜 | 약 1.0초 | 보스 주변 + 좌우 광역 | 약 1.2초 | 점프·대시·거리 확보 / 높음 / 높음 | Shockwave 범위 또는 개수 강화 |

- Slam: 충돌 순간 Camera Shake, 얼음·눈 파편, Ground Dust, 강한 충돌 SFX.
- Swing: 회피하면서 후방 또는 반격 가능한 위치를 확보하도록 유도한다.
- Charge: 벽이나 특정 오브젝트 충돌 시 짧은 경직·반격 기회를 제공할 수 있다(선택 기믹).
- Ice Spike: 바닥 균열 → 푸른빛 증가 → 가시 생성으로 위치와 회피 시간을 반드시 예고한다. 별도 Body 공격 애니메이션보다 Slam + Ice Spike VFX 조합을 우선한다.
- Ground Smash: 강한 Camera Shake, 순간 Flash, 얼음 파편, 눈먼지, Shockwave, 강한 저음 SFX.

## 5. 공격 선택과 공통 구조

플레이어 거리 → 사용 가능한 패턴 목록 → 직전 패턴 확인 → 패턴 선택 → 실행. 완전 랜덤을 사용하지 않으며 직전 공격은 일정 확률로 제외해 과도한 반복을 막는다.

| 거리 | 후보 |
| --- | --- |
| 근거리 | Slam, Swing, Ground Smash |
| 중거리 | Charge, Ice Spike, Swing |
| 원거리 | Charge, Ice Spike |

모든 공격은 **Telegraph → Attack → Recovery → Cooldown** 구조다. Recovery에는 행동할 수 없고 플레이어가 반격한다. 예시 Slam: 0.7초 → 0.15초 → 0.8초 → 0.5초. 거리 임계값, 선택 가중치, 반복 제외 확률은 아직 지정되지 않았다.

## 6. Body 애니메이션과 EyeGlow

필요 Body 애니메이션: Idle, Walk, Slam, Swing, Charge, Ground Smash, Roar, Hit, Death. Walk는 접근·추적, Hit는 피격, Death는 사망에 사용한다.

**Phase 2 전용 Body 세트를 새로 만들거나 기존 Body Sprite를 붉은 눈 버전으로 수정하지 않는다.** 기존 Body 애니메이션에 BossVisual Scale, Red EyeGlow, Ice Aura, 강화 로직, VFX/SFX를 조합한다.

EyeGlow는 Body보다 위에 렌더링하는 별도 SpriteRenderer다. 각 Body 프레임에 대응하는 투명 배경의 붉은 Eye Core + 반투명 Glow만 제작한다. Body가 8프레임이면 EyeGlow도 동일한 8프레임이다(8은 예시이지 모든 시트의 고정 프레임 수가 아니다).

Body와 EyeGlow의 다음 항목을 일치시킨다:

- 전체 캔버스 크기, 셀 크기.
- 프레임 수·순서.
- Pivot, PPU, Slice 설정, Transform 기준.
- 각 프레임의 실제 눈 위치.

Eye Core는 기존 푸른 눈을 덮을 수 있도록 선명하고 강한 붉은색으로, 주변 Glow는 반투명으로 제작한다.

필요 시트: Idle_EyeGlow, Walk_EyeGlow, Slam_EyeGlow, Swing_EyeGlow, Charge_EyeGlow, GroundSmash_EyeGlow, Roar_EyeGlow, Hit_EyeGlow.

별도 Animator 두 개보다 **하나의 Animator Clip에서 Body.Sprite와 EyeGlow.Sprite를 동일한 프레임 타이밍으로 제어**하는 것을 우선한다.

- Phase 1: Body 재생, EyeGlow Renderer OFF.
- PhaseTransition: Roar의 전환 연출 시점에 EyeGlow ON.
- Phase 2: Body + EyeGlow 동기 재생.
- Death: EyeGlow OFF 또는 Fade Out. 전용 Death EyeGlow 시트는 필수로 확정하지 않는다.

## 7. VFX, SFX, Game Feel과 사망

필요 VFX: Red Eye Glow, Phase Transition Effect, Ice Aura, 눈 파편, 얼음 파편, Ground Dust, Ice Spike, Shockwave, Ice Trail, Hit Effect, Death Effect.

Phase 2 시각 우선순위: **Red Eye Glow → Scale 증가 → Camera Shake → Ice Aura → 추가 파티클·눈보라**. 핵심은 크기 변화와 붉은 안광이며 Aura·환경 연출은 후속 추가한다.

필요 SFX: 발걸음(Walk), 팔 휘두르기(Swing), 지면 충돌(Slam), 돌진(Charge), 얼음 생성(Ice Spike), 얼음 파괴, 포효(Encounter/PhaseTransition), Phase Transition 효과음, 피격, 사망. Roar SFX 재사용 가능, 전환에는 추가 효과음 조합 가능.

- 강한 공격: Camera Shake + Hit Stop + Impact VFX + SFX.
- 보스 피격: White Flash + Hit Effect + 약한 Camera Shake.
- 전투 시작: Roar + Roar SFX + Camera Shake.
- Phase 2: 행동 정지 + Roar + Scale Up + Red EyeGlow ON + Ice Aura + Camera Shake. 진입 Roar보다 강한 연출.
- 사망: HP 0 → 일반 공격 중단 → Death → 안광 OFF 또는 소멸 → 짧은 경직 → 몸의 얼음 균열 증가 → 얼음 결정 파괴 → 붕괴 → 눈·얼음 파편 확산 → 전투 종료.
- 가능하면 마지막 적중 직후 짧은 Hit Stop과 Camera Shake로 최후 일격을 강조한다.

## 8. 구현 우선순위와 완성 기준

아래 항목은 사용자 기획상의 할 일이다. 현 프로젝트 구현 여부는 이번 기록 작업에서 검사하지 않았으며 완료 표시하지 않는다.

1. **전투 MVP:** Boss HP, 플레이어 탐색·방향 전환, 공격 선택, Slam/Swing/Charge, 피격 판정, 사망 처리.
2. **Encounter / 기본 전투:** 방 진입 Trigger, Dormant·Encounter, 시작 Roar와 공격 금지·종료 후 Phase 1, Walk·접근, Ice Spike, Ground Smash, Telegraph/Recovery/Cooldown, 거리 기반 선택.
3. **Phase 2:** HP 50% 판정, 전환 1회 제한, PhaseTransition, 공격 AI 정지, 전환 Roar, BossVisual 확대, EyeGlow 제작·Animator Track·활성화, 패턴 강화·템포 조절.
4. **Phase 2 추가 연출:** Ice Aura, Phase Transition VFX, Ice Trail, 강화 Shockwave, 추가 Camera Shake, Phase SFX.
5. **Game Feel:** Camera Shake, Hit Stop, Hit Flash, 공격·피격 VFX, 공격 SFX, 사망 연출, 최종 타격감 조정.

MVP 완성: 플레이어가 보스 방에 진입해 전투하고, 공격을 회피하며 피해를 주고, HP를 모두 줄여 처치할 수 있다. 최소 패턴은 Slam / Swing / Charge.

목표 완성: 진입 → Encounter Roar → Phase 1의 5패턴 → HP 50% → Phase Transition Roar → Scale 증가 + 붉은 안광 → Phase 2 강화 패턴 → Death.

## 9. 미정 사항과 기존 기록의 관계

다음은 원문에서 수치나 정책이 확정되지 않았거나 선택지로 남은 사항이다. 임의 확정하지 않는다.

- Phase 2 크기 배율, 공격 판정 확대 여부·크기.
- 현재 공격을 끝낸 뒤 전환할지, 안전하게 중단할지의 구체적 기준.
- Encounter/전환 중 피격·무적 정책, 전환 중 HP 0의 처리 순서.
- 보스 HP·피해량, 거리 임계값, 패턴 선택 확률, 강화 수치·템포, VFX 지속 시간.
- 사망 안광 OFF/Fade Out 선택, 벽 충돌 경직 채택 및 세부값.
- 재도전 시 Encounter·Phase 상태 초기화 정책.

이전 맵 아트 기록의 보스 크기 5~6배는 과거 시각 비교 기준으로 남으며 v2.0의 Phase 2 확대 배율로 해석하지 않는다. 과거의 점프 후 내려찍기는 v2.0의 5개 패턴 목록에 포함되지 않는다. 최신 구현 요구로 자동 추가하지 않으며 v2.0의 패턴 목록을 우선한다.

기존 보스전 중앙 파괴 바닥·낙하 연결과 이 보스의 Phase 전환·사망 사이의 연동 조건은 이 기획서에서 새로 확정되지 않았다. 자동 연결하지 않는다.

## 10. 변경 이력

- 2026-09-29: 사용자 제공 v2.0을 최신 보스 기준으로 기록. 보스 기획은 Docs/Boss에서 별도로 관리하며 AGENTS.md에는 보스 v2.0 항목을 두지 않는다. 이번 작업은 문서 보존이며 게임 구현 및 플레이 검증은 수행하지 않았다.
