# 설산 거인 얼음가시 전용 동작 v01

> **과거 참고용 — v3 제작으로 런타임 적용 철회 (2026-09-29).** 아래는 당시 작업 기록이다. v01 시트·클립·추가 스크립트는 Assets에서 제거했고 [ArchivedAssets](ArchivedAssets/)에 참고 사본을 보관했다. 기존 보스·가시 프리팹, VFX 클립, AnimatorController, 공격 데이터와 기존 코드의 v01 연동 변경도 제거했다. 현재 작업은 [v3 제작 기록](../IceThorn_v3/Implementation.md)을 따른다. 아래 재생·이벤트·콜라이더 설명은 현재 프로젝트 동작이 아니다.

2026-09-29 사용자 요청: Slam과 다른 전용 동작, Idle 시작·복귀, 2048×1024 시트 / 512×512 셀, Idle·Walk와 같은 피봇·기준 위치, 팔과 기존 BossIceThorn 프리팹에 콜라이더.

## 결과물

- [Body 시트 참고 사본](ArchivedAssets/Boss_IceThorn_v01.png): 실제 2048×1024 RGBA, 4열×2행, 8개 Sprite.
- [Body 애니메이션 참고 사본](ArchivedAssets/Boss_IceThorn_v01.anim): 20 FPS 타임라인, 1.8초, 비반복, 프레임별 유지 시간 적용.
- [기존 보스 프리팹](../../../Assets/03.Prefabs/Boss/Boss_SnowGiant.prefab): 전용 공격 컴포넌트, Colliders/IceThornArmHitbox, Colliders/IceThornEffectPoint 추가.
- [기존 가시 프리팹](../../../Assets/03.Prefabs/FX/BossIceThorn.prefab): 기존 13프레임 이펙트 유지. Trigger와 Kinematic Rigidbody2D, 수명 정리 컴포넌트 추가. 누락된 초기 Sprite 참조는 현재 이펙트의 첫 Sprite로 수정.
- [재생 미리보기](Preview.html): 좌우 방향·타임라인·콜라이더 표시.

## 이미지 제작과 정렬

내장 image_gen으로 생성. [최종 프롬프트](Prompt.txt), [생성 원본](../../../ArtSource/Boss_IceThorn_v01/Generated.png), Idle·Walk 참조 사본과 [조립 스크립트](../../../ArtSource/Boss_IceThorn_v01/PackSheet.ps1)를 프로젝트에 보관한다.

요청 해상도는 2048×1024였고 생성 원본의 실제 해상도는 1774×887이다. 중간 자세는 동일한 1.14 배율로 등비 리샘플링하고 발 기준선에 정렬해 최종 시트를 2048×1024로 조립했다. 확대를 원본 세부 묘사 증가로 해석하지 않는다. 알파 8/255 미만 배경 노이즈만 제거했다. 0번과 7번 셀은 Idle_0 RGBA 픽셀을 그대로 복사했다. [조립 수치](Alignment.txt).

모든 셀은 512×512, Pivot Bottom Center (0.5,0), 픽셀 피봇 (256,0), PPU 50이다. Idle·Walk와 같은 설정이다. 루트 Transform Scale과 기존 Idle/Walk/Slam 원본은 변경하지 않았다. 푸른 눈 Body 기준이며 이번 작업에 붉은 EyeGlow 시트는 포함하지 않는다.

## 타이밍

| 셀 | 시작 시간 | 동작 |
| --- | --- | --- |
| 0 | 0.00초 | Idle_0 원본 |
| 1 | 0.08초 | 몸 낮추기 |
| 2 | 0.25초 | 손을 지면 쪽으로 |
| 3 | 0.45초 | 손바닥을 위로 돌리고 힘 모으기 |
| 4 | 0.85초 | 팔을 끌어올림 |
| 5 | 0.95초 | 들어 올린 팔과 몸의 후속 동작 |
| 6 | 1.30초 | 팔 내리며 회복 |
| 7 | 1.65초 | Idle_0 원본 복귀 |

- 0.60초: `SpawnIceThorn` 이벤트. 기존 이펙트의 작은 시작 프레임부터 재생.
- 0.85~1.10초: 팔 Trigger 활성. BoxCollider2D 위치·크기는 Body 프레임에 맞춰 변한다.
- 이펙트 자체 0.25~0.6667초(보스 동작 기준 0.85~1.2667초): 가시 Trigger 활성. 크기·높이는 솟음과 붕괴에 맞춰 변한다.
- 이펙트 1.0833초: `StopEffect`로 소멸. 중단 시에도 1.2초 수명 제한으로 정리한다.
- 1.70초: `FinishIceThorn`, 추가 Recovery 0.5초 시작.
- 1.80초: Animator가 Idle로 복귀. 공격 소유권은 Recovery 종료까지 유지한다.

몸을 낮추는 시작부터 팔 공격까지 약 0.85초의 준비 동작을 확보했다. 별도의 긴 바닥 균열 Telegraph 자산은 이번 제작 범위에 포함하지 않았다. 실제 전투에서 플레이어 주변에 생성할 경우 위치 확정·바닥 경고 연출은 별도 연결한다.

## 재생과 연결

Animator `Boss_SnowGiantAC`에 `IsIceThorn` Trigger와 `Boss_IceThorn` 상태를 추가했다. Idle/Walk에서만 진입하고 종료 후 Idle로 돌아간다.

Play 모드에서 보스의 `BossIceThornAttack` 컴포넌트 메뉴 **Play Ice Thorn (Play Mode)**로 확인할 수 있다. 유효한 Target, Idle/Walk 상태, 쿨다운·거리 조건이 필요하다. 코드 연결은 `BossAttackController.TryStartIceThornAttack()`을 호출한다. Animator Trigger만 직접 호출하면 공격 소유권 및 VFX 생성 조건을 우회하므로 이 진입 함수를 사용한다.

기존 세 공격의 무작위 선택 목록에는 추가하지 않았다. 현재 Phase 상태 시스템이 없는 상태에서 Phase 1 공격 목록을 임의로 바꾸지 않기 위한 구성이다. Phase 2 선택 로직은 후속 연결 대상이다.

공격 데이터의 초기 검토값은 최소 거리 0 / 최대 30 월드 단위 / Recovery 0.5초 / Cooldown 6초 / Damage 1 / Weight 10이다. 현재 프리팹 Scale 2.7에 맞춘 테스트 범위이며 최종 밸런스가 아니다. 새 타입은 기존 enum 값 뒤에 추가해 Slam/Swing/Charge/Roar 직렬화 값을 보존했다.

이펙트 생성점은 보스 로컬 (-5.8,2.025,0), 원본 프리팹 Scale (1.5,1.5,1)와 부모 스케일을 반영한다. 생성 후 월드 좌표에 고정해 보스의 복귀나 방향 전환에 끌려가지 않는다. 방향 반전도 반영한다. 경사 지면 탐색·플레이어 위치 추적은 포함하지 않았다.

두 콜라이더는 **공격 영역용 Trigger**다. 물리 벽이나 발판으로 사용하지 않는다. 현재 프로젝트의 기존 보스 Hitbox와 마찬가지로 피해 적용 코드는 이번에 추가하지 않았다. `Health.TakeDamage` 호출·대시 무적·동일 공격 중 중복 피격 억제는 전투 판정 단계에서 연결해야 한다.

## 검증

- Unity 6000.3.22f1에서 실제 Import 확인: 8개 Sprite, 각각 Rect 512×512 / Pivot (256,0) / PPU 50.
- 실제 보스 프리팹과 가시 프리팹을 임시 Preview Scene에서 샘플링·렌더. [왼쪽](Unity_4.png), [오른쪽](Unity_Mirrored.png), [시간별 기록](Unity_Samples.txt).
- 분리한 임시 Play 씬에서 실제 Animator·이벤트·Collider2D를 검증했다. 8프레임 재생, Trigger 활성·Physics2D 조회, 전조/복귀 중 팔 OFF, 정상 종료, 좌우 생성, 사망 취소 및 가시 정리를 확인. [11개 검사 결과](PlayValidation.txt).
- 검증용 임시 씬·스크립트는 정리하고 Play 시작 씬 설정을 복원했다. MinsuScene으로 돌아왔으며 dirty=false 확인. 기존 씬 파일은 수정하지 않았다.
- 최종 피해 판정, 실제 플레이어 회피, 지형 적응, Phase 2 자동 실행, 붉은 안광은 이 검증에 포함하지 않는다.

## 변경 전 자료

[Before](Before/)에는 수정 전 보스 프리팹, 가시 프리팹·클립, 보스 AnimatorController, 공격 데이터 사본이 있다. 새 시트·클립은 v01 파일로 추가했다. 제작용 C# 기록은 반복 실행을 위한 멱등 도구가 아니므로 기존 에셋 위에 재실행하지 않는다.
