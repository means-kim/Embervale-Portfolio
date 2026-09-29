# 설산 거인 얼음가시 — v3.0 스프라이트·클립

2026-09-29. [보스 기획서 v3.0](../SnowGiant_Boss_Design_v3.0.md)에 따라 전용 Body 동작을 다시 제작했다. 사용자 요청 범위는 스프라이트와 애니메이션 클립이며, 전투 스크립트는 사용자가 작성한다.

## 결과물

- [Boss_IceThorn_v3.png](../../../Assets/04.Sprites/Boss/Boss_IceThorn_v3.png): **4096×1024 RGBA, 8열×2행, 512×512 셀 16개**.
- [Boss_IceThorn_v3.anim](../../../Assets/05.Animation/Boss/Boss_IceThorn_v3.anim): **3.00초, 비반복**, 고유 공격 스프라이트 16장과 기존 `Boss_Idle_0` 참조.
- [동작 미리보기](Preview.html): 재생·속도·프레임 선택·좌우 반전·Idle 겹치기·셀 중심/피봇 표시.
- [Unity 실제 렌더](Unity_Compression.png), [클립 검증](Unity_Verification.txt), [스프라이트 검증](Sprite_Verification.txt).

## 동작

Idle → 무릎과 골반 낮추기 → 양팔을 몸 가까이 당겨 힘 모으기 → **0.2초 Hold** → 넓게 버티며 양손을 지면에 내리기 → 상체와 무릎이 깊게 내려가는 압축 → 손·상체 반동과 중심 회복 → Idle.

머리 위에서 한 팔을 내리치는 Slam과 구분되도록 양손을 낮은 몸 앞에서 모으는 전조와 지면을 누르는 자세를 사용했다. 팔만 움직이는 대신 다리 벌림, 골반 높이, 몸통 기울기와 방향을 함께 바꾼다. 푸른 눈 Body 기준이며 Phase 2 EyeGlow는 별도 후속 소재다.

Ready/Recovery의 Idle은 시트 16칸에 복사하지 않는다. **16칸 모두 신규 고유 동작**이고 클립에서 기존 Idle 시트를 별도로 참조한다.

## Idle·Walk 정렬 기준

| 항목 | 적용값 |
| --- | --- |
| 참조 | 현재 `Boss_Idle.png`, `Boss_Walk.png`와 두 Import 설정 |
| 셀 | 512×512 px |
| 셀의 기하학적 중심 | (256, 256) |
| Unity 피봇 | **Bottom Center (0.5, 0)** / 픽셀 **(256, 0)** |
| PPU | **50** |
| Slice Offset / Padding | (0, 0) / (0, 0) |
| 프레임 순서 | 위 행 왼쪽→오른쪽, 아래 행 왼쪽→오른쪽 |
| Import | Multiple, Max Size 4096, Uncompressed, Bilinear, Mip Map OFF |
| 위치·배율 | 기존 발 접지선과 몸 크기를 기준으로 공통 좌표계 사용 |

셀 중심을 피봇으로 설정하거나, 프레임마다 그림의 바운딩 박스를 기준으로 자동 Trim·중앙 정렬·확대하지 않았다. 클립에 Transform 위치·Scale 키가 없다. 자세에 따른 실제 몸 이동과 발의 수 픽셀 차이는 그림의 움직임이며 피봇 변경이 아니다.

기존 Idle/Walk PNG와 `.meta`는 수정하지 않았다.

## 클립 타이밍

| 시작 시간 | Sprite | 역할 |
| ---: | --- | --- |
| 0.00 | 기존 Idle_0 | Ready |
| 0.25 | 01 | 몸 낮추기 시작 |
| 0.40 | 02 | 체중 준비 |
| 0.55 | 03 | 무릎·골반 낮추기 |
| 0.70 | 04 | 양손 모으기 |
| 0.85 | 05 | 몸 앞에 힘 모으기 |
| 0.95 | 06 | Maximum Anticipation, 1.15초까지 Hold |
| 1.15 | 07 | 다리로 버티며 전방으로 힘 전달 |
| **1.25** | **08** | **양손의 첫 지면 접촉 기준** |
| 1.35 | 09 | 하향 체중 전달 |
| 1.45 | 10 | 깊은 압축 |
| 1.50 | 11 | Follow Through 시작 |
| 1.65 | 12 | 상체 반동·지면에서 손 떼기 |
| 1.80 | 13 | 중심 회복 |
| 1.95 | 14 | 기립 후행 동작 |
| 2.10 | 15 | 팔·몸 안정 |
| 2.20 | 16 | Idle로 이어지는 복귀 자세 |
| 2.30~3.00 | 기존 Idle_0 | Recovery |

클립 샘플 레이트는 60이다. 이는 60개의 고유 그림이나 균등 재생을 의미하지 않는다. 각 그림의 유지 시간이 위 표처럼 다르고 마지막 Idle 키는 2.983333초에 있으며 클립 종료는 3.00초다.

## 사용자가 연결할 부분

현재 보스와 동일한 **Animator 루트의 SpriteRenderer → `m_Sprite`** 트랙 하나만 포함한다. AnimatorController의 상태·Trigger·Transition은 새로 추가하지 않았다. 나중에 SpriteRenderer를 `BossVisual/Body` 자식으로 이동하면 클립 바인딩 경로도 함께 옮긴다.

`Assets/03.Prefabs/FX/BossIceThorn.prefab`은 프로젝트의 기존 VFX로 보존했다. 1.25초를 Body 지면 접촉의 참고 시점으로 사용할 수 있다. 가시 VFX 생성·성장·Telegraph와 실제 피해 시점은 해당 이펙트 재생에 맞춰 별도로 연결한다. 이 시점을 피해 활성화로 자동 확정하지 않는다.

새 게임 스크립트, Animation Event, 콜라이더/피해 트랙, VFX 생성, Phase 2 공격 선택 로직은 포함하지 않는다. 팔과 가시의 판정은 사용자가 작성할 로직에서 전조·복귀 동안 끄고 실제 타격 구간에만 활성화하는 기획 기준을 따른다.

## 이전 작업 정리

- v01 Body PNG·`.meta`, Body 클립·`.meta`를 Assets에서 제거했다.
- 제가 추가했던 `BossIceThornAttack.cs`, `BossIceThornLifetime.cs`와 각 `.meta`를 제거했다.
- `BossColliderRig`, `BossAttackController`, `BossAttackType`에 추가했던 v01 연동 코드를 제거했다.
- 기존 보스 프리팹의 전용 컴포넌트·팔 Hitbox·EffectPoint, AnimatorController의 v01 상태·Trigger·Transition, 공격 데이터의 IceThorn 항목을 제거했다.
- 기존 가시 프리팹과 VFX 클립에 추가했던 콜라이더·수명 컴포넌트·Animation Event·판정 트랙을 제거하고 이전 상태로 돌렸다. **원래 가시 VFX 프리팹·클립 자체는 보존했다.**
- 원본/이전 PNG, 제작·검증 기록은 [ArtSource v01](../../../ArtSource/Boss_IceThorn_v01/)과 [Docs v01](../IceThorn_v01/)에 참고용으로 보존했다. 삭제한 런타임 에셋과 수정 전환 당시 내용은 [ArchivedAssets](../IceThorn_v01/ArchivedAssets/)에 복사했고 스크립트 사본은 `.txt`로 보관한다. v01 문서·미리보기는 과거 참고용으로 표시했다.

확인한 기존 8개 파일의 변경분만 복원했다. 사용자 씬, 기존 Idle/Walk, `.husky/pre-commit`, 프로젝트 설정과 AGENTS.md는 이번 작업에서 변경하지 않았다.

## 생성 원본과 검증 범위

[생성 프롬프트](Prompt.txt), [생성 원본](../../../ArtSource/Boss_IceThorn_v3/Generated_4x4.png), [Idle 참조](../../../ArtSource/Boss_IceThorn_v3/Idle_Reference.png), [시트 조립 기록](../../../ArtSource/Boss_IceThorn_v3/PackAtlas.ps1), [정렬 수치](Atlas_Registration.txt)를 보관한다.

이미지 생성에는 4×4 작업용 배열을 요청했고, **실제 생성 결과는 1254×1254**였다. 모든 자세에 동일한 **1.39배 등비 리샘플링**을 적용하고 행별 공통 접지선을 등록해 최종 **4096×1024 / 8×2**로 조립했다. 이는 생성 원본 자체가 네이티브 4096인 경우와 다르며, 확대가 세부 묘사를 추가하지 않는다. 알파 8/255 미만 노이즈를 정리했고 원본 셀 11/12 사이의 분리 경계를 보정해 이웃 발 픽셀이 섞이지 않게 했다. 이 경계 보정은 그림 위치를 재중앙 정렬하는 처리가 아니다.

- 실제 PNG 크기, Unity Import 후 4096×1024 유지, 16개 Slice·피봇·PPU 확인.
- 개별 프레임 PNG 16개의 해시가 모두 다름을 확인.
- 알파 40/255 초과 픽셀 기준 모든 셀의 네 변에 닿는 픽셀이 0개임을 확인. 손·발·얼음의 셀 경계 잘림을 점검했다.
- Unity에서 클립의 모든 키를 샘플링해 순서와 Idle 참조, 루트 위치·배율 고정을 확인.
- 기존 보스 프리팹을 임시 Preview Scene에 불러와 Ready·Hold·Impact·압축·복귀·좌우 반전의 8개 렌더를 저장하고 주요 자세와 Idle을 비교했다.
- 클립 3초, 비반복, Sprite 트랙 1개, Float 트랙 0개, Animation Event 0개. Unity Console 오류 0개.
- 임시 Preview Scene을 정리했고 기존 `MinsuScene`의 dirty=false를 확인했다.
- 실제 전투, VFX/피해 동기화, 플레이어 회피, Phase 2 연동은 이 검증에 포함되지 않는다. HTML 미리보기 파일은 제공하되 앱 브라우저의 로컬 파일 URL 차단으로 자동 브라우저 검수는 하지 못했다. 실제 이미지 확인은 Unity 렌더로 수행했다.
