# 설산 거인 얼음가시 v3.0 — 팔 상승 개정본

2026-09-29 최신 사용자 지시: v01의 아래에서 위로 팔을 끌어올리는 느낌을 사용하고, v3.0처럼 전조·체중 이동·전신 동작을 크게 만든다. 기존 가시 이펙트와 팔·가시 콜라이더도 함께 구성한다. 전투 스크립트는 사용자가 작성한다.

## 결과물

- [Body 시트](../../../Assets/04.Sprites/Boss/Boss_IceThorn_v3.png): **4096×1024 RGBA / 8열×2행 / 고유 16프레임**.
- [3초 애니메이션 클립](../../../Assets/05.Animation/Boss/Boss_IceThorn_v3.anim): Body, 가시 VFX, 팔·가시 Trigger를 하나의 타임라인에서 제어.
- [보스 프리팹](../../../Assets/03.Prefabs/Boss/Boss_SnowGiant.prefab): 팔 Hitbox와 기존 가시 프리팹의 중첩 인스턴스 추가.
- [기존 가시 프리팹](../../../Assets/03.Prefabs/FX/BossIceThorn.prefab): BoxCollider2D Trigger·Kinematic Rigidbody2D 추가. 기존 13프레임 VFX 사용.
- [동작·이펙트·콜라이더 미리보기](Preview.html), [Unity 실제 렌더](LiftRevision/Unity_Apex.png), [콜라이더 렌더](LiftRevision/Hitboxes_Apex.png).
- [최신 기획서](../SnowGiant_Boss_Design_v3.0.md)의 Pattern 04도 팔 상승 방향으로 수정했다.

## 동작과 유지한 규격

**Idle → 깊은 웅크림·체중 이동 → 몸통 비틀기 → 손바닥을 위로 펼친 낮은 전조 → Hold → 다리를 펴며 팔을 머리 위까지 끌어올림 → 상체 젖힘과 팔·털의 관성 → 팔을 내리고 중심 회복 → Idle.**

공격 팔은 화면 왼쪽 팔이다. 반대 팔은 전조에서 뒤로 움직여 균형을 잡는다. 아래로 내려찍는 Slam과 구분되는 손바닥 방향과 상승 궤적을 사용한다. Body는 푸른 눈이며 붉은 EyeGlow는 별도 소재다.

| 항목 | 값 |
| --- | --- |
| 셀 | 512×512 px |
| 시트 / 순서 | 4096×1024, 8×2 / 위 행 왼쪽→오른쪽, 아래 행 왼쪽→오른쪽 |
| 피봇 | **Bottom Center (0.5, 0)** / 픽셀 **(256, 0)** |
| 셀의 기하학적 중심 | (256, 256) |
| PPU | **50**, 현재 Idle·Walk와 동일 |
| Slice Offset / Padding | (0, 0) / (0, 0) |
| Import | Multiple, Max Size 4096, Uncompressed, Bilinear, Mip Map OFF |
| Ready / Recovery | 기존 Boss_Idle_0 직접 참조, 고유 16장에 포함하지 않음 |

기존 Idle·Walk 원본과 .meta는 그대로 유지했다. v3 시트·클립 경로와 .meta GUID, 16개 Sprite ID도 유지했다. 프레임별로 몸체를 자동 Trim·재중앙 정렬·확대하지 않았으며, 루트 Transform 위치·Scale 애니메이션은 없다.

## 타임라인

| 구간 | 시간 | 역할 |
| --- | --- | --- |
| Ready | 0.00~0.25초 | 기존 Idle |
| Anticipation | 0.25~1.15초 | 신규 01~06, 몸 낮추기와 팔 준비 |
| Maximum Anticipation Hold | **0.95~1.15초** | 06번 자세 0.2초 유지 |
| Attack | **1.15~1.50초** | 07~10, 팔을 빠르게 끌어올림 |
| Follow Through | 1.50~2.30초 | 11~16, 정점 관성과 복귀 |
| Recovery | 2.30~3.00초 | 기존 Idle |

각 고유 프레임 시작 시간은 0.25, 0.40, 0.55, 0.70, 0.85, 0.95, 1.15, 1.25, 1.35, 1.45, 1.50, 1.65, 1.80, 1.95, 2.10, 2.20초다. 60 Sample Rate는 고유 그림 60장을 뜻하지 않는다. 클립은 비반복이며 정확히 3초다.

## 이펙트와 콜라이더

| 대상 | 활성 구간 | 구성 |
| --- | --- | --- |
| 가시 VFX 표시 | **0.90~1.9833초** | 작은 기존 시작 프레임부터 상승·붕괴까지 |
| 팔 Trigger | **1.15~1.50초** | 손·전완 위치에 맞춘 BoxCollider2D의 위치·크기·회전 |
| 가시 Trigger | **1.15~1.5667초** | 기존 VFX의 자체 시간 0.25~0.6667초 |
| 준비·복귀 | OFF | 공격 전조의 초반 작은 VFX에는 피해 영역 없음 |

팔 BoxCollider2D는 올라가는 손·전완에 맞춰 이동하며 0→22→28도 회전한다. 가시 BoxCollider2D는 **프레임별 X/Y 오프셋과 크기**를 함께 조절한다. 원본 VFX의 불투명 픽셀 범위에서 가장자리 1~2픽셀을 안쪽으로 잡았으며, 단일 Box라 가시 사이의 빈 공간까지 완벽히 따르는 Polygon 판정은 아니다.

현재 프리팹 구조:

```text
Boss_SnowGiant
├─ SpriteRenderer / Animator
└─ Colliders                       ← 기존 BossColliderRig가 좌우 반전
   ├─ 기존 Hurtbox / 공격 Hitbox
   ├─ IceThornArmHitbox             ← 기본 OFF, 팔 상승에 맞춰 이동·회전
   └─ IceThornEffectPoint           ← local (-5.8, 2.0, 0)
      └─ BossIceThorn               ← 기존 FX 프리팹 중첩, Scale 1.5
         ├─ SpriteRenderer         ← 기본 OFF, 보스 클립에서 제어
         ├─ BoxCollider2D          ← 기본 OFF, 보스 클립에서 제어
         ├─ Rigidbody2D            ← Kinematic
         ├─ Animator               ← 중첩 인스턴스에서는 OFF
         └─ BossEffect             ← 중첩 인스턴스에서는 OFF
```

중첩 FX는 별도 Animator를 동시에 재생하지 않는다. 부모 클립이 Body와 VFX의 Sprite·판정을 함께 바꾸므로 재생 속도 변경에도 같은 타임라인을 따른다. **보스에 붙은 이펙트이므로 보스가 이동하면 함께 이동한다.** 실제 공격에서는 보스 이동·방향 전환을 잠그는 처리를 사용자가 연결한다.

원본 FX 프리팹을 별도로 생성해 사용하는 경우에는 자체 Animator로 재생한다. 원래 존재하던 `BossEffect.StopEffect()`를 종료 이벤트로 호출하도록 연결했다. 새 수명 스크립트를 추가하지 않았다. 월드 위치에 고정된 가시가 필요하면 이 원본 프리팹을 별도로 생성하고 중첩 표시와 중복되지 않게 연결한다.

## Animator와 사용자 스크립트 연결

[Boss_SnowGiantAC](../../../Assets/05.Animation/Boss/Boss_SnowGiantAC.controller)에 다음을 추가했다.

- `IsIceThorn` Trigger.
- `Boss_IceThorn` 상태와 v3 클립.
- Idle·Walk → IceThorn 진입, 3초 후 Idle 복귀.
- 기존 상태들과 동일하게 Write Defaults ON. 다른 상태로 전환할 때 새 팔·가시 판정과 VFX가 기본 OFF로 돌아간다.

Animator의 `IsIceThorn`은 현재 시각 재생 진입점이다. 자동 공격 선택에는 등록하지 않았고 공격 소유권·쿨다운·이동 정지·피해 적용은 연결하지 않았다. 기존 `BossAttackController`의 공격 실행 상태와 함께 관리하도록 사용자가 작성한다.

기존 공격 중 피격 경직을 억제하는 정책은 유지했다. 검증에서는 명시적으로 Hit 상태로 이동했을 때의 OFF 복원을 확인했으며, IceThorn에 새로운 피격 취소 정책을 만들지는 않았다. 사망은 기존 Any State → Death 경로를 따른다.

## 검증

- [Animator 검증 15항목](LiftRevision/Animator_Verification.txt): Idle/Walk에서 Trigger 진입, 전조·상승·복귀 구간, 자연 종료, 명시적 Hit 전환, Death Trigger 전환 및 판정/VFX OFF 복원.
- 기존 이벤트의 게임 코드 호출을 분리하기 위해 검증용 복제 클립에서만 이벤트를 제거하고 Editor Animator를 평가했다. 원본 이벤트나 게임 코드는 검증 때문에 변경하지 않았다.
- [Unity 샘플 기록](LiftRevision/Unity_Samples.txt): 10개 시간·방향 샘플. 각각 실제 렌더와 팔/가시 판정 영역 렌더를 저장했다.
- [정렬 기록](LiftRevision/Alignment.txt), [VFX 픽셀 범위](LiftRevision/FX_AlphaBounds.txt).
- 기존 BossColliderRig의 좌우 반전에 새 팔·가시가 함께 따라가는 것을 렌더로 확인.
- Play 모드의 실제 플레이어 피해·회피, Phase 2 선택, 지형 탐색·월드 고정 생성은 검증 범위가 아니다.
- HTML 자동 브라우저 검수는 로컬 파일 URL 제한으로 수행하지 않았으며, 실제 이미지 검수는 Unity 렌더로 수행했다.

## 제작·보존

**내장 image_gen**으로 생성했다. [최종 프롬프트](LiftRevision/Prompt.txt), [생성 원본](../../../ArtSource/Boss_IceThorn_v3/LiftRevision/Generated_4x4.png), [조립 작업 파일](../../../ArtSource/Boss_IceThorn_v3/LiftRevision/PackSheet.ps1)을 프로젝트에 보관한다.

요청한 작업용 4×4 이미지의 실제 생성 결과는 **1254×1254 RGBA**였다. 모든 동작에 동일한 **1.375배** 등비 리샘플링을 적용하고 공통 행 접지선·열 기준을 사용해 최종 4096×1024 시트로 재배열했다. 네이티브 4096 그림을 생성한 것으로 설명하지 않으며 확대가 세부 묘사를 추가하지 않는다. 알파 8/255 미만 노이즈와 이웃 그림이 섞이지 않도록 분리 경계를 정리했다.

직전의 아래로 지면을 누르는 v3 시트·클립·문서와 연동 대상의 변경 전 파일은 [Before](LiftRevision/Before/)에 참고용으로 보관한다. 기존 v01 자료도 참고용으로 유지한다. v01 공격·수명 스크립트를 복원하지 않았고 **Assets의 .cs 파일과 공격 DATA, 사용자 씬, AGENTS.md는 변경하지 않았다.**
